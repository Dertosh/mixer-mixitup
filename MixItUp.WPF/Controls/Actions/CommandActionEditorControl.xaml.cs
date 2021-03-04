﻿using MixItUp.Base.Model.Commands;
using MixItUp.Base.Util;
using MixItUp.Base.ViewModel.Actions;
using MixItUp.WPF.Windows.Commands;

namespace MixItUp.WPF.Controls.Actions
{
    /// <summary>
    /// Interaction logic for CommandActionEditorControl.xaml
    /// </summary>
    public partial class CommandActionEditorControl : ActionEditorControlBase
    {
        public CommandActionEditorControl()
        {
            InitializeComponent();
        }

        private void AddCommandButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommandEditorWindow window = new CommandEditorWindow(CommandTypeEnum.ActionGroup);
            window.CommandSaved += Window_CommandSaved;
            window.Show();
        }

        private async void EditCommandButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CommandActionEditorControlViewModel viewModel = (CommandActionEditorControlViewModel)this.DataContext;
            if (viewModel != null && viewModel.SelectedCommand != null)
            {
                if (viewModel.SelectedCommand.Type == CommandTypeEnum.PreMade)
                {
                    await DialogHelper.ShowMessage(MixItUp.Base.Resources.PreMadeCommandsCanNotBeEdited);
                }
                else
                {
                    CommandEditorWindow window = new CommandEditorWindow(viewModel.SelectedCommand);
                    window.Show();
                }
            }
        }

        private void Window_CommandSaved(object sender, CommandModelBase command)
        {
            if (command != null)
            {
                CommandActionEditorControlViewModel viewModel = (CommandActionEditorControlViewModel)this.DataContext;
                if (viewModel != null)
                {
                    viewModel.SelectedCommandType = command.Type;
                    viewModel.SelectedCommand = command;
                }
            }
        }
    }
}
