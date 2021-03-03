﻿using MixItUp.Base.Model.Commands;
using MixItUp.Base.Services;
using MixItUp.Base.Services.External;
using StreamingClient.Base.Util;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MixItUp.Base.Model.Actions
{
    public enum TwitterActionTypeEnum
    {
        [Name("Send Tweet")]
        SendTweet,
        [Name("Update Name")]
        UpdateName,
    }

    [DataContract]
    public class TwitterActionModel : ActionModelBase
    {
        public static bool CheckIfTweetContainsTooManyTags(string tweet) { return !string.IsNullOrEmpty(tweet) && tweet.Count(c => c == '@') > 0; }

        public static TwitterActionModel CreateTweetAction(string tweetText, string imagePath = null)
        {
            TwitterActionModel action = new TwitterActionModel(TwitterActionTypeEnum.SendTweet);
            action.TweetText = tweetText;
            action.ImagePath = imagePath;
            return action;
        }

        public static TwitterActionModel CreateUpdateProfileNameAction(string nameUpdate)
        {
            TwitterActionModel action = new TwitterActionModel(TwitterActionTypeEnum.UpdateName);
            action.NameUpdate = nameUpdate;
            return action;
        }

        [DataMember]
        public TwitterActionTypeEnum ActionType { get; set; }

        [DataMember]
        public string TweetText { get; set; }
        [DataMember]
        public string ImagePath { get; set; }

        [DataMember]
        public string NameUpdate { get; set; }

        public TwitterActionModel(TwitterActionTypeEnum actionType)
            : base(ActionTypeEnum.Twitter)
        {
            this.ActionType = actionType;
        }

#pragma warning disable CS0612 // Type or member is obsolete
        internal TwitterActionModel(MixItUp.Base.Actions.TwitterAction action)
            : base(ActionTypeEnum.Twitter)
        {
            this.ActionType = (TwitterActionTypeEnum)(int)action.ActionType;
            this.TweetText = action.TweetText;
            this.ImagePath = action.ImagePath;
            this.NameUpdate = action.NewProfileName;
        }
#pragma warning restore CS0612 // Type or member is obsolete

        private TwitterActionModel() { }

        protected override async Task PerformInternal(CommandParametersModel parameters)
        {
            if (ServiceManager.Get<TwitterService>().IsConnected)
            {
                if (this.ActionType == TwitterActionTypeEnum.SendTweet)
                {
                    string tweet = await this.ReplaceStringWithSpecialModifiers(this.TweetText, parameters);
                    string imagePath = await this.ReplaceStringWithSpecialModifiers(this.ImagePath, parameters);

                    if (!string.IsNullOrEmpty(tweet))
                    {
                        if (TwitterActionModel.CheckIfTweetContainsTooManyTags(tweet))
                        {
                            await ServiceManager.Get<ChatService>().SendMessage("The tweet you specified can not be sent because it contains an @mention", parameters.Platform);
                            return;
                        }

                        if (!await ServiceManager.Get<TwitterService>().SendTweet(tweet, imagePath))
                        {
                            await ServiceManager.Get<ChatService>().SendMessage("The tweet you specified could not be sent. Please ensure your Twitter account is correctly authenticated and you have not sent a tweet in the last 5 minutes", parameters.Platform);
                        }
                    }
                }
                else if (this.ActionType == TwitterActionTypeEnum.UpdateName)
                {
                    await ServiceManager.Get<TwitterService>().UpdateName(this.NameUpdate);
                }
            }
        }
    }
}
