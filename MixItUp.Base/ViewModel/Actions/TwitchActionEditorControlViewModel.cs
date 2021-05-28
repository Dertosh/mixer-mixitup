﻿using MixItUp.Base.Model.Actions;
using MixItUp.Base.Util;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.ChannelPoints;

namespace MixItUp.Base.ViewModel.Actions
{
    public class TwitchActionEditorControlViewModel : ActionEditorControlViewModelBase
    {
        public override ActionTypeEnum Type { get { return ActionTypeEnum.Twitch; } }

        public IEnumerable<TwitchActionType> ActionTypes { get { return EnumHelper.GetEnumList<TwitchActionType>(); } }

        public TwitchActionType SelectedActionType
        {
            get { return this.selectedActionType; }
            set
            {
                this.selectedActionType = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("ShowUsernameGrid");
                this.NotifyPropertyChanged("ShowAdGrid");
                this.NotifyPropertyChanged("ShowClipsGrid");
                this.NotifyPropertyChanged("ShowStreamMarkerGrid");
                this.NotifyPropertyChanged("ShowUpdateChannelPointRewardGrid");
                this.NotifyPropertyChanged("ShowPollGrid");
                this.NotifyPropertyChanged("ShowPredictionGrid");
            }
        }
        private TwitchActionType selectedActionType;

        public bool ShowInfoInChat
        {
            get { return this.showInfoInChat; }
            set
            {
                this.showInfoInChat = value;
                this.NotifyPropertyChanged();
            }
        }
        private bool showInfoInChat;

        public bool ShowUsernameGrid
        {
            get
            {
                return this.SelectedActionType == TwitchActionType.Host || this.SelectedActionType == TwitchActionType.Raid ||
                    this.SelectedActionType == TwitchActionType.VIPUser || this.SelectedActionType == TwitchActionType.UnVIPUser;
            }
        }

        public string Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                this.NotifyPropertyChanged();
            }
        }
        private string username;

        public bool ShowAdGrid { get { return this.SelectedActionType == TwitchActionType.RunAd; } }

        public IEnumerable<int> AdLengths { get { return TwitchActionModel.SupportedAdLengths; } }

        public int SelectedAdLength
        {
            get { return this.selectedAdLength; }
            set
            {
                this.selectedAdLength = value;
                this.NotifyPropertyChanged();
            }
        }
        private int selectedAdLength = TwitchActionModel.SupportedAdLengths.FirstOrDefault();

        public bool ShowClipsGrid { get { return this.SelectedActionType == TwitchActionType.Clip; } }

        public bool ClipIncludeDelay
        {
            get { return this.clipIncludeDelay; }
            set
            {
                this.clipIncludeDelay = value;
                this.NotifyPropertyChanged();
            }
        }
        private bool clipIncludeDelay;

        public bool ShowStreamMarkerGrid { get { return this.SelectedActionType == TwitchActionType.StreamMarker; } }

        public string StreamMarkerDescription
        {
            get { return this.streamMarkerDescription; }
            set
            {
                this.streamMarkerDescription = value;
                this.NotifyPropertyChanged();
            }
        }
        private string streamMarkerDescription;

        public bool ShowUpdateChannelPointRewardGrid { get { return this.SelectedActionType == TwitchActionType.UpdateChannelPointReward; } }

        public ObservableCollection<CustomChannelPointRewardModel> ChannelPointRewards { get; set; } = new ObservableCollection<CustomChannelPointRewardModel>();

        public CustomChannelPointRewardModel ChannelPointReward
        {
            get { return this.channelPointReward; }
            set
            {
                this.channelPointReward = value;
                this.NotifyPropertyChanged();

                if (this.existingChannelPointRewardID == Guid.Empty)
                {
                    this.ChannelPointRewardState = this.ChannelPointReward.is_enabled;
                    this.ChannelPointRewardCost = this.ChannelPointReward.cost.ToString();
                    this.ChannelPointRewardMaxPerStream = this.ChannelPointReward.max_per_stream_setting.max_per_stream.ToString();
                    this.ChannelPointRewardMaxPerUser = this.ChannelPointReward.max_per_user_per_stream_setting.max_per_user_per_stream.ToString();
                    this.ChannelPointRewardGlobalCooldown = (this.ChannelPointReward.global_cooldown_setting.global_cooldown_seconds / 60).ToString();
                    this.ChannelPointRewardUpdateCooldownsAndLimits = (this.ChannelPointReward.max_per_stream_setting.is_enabled || this.ChannelPointReward.max_per_user_per_stream_setting.is_enabled || this.ChannelPointReward.global_cooldown_setting.is_enabled);
                }
                this.existingChannelPointRewardID = Guid.Empty;
            }
        }
        private CustomChannelPointRewardModel channelPointReward;

        public bool ChannelPointRewardState
        {
            get { return this.channelPointRewardState; }
            set
            {
                this.channelPointRewardState = value;
                this.NotifyPropertyChanged();
            }
        }
        private bool channelPointRewardState;

        public string ChannelPointRewardCost
        {
            get { return (this.channelPointRewardCost > 0) ? this.channelPointRewardCost.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.channelPointRewardCost = cost;
                }
                else
                {
                    this.channelPointRewardCost = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int channelPointRewardCost = 0;

        public bool ChannelPointRewardUpdateCooldownsAndLimits
        {
            get { return this.channelPointRewardUpdateCooldownsAndLimits; }
            set
            {
                this.channelPointRewardUpdateCooldownsAndLimits = value;
                this.NotifyPropertyChanged();

                if (!this.ChannelPointRewardUpdateCooldownsAndLimits)
                {
                    this.ChannelPointRewardMaxPerStream = string.Empty;
                    this.ChannelPointRewardMaxPerUser = string.Empty;
                    this.ChannelPointRewardGlobalCooldown = string.Empty;
                }
            }
        }
        private bool channelPointRewardUpdateCooldownsAndLimits = false;

        public string ChannelPointRewardMaxPerStream
        {
            get { return (this.channelPointRewardMaxPerStream > 0) ? this.channelPointRewardMaxPerStream.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.channelPointRewardMaxPerStream = cost;
                }
                else
                {
                    this.channelPointRewardMaxPerStream = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int channelPointRewardMaxPerStream = 0;

        public string ChannelPointRewardMaxPerUser
        {
            get { return (this.channelPointRewardMaxPerUser > 0) ? this.channelPointRewardMaxPerUser.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.channelPointRewardMaxPerUser = cost;
                }
                else
                {
                    this.channelPointRewardMaxPerUser = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int channelPointRewardMaxPerUser = 0;

        public string ChannelPointRewardGlobalCooldown
        {
            get { return (this.channelPointRewardGlobalCooldown > 0) ? this.channelPointRewardGlobalCooldown.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.channelPointRewardGlobalCooldown = cost;
                }
                else
                {
                    this.channelPointRewardGlobalCooldown = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int channelPointRewardGlobalCooldown = 0;

        private Guid existingChannelPointRewardID;

        public bool ShowPollGrid { get { return this.SelectedActionType == TwitchActionType.CreatePoll; } }

        public string PollTitle
        {
            get { return this.pollTitle; }
            set
            {
                this.pollTitle = value;
                this.NotifyPropertyChanged();
            }
        }
        private string pollTitle;

        public int PollDurationSeconds
        {
            get { return this.pollDurationSeconds; }
            set
            {
                this.pollDurationSeconds = value;
                this.NotifyPropertyChanged();
            }
        }
        private int pollDurationSeconds = 60;

        public string PollChannelPointsCost
        {
            get { return (this.pollChannelPointsCost > 0) ? this.pollChannelPointsCost.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.pollChannelPointsCost = cost;
                }
                else
                {
                    this.pollChannelPointsCost = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int pollChannelPointsCost = 0;

        public string PollBitsCost
        {
            get { return (this.pollBitsCost > 0) ? this.pollBitsCost.ToString() : string.Empty; }
            set
            {
                if (int.TryParse(value, out int cost) && cost > 0)
                {
                    this.pollBitsCost = cost;
                }
                else
                {
                    this.pollBitsCost = 0;
                }
                this.NotifyPropertyChanged();
            }
        }
        private int pollBitsCost = 0;

        public string PollChoice1
        {
            get { return this.pollChoice1; }
            set
            {
                this.pollChoice1 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string pollChoice1;

        public string PollChoice2
        {
            get { return this.pollChoice2; }
            set
            {
                this.pollChoice2 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string pollChoice2;

        public string PollChoice3
        {
            get { return this.pollChoice3; }
            set
            {
                this.pollChoice3 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string pollChoice3;

        public string PollChoice4
        {
            get { return this.pollChoice4; }
            set
            {
                this.pollChoice4 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string pollChoice4;

        public bool ShowPredictionGrid { get { return this.SelectedActionType == TwitchActionType.CreatePrediction; } }

        public string PredictionTitle
        {
            get { return this.predictionTitle; }
            set
            {
                this.predictionTitle = value;
                this.NotifyPropertyChanged();
            }
        }
        private string predictionTitle;

        public int PredictionDurationSeconds
        {
            get { return this.predictionDurationSeconds; }
            set
            {
                this.predictionDurationSeconds = value;
                this.NotifyPropertyChanged();
            }
        }
        private int predictionDurationSeconds = 60;

        public string PredictionOutcome1
        {
            get { return this.predictionOutcome1; }
            set
            {
                this.predictionOutcome1 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string predictionOutcome1;

        public string PredictionOutcome2
        {
            get { return this.predictionOutcome2; }
            set
            {
                this.predictionOutcome2 = value;
                this.NotifyPropertyChanged();
            }
        }
        private string predictionOutcome2;

        public TwitchActionEditorControlViewModel(TwitchActionModel action)
            : base(action)
        {
            this.SelectedActionType = action.ActionType;
            if (this.ShowUsernameGrid)
            {
                this.Username = action.Username;
            }
            else if (this.ShowAdGrid)
            {
                this.SelectedAdLength = action.AdLength;
            }
            else if (this.ShowClipsGrid)
            {
                this.ClipIncludeDelay = action.ClipIncludeDelay;
                this.ShowInfoInChat = action.ShowInfoInChat;
            }
            else if (this.ShowStreamMarkerGrid)
            {
                this.StreamMarkerDescription = action.StreamMarkerDescription;
                this.ShowInfoInChat = action.ShowInfoInChat;
            }
            else if (this.ShowUpdateChannelPointRewardGrid)
            {
                this.existingChannelPointRewardID = action.ChannelPointRewardID;
                this.ChannelPointRewardState = action.ChannelPointRewardState;
                this.channelPointRewardCost = action.ChannelPointRewardCost;
                this.ChannelPointRewardUpdateCooldownsAndLimits = action.ChannelPointRewardUpdateCooldownsAndLimits;
                this.channelPointRewardMaxPerStream = action.ChannelPointRewardMaxPerStream;
                this.channelPointRewardMaxPerUser = action.ChannelPointRewardMaxPerUser;
                this.channelPointRewardGlobalCooldown = action.ChannelPointRewardGlobalCooldown;
            }
            else if (this.ShowPollGrid)
            {
                this.PollTitle = action.PollTitle;
                this.PollDurationSeconds = action.PollDurationSeconds;
                this.pollChannelPointsCost = action.PollChannelPointsCost;
                this.pollBitsCost = action.PollBitsCost;
                if (action.PollChoices.Count > 0)
                {
                    this.PollChoice1 = action.PollChoices[0];
                }
                if (action.PollChoices.Count > 1)
                {
                    this.PollChoice2 = action.PollChoices[1];
                }
                if (action.PollChoices.Count > 2)
                {
                    this.PollChoice3 = action.PollChoices[2];
                }
                if (action.PollChoices.Count > 3)
                {
                    this.PollChoice4 = action.PollChoices[3];
                }
            }
            else if (this.ShowPredictionGrid)
            {
                this.PredictionTitle = action.PredictionTitle;
                this.PredictionDurationSeconds = action.PredictionDurationSeconds;
                this.PredictionOutcome1 = action.PredictionOutcomes[0];
                this.PredictionOutcome2 = action.PredictionOutcomes[1];
            }
        }

        public TwitchActionEditorControlViewModel() : base() { }

        public override Task<Result> Validate()
        {
            if (this.ShowStreamMarkerGrid)
            {
                if (!string.IsNullOrEmpty(this.StreamMarkerDescription) && this.StreamMarkerDescription.Length > TwitchActionModel.StreamMarkerMaxDescriptionLength)
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionStreamMarkerDescriptionMustBe140CharactersOrLess));
                }
            }
            else if (this.ShowUpdateChannelPointRewardGrid)
            {
                if (this.ChannelPointReward == null)
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionChannelPointRewardMissing));
                }
            }
            else if (this.ShowPollGrid)
            {
                if (string.IsNullOrEmpty(this.PollTitle))
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePollMissingTitle));
                }

                if (this.PollDurationSeconds <= 0)
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePollInvalidDuration));
                }

                if (string.IsNullOrEmpty(this.PollChoice1) || string.IsNullOrEmpty(this.PollChoice2))
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePollTwoOrMoreChoices));
                }
            }
            else if (this.ShowPredictionGrid)
            {
                if (string.IsNullOrEmpty(this.PredictionTitle))
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePredictionMissingTitle));
                }

                if (this.PredictionDurationSeconds <= 0)
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePredictionInvalidDuration));
                }

                if (string.IsNullOrEmpty(this.PredictionOutcome1) || string.IsNullOrEmpty(this.PredictionOutcome2))
                {
                    return Task.FromResult<Result>(new Result(MixItUp.Base.Resources.TwitchActionCreatePredictionTwoChoices));
                }
            }
            return Task.FromResult(new Result());
        }

        protected override async Task OnLoadedInternal()
        {
            foreach (CustomChannelPointRewardModel channelPoint in (await ChannelSession.TwitchUserConnection.GetCustomChannelPointRewards(ChannelSession.TwitchUserNewAPI)).OrderBy(c => c.title))
            {
                this.ChannelPointRewards.Add(channelPoint);
            }

            if (this.ShowUpdateChannelPointRewardGrid)
            {
                this.ChannelPointReward = this.ChannelPointRewards.FirstOrDefault(c => c.id.Equals(this.existingChannelPointRewardID));
            }

            await base.OnLoadedInternal();
        }

        protected override Task<ActionModelBase> GetActionInternal()
        {
            if (this.ShowUsernameGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreateUserAction(this.SelectedActionType, this.Username));
            }
            else if (this.ShowAdGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreateAdAction(this.SelectedAdLength));
            }
            else if (this.ShowClipsGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreateClipAction(this.ClipIncludeDelay, this.ShowInfoInChat));
            }
            else if (this.ShowStreamMarkerGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreateStreamMarkerAction(this.StreamMarkerDescription, this.ShowInfoInChat));
            }
            else if (this.ShowUpdateChannelPointRewardGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreateUpdateChannelPointReward(this.ChannelPointReward.id, this.ChannelPointRewardState, this.channelPointRewardCost,
                    this.ChannelPointRewardUpdateCooldownsAndLimits, this.channelPointRewardMaxPerStream, this.channelPointRewardMaxPerUser, this.channelPointRewardGlobalCooldown));
            }
            else if (this.ShowPollGrid)
            {
                List<string> choices = new List<string>();
                if (!string.IsNullOrEmpty(this.PollChoice1)) { choices.Add(this.PollChoice1); }
                if (!string.IsNullOrEmpty(this.PollChoice2)) { choices.Add(this.PollChoice2); }
                if (!string.IsNullOrEmpty(this.PollChoice3)) { choices.Add(this.PollChoice3); }
                if (!string.IsNullOrEmpty(this.PollChoice4)) { choices.Add(this.PollChoice4); }
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreatePollAction(this.PollTitle, this.PollDurationSeconds, this.pollChannelPointsCost, this.pollBitsCost, choices));
            }
            else if (this.ShowPredictionGrid)
            {
                return Task.FromResult<ActionModelBase>(TwitchActionModel.CreatePredictionAction(this.PredictionTitle, this.PredictionDurationSeconds, new List<string>() { this.PredictionOutcome1, this.PredictionOutcome2 }));
            }
            return Task.FromResult<ActionModelBase>(null);
        }
    }
}
