﻿using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.Base.ViewModel.Requirement;
using MixItUp.WPF.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MixItUp.WPF.Controls.Command
{
    /// <summary>
    /// Interaction logic for InteractiveCommandDetailsControl.xaml
    /// </summary>
    public partial class InteractiveCommandDetailsControl : CommandDetailsControlBase
    {
        public InteractiveGameListingModel Game { get; private set; }
        public InteractiveGameVersionModel Version { get; private set; }
        public InteractiveSceneModel Scene { get; private set; }
        public InteractiveControlModel Control { get; private set; }

        private InteractiveCommand command;

        public InteractiveCommandDetailsControl(InteractiveCommand command)
        {
            this.command = command;
            this.Control = command.Control;

            InitializeComponent();
        }

        public InteractiveCommandDetailsControl(InteractiveGameListingModel game, InteractiveGameVersionModel version, InteractiveSceneModel scene, InteractiveControlModel control)
        {
            this.Game = game;
            this.Version = version;
            this.Scene = scene;
            this.Control = control;

            InitializeComponent();
        }

        public override async Task Initialize()
        {
            this.ButtonTriggerComboBox.ItemsSource = EnumHelper.GetEnumNames<InteractiveButtonCommandTriggerType>();

            if (this.Control != null && this.Control is InteractiveButtonControlModel)
            {
                this.ButtonTriggerComboBox.IsEnabled = true;
                this.ButtonTriggerComboBox.SelectedItem = EnumHelper.GetEnumName(InteractiveButtonCommandTriggerType.MouseDown);
                this.SparkCostTextBox.IsEnabled = true;
                this.SparkCostTextBox.Text = ((InteractiveButtonControlModel)this.Control).cost.ToString();
            }

            if (this.command != null)
            {
                if (this.command.Button != null)
                {
                    this.ButtonTriggerComboBox.SelectedItem = EnumHelper.GetEnumName(this.command.Trigger);
                    this.UnlockedControl.Unlocked = this.command.Unlocked;
                    this.Requirements.SetRequirements(this.command.Requirements);
                }

                this.UnlockedControl.Unlocked = this.command.Unlocked;

                IEnumerable<InteractiveGameListingModel> games = await ChannelSession.Connection.GetOwnedInteractiveGames(ChannelSession.Channel);
                this.Game = games.FirstOrDefault(g => g.id.Equals(this.command.GameID));
                if (this.Game != null)
                {
                    this.Version = this.Game.versions.First();
                    this.Version = await ChannelSession.Connection.GetInteractiveGameVersion(this.Version);
                    this.Scene = this.Version.controls.scenes.FirstOrDefault(s => s.sceneID.Equals(this.command.SceneID));
                }
            }
        }

        public override async Task<bool> Validate()
        {
            if (this.Control is InteractiveButtonControlModel)
            {
                if (this.ButtonTriggerComboBox.SelectedIndex < 0)
                {
                    await MessageBoxHelper.ShowMessageDialog("An trigger type must be selected");
                    return false;
                }

                if (!int.TryParse(this.SparkCostTextBox.Text, out int sparkCost) || sparkCost < 0)
                {
                    await MessageBoxHelper.ShowMessageDialog("A valid spark cost must be entered");
                    return false;
                }
            }

            if (!await this.Requirements.Validate())
            {
                return false;
            }

            return true;
        }

        public override CommandBase GetExistingCommand() { return this.command; }

        public override async Task<CommandBase> GetNewCommand()
        {
            if (await this.Validate())
            {
                InteractiveButtonCommandTriggerType trigger = EnumHelper.GetEnumValueFromString<InteractiveButtonCommandTriggerType>((string)this.ButtonTriggerComboBox.SelectedItem);

                RequirementViewModel requirements = this.Requirements.GetRequirements();

                if (this.command == null)
                {
                    if (this.Control is InteractiveButtonControlModel)
                    {                
                        this.command = new InteractiveCommand(this.Game, this.Scene, (InteractiveButtonControlModel)this.Control, trigger, requirements);
                    }
                    else
                    {
                        this.command = new InteractiveCommand(this.Game, this.Scene, (InteractiveJoystickControlModel)this.Control, requirements);
                    }
                    ChannelSession.Settings.InteractiveCommands.Add(this.command);
                }

                if (this.Control is InteractiveButtonControlModel)
                {
                    this.command.Trigger = trigger;
                    this.command.Button.cost = int.Parse(this.SparkCostTextBox.Text);
                    this.command.Requirements = requirements;

                    await ChannelSession.Connection.UpdateInteractiveGameVersion(this.Version);
                }
                this.command.Unlocked = this.UnlockedControl.Unlocked;
                return this.command;
            }
            return null;
        }
    }
}
