﻿using MixItUp.Base.Model.Actions;
using MixItUp.Base.Model.Overlay;
using MixItUp.Base.Model.Overlay.Widgets;
using MixItUp.Base.Services;
using MixItUp.Base.Util;
using MixItUp.Base.ViewModel.Overlay;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MixItUp.Base.ViewModel.Actions
{
    public enum OverlayActionTypeEnum
    {
        Text,
        Image,
        Video,
        YouTube,
        HTML,
        Timer,
        TwitchClip,
        DamageStreamBoss,
        AddToGoal,
        AddToPersistentTimer,
    }

    public class OverlayActionEditorControlViewModel : ActionEditorControlViewModelBase
    {
        public override ActionTypeEnum Type { get { return ActionTypeEnum.Overlay; } }

        public IEnumerable<OverlayActionTypeEnum> ActionTypes { get { return EnumHelper.GetEnumList<OverlayActionTypeEnum>(); } }

        public OverlayActionTypeEnum SelectedActionType
        {
            get { return this.selectedActionType; }
            set
            {
                if (this.selectedActionType != value)
                {
                    this.selectedActionType = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged(nameof(this.OverlayNotEnabled));
                    this.NotifyPropertyChanged(nameof(this.OverlayEnabled));
                    this.NotifyPropertyChanged(nameof(this.ShowItem));
                    this.NotifyPropertyChanged(nameof(this.ShowDamageStreamBoss));
                    this.NotifyPropertyChanged(nameof(this.ShowAddGoal));
                    this.NotifyPropertyChanged(nameof(this.ShowAddPersistTimer));

                    if (this.ShowItem)
                    {
                        if (this.SelectedActionType == OverlayActionTypeEnum.Text)
                        {
                            this.Item = new OverlayTextV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.Image)
                        {
                            this.Item = new OverlayImageV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.Video)
                        {
                            this.Item = new OverlayVideoV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.YouTube)
                        {
                            this.Item = new OverlayYouTubeV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.HTML)
                        {
                            this.Item = new OverlayHTMLV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.Timer)
                        {
                            this.Item = new OverlayTimerV3ViewModel();
                        }
                        else if (this.SelectedActionType == OverlayActionTypeEnum.TwitchClip)
                        {
                            this.Item = new OverlayTwitchClipV3ViewModel();
                        }

                        this.HTML = OverlayItemV3ModelBase.GetPositionWrappedHTML(this.Item.DefaultHTML);
                        this.CSS = OverlayItemV3ModelBase.GetPositionWrappedCSS(this.Item.DefaultCSS);
                        this.Javascript = this.Item.DefaultJavascript;
                    }
                }
            }
        }
        private OverlayActionTypeEnum selectedActionType;

        public bool OverlayNotEnabled { get { return !ServiceManager.Get<OverlayV3Service>().IsConnected; } }

        public bool OverlayEnabled { get { return !this.OverlayNotEnabled; } }

        public IEnumerable<OverlayEndpointV3Model> OverlayEndpoints { get { return ServiceManager.Get<OverlayV3Service>().GetOverlayEndpoints(); } }

        public OverlayEndpointV3Model SelectedOverlayEndpoint
        {
            get { return this.selectedOverlayEndpoint; }
            set
            {
                var overlays = ServiceManager.Get<OverlayV3Service>().GetOverlayEndpoints();
                if (overlays.Contains(value))
                {
                    this.selectedOverlayEndpoint = value;
                }
                else
                {
                    this.selectedOverlayEndpoint = ServiceManager.Get<OverlayV3Service>().GetDefaultOverlayEndpoint();
                }
                this.NotifyPropertyChanged();
            }
        }
        private OverlayEndpointV3Model selectedOverlayEndpoint;

        public bool ShowItem
        {
            get
            {
                return this.SelectedActionType == OverlayActionTypeEnum.Text || this.SelectedActionType == OverlayActionTypeEnum.Image ||
                    this.SelectedActionType == OverlayActionTypeEnum.Video || this.SelectedActionType == OverlayActionTypeEnum.YouTube ||
                    this.SelectedActionType == OverlayActionTypeEnum.HTML || this.SelectedActionType == OverlayActionTypeEnum.Timer ||
                    this.SelectedActionType == OverlayActionTypeEnum.TwitchClip;
            }
        }

        public OverlayItemV3ViewModelBase Item
        {
            get { return this.item; }
            set
            {
                this.item = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayItemV3ViewModelBase item;

        public OverlayPositionV3ViewModel Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayPositionV3ViewModel position = new OverlayPositionV3ViewModel();

        public string Duration
        {
            get { return this.duration; }
            set
            {
                this.duration = value;
                this.NotifyPropertyChanged();
            }
        }
        private string duration;

        public OverlayAnimationV3ViewModel EntranceAnimation
        {
            get { return this.entranceAnimation; }
            set
            {
                this.entranceAnimation = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayAnimationV3ViewModel entranceAnimation;

        public OverlayAnimationV3ViewModel ExitAnimation
        {
            get { return this.exitAnimation; }
            set
            {
                this.exitAnimation = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayAnimationV3ViewModel exitAnimation;

        public string HTML
        {
            get { return this.html; }
            set
            {
                this.html = value;
                this.NotifyPropertyChanged();
            }
        }
        private string html;

        public string CSS
        {
            get { return this.css; }
            set
            {
                this.css = value;
                this.NotifyPropertyChanged();
            }
        }
        private string css;

        public string Javascript
        {
            get { return this.javascript; }
            set
            {
                this.javascript = value;
                this.NotifyPropertyChanged();
            }
        }
        private string javascript;

        public bool ShowDamageStreamBoss { get { return this.SelectedActionType == OverlayActionTypeEnum.DamageStreamBoss; } }

        public ObservableCollection<OverlayWidgetV3Model> StreamBosses { get; set; } = new ObservableCollection<OverlayWidgetV3Model>();

        public OverlayWidgetV3Model SelectedStreamBoss
        {
            get { return this.selectedStreamBoss; }
            set
            {
                this.selectedStreamBoss = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayWidgetV3Model selectedStreamBoss;

        public string StreamBossDamageAmount
        {
            get { return this.streamBossDamageAmount; }
            set
            {
                this.streamBossDamageAmount = value;
                this.NotifyPropertyChanged();
            }
        }
        private string streamBossDamageAmount;

        public bool StreamBossForceDamage
        {
            get { return this.streamBossForceDamage; }
            set
            {
                this.streamBossForceDamage = value;
                this.NotifyPropertyChanged();
            }
        }
        private bool streamBossForceDamage = true;

        public bool ShowAddGoal { get { return this.SelectedActionType == OverlayActionTypeEnum.AddToGoal; } }

        public ObservableCollection<OverlayWidgetV3Model> Goals { get; set; } = new ObservableCollection<OverlayWidgetV3Model>();

        public OverlayWidgetV3Model SelectedGoal
        {
            get { return this.selectedGoal; }
            set
            {
                this.selectedGoal = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayWidgetV3Model selectedGoal;

        public string GoalAmount
        {
            get { return this.goalAmount; }
            set
            {
                this.goalAmount = value;
                this.NotifyPropertyChanged();
            }
        }
        private string goalAmount;

        public bool ShowAddPersistTimer { get { return this.SelectedActionType == OverlayActionTypeEnum.AddToPersistentTimer; } }

        public ObservableCollection<OverlayWidgetV3Model> PersistentTimers { get; set; } = new ObservableCollection<OverlayWidgetV3Model>();

        public OverlayWidgetV3Model SelectedPersistentTimer
        {
            get { return this.selectedPersistentTimer; }
            set
            {
                this.selectedPersistentTimer = value;
                this.NotifyPropertyChanged();
            }
        }
        private OverlayWidgetV3Model selectedPersistentTimer;

        public string TimeAmount
        {
            get { return this.timeAmount; }
            set
            {
                this.timeAmount = value;
                this.NotifyPropertyChanged();
            }
        }
        private string timeAmount;

        private Guid widgetID;

        public OverlayActionEditorControlViewModel(OverlayActionModel action)
            : base(action)
        {
            this.SelectedOverlayEndpoint = ServiceManager.Get<OverlayV3Service>().GetDefaultOverlayEndpoint();

            if (action.OverlayItemV3 != null)
            {
                OverlayEndpointV3Model overlayEndpoint = ServiceManager.Get<OverlayV3Service>().GetOverlayEndpoint(action.OverlayItemV3.OverlayEndpointID);
                if (overlayEndpoint != null)
                {
                    this.SelectedOverlayEndpoint = overlayEndpoint;
                }

                if (action.OverlayItemV3.Type == OverlayItemV3Type.Text)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.Text;
                    this.Item = new OverlayTextV3ViewModel((OverlayTextV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.Image)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.Image;
                    this.Item = new OverlayImageV3ViewModel((OverlayImageV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.Video)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.Video;
                    this.Item = new OverlayVideoV3ViewModel((OverlayVideoV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.YouTube)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.YouTube;
                    this.Item = new OverlayYouTubeV3ViewModel((OverlayYouTubeV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.HTML)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.HTML;
                    this.Item = new OverlayHTMLV3ViewModel((OverlayHTMLV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.Timer)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.Timer;
                    this.Item = new OverlayTimerV3ViewModel((OverlayTimerV3Model)action.OverlayItemV3);
                }
                else if (action.OverlayItemV3.Type == OverlayItemV3Type.TwitchClip)
                {
                    this.SelectedActionType = OverlayActionTypeEnum.TwitchClip;
                    this.Item = new OverlayTwitchClipV3ViewModel((OverlayTwitchClipV3Model)action.OverlayItemV3);
                }

                this.Position = new OverlayPositionV3ViewModel(action.OverlayItemV3.Position);
                this.Duration = action.Duration;
                this.EntranceAnimation = new OverlayAnimationV3ViewModel(Resources.Entrance, action.EntranceAnimation);
                this.ExitAnimation = new OverlayAnimationV3ViewModel(Resources.Exit, action.ExitAnimation);

                this.HTML = action.OverlayItemV3.HTML;
                this.CSS = action.OverlayItemV3.CSS;
                this.Javascript = action.OverlayItemV3.Javascript;
            }

            if (action.StreamBossID != Guid.Empty)
            {
                this.SelectedActionType = OverlayActionTypeEnum.DamageStreamBoss;
                this.widgetID = action.StreamBossID;
                this.StreamBossDamageAmount = action.StreamBossDamageAmount;
                this.StreamBossForceDamage = action.StreamBossForceDamage;
            }
            else if (action.GoalID != Guid.Empty)
            {
                this.SelectedActionType = OverlayActionTypeEnum.AddToGoal;
                this.widgetID = action.StreamBossID;
                this.GoalAmount = action.GoalAmount;
            }
            else if (action.PersistentTimerID != Guid.Empty)
            {
                this.SelectedActionType = OverlayActionTypeEnum.AddToPersistentTimer;
                this.widgetID = action.PersistentTimerID;
                this.TimeAmount = action.TimeAmount;
            }
        }

        public OverlayActionEditorControlViewModel()
            : base()
        {
            this.SelectedOverlayEndpoint = ServiceManager.Get<OverlayV3Service>().GetDefaultOverlayEndpoint();
            this.SelectedActionType = OverlayActionTypeEnum.Text;

            this.Item = new OverlayTextV3ViewModel();
            this.HTML = OverlayItemV3ModelBase.GetPositionWrappedHTML(this.Item.DefaultHTML);
            this.CSS = OverlayItemV3ModelBase.GetPositionWrappedCSS(this.Item.DefaultCSS);
            this.Javascript = this.Item.DefaultJavascript;

            this.EntranceAnimation = new OverlayAnimationV3ViewModel(Resources.Entrance);
            this.ExitAnimation = new OverlayAnimationV3ViewModel(Resources.Exit);
        }

        public override Task<Result> Validate()
        {
            if (this.ShowItem)
            {
                Result result = this.Item.Validate();
                if (!result.Success)
                {
                    return Task.FromResult<Result>(new Result(Resources.OverlayActionValidationErrorHeader + result.Message));
                }

                if (string.IsNullOrWhiteSpace(this.Duration))
                {
                    if (this.SelectedActionType != OverlayActionTypeEnum.Video && this.SelectedActionType != OverlayActionTypeEnum.YouTube &&
                        this.SelectedActionType != OverlayActionTypeEnum.TwitchClip)
                    {
                        return Task.FromResult<Result>(new Result(Resources.OverlayActionDurationInvalid));
                    }
                }

                result = this.Position.Validate();
                if (!result.Success)
                {
                    return Task.FromResult<Result>(result);
                }
            }
            else if (this.ShowDamageStreamBoss)
            {
                if (this.SelectedStreamBoss == null)
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }

                if (string.IsNullOrEmpty(this.StreamBossDamageAmount))
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }
            }
            else if (this.ShowAddGoal)
            {
                if (this.SelectedGoal == null)
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }

                if (string.IsNullOrEmpty(this.GoalAmount))
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }
            }
            else if (this.ShowAddPersistTimer)
            {
                if (this.SelectedPersistentTimer == null)
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }

                if (string.IsNullOrEmpty(this.TimeAmount))
                {
                    return Task.FromResult<Result>(new Result(Resources.ValidValueMustBeSpecified));
                }
            }

            return Task.FromResult(new Result());
        }

        protected override async Task OnOpenInternal()
        {
            await base.OnOpenInternal();

            foreach (OverlayWidgetV3Model widget in ChannelSession.Settings.OverlayWidgetsV3)
            {
                if (widget.Type == OverlayItemV3Type.StreamBoss)
                {
                    this.StreamBosses.Add(widget);
                }
                else if (widget.Type == OverlayItemV3Type.Goal)
                {
                    this.Goals.Add(widget);
                }
                else if (widget.Type == OverlayItemV3Type.PersistentTimer)
                {
                    this.PersistentTimers.Add(widget);
                }
            }

            if (this.ShowDamageStreamBoss)
            {
                this.SelectedStreamBoss = this.StreamBosses.FirstOrDefault(w => w.ID.Equals(this.widgetID));
            }
            else if (this.ShowAddGoal)
            {
                this.SelectedGoal = this.Goals.FirstOrDefault(w => w.ID.Equals(this.widgetID));
            }
            else if (this.ShowAddPersistTimer)
            {
                this.SelectedPersistentTimer = this.PersistentTimers.FirstOrDefault(w => w.ID.Equals(this.widgetID));
            }
        }

        protected override Task<ActionModelBase> GetActionInternal()
        {
            if (this.ShowItem)
            {
                OverlayItemV3ModelBase item = this.Item.GetItem();
                if (item != null)
                {
                    item.OverlayEndpointID = this.SelectedOverlayEndpoint.ID;
                    item.HTML = this.HTML;
                    item.CSS = this.CSS;
                    item.Javascript = this.Javascript;
                    item.Position = this.Position.GetPosition();
                    return Task.FromResult<ActionModelBase>(new OverlayActionModel(item, this.Duration, this.EntranceAnimation.GetAnimation(), this.ExitAnimation.GetAnimation()));
                }
            }
            else if (this.ShowDamageStreamBoss)
            {
                return Task.FromResult<ActionModelBase>(new OverlayActionModel((OverlayStreamBossV3Model)this.SelectedStreamBoss.Item, this.StreamBossDamageAmount, this.StreamBossForceDamage));
            }
            else if (this.ShowAddGoal)
            {
                return Task.FromResult<ActionModelBase>(new OverlayActionModel((OverlayGoalV3Model)this.SelectedGoal.Item, this.GoalAmount));
            }
            else if (this.ShowAddPersistTimer)
            {
                return Task.FromResult<ActionModelBase>(new OverlayActionModel((OverlayPersistentTimerV3Model)this.SelectedPersistentTimer.Item, this.TimeAmount));
            }
            return Task.FromResult<ActionModelBase>(null);
        }
    }
}
