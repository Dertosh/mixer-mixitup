﻿using MixItUp.Base.ViewModel.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MixItUp.Base.ViewModel.Requirement
{
    [Obsolete]
    [DataContract]
    public class ThresholdRequirementViewModel
    {
        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public int TimeSpan { get; set; }

        [JsonIgnore]
        private Dictionary<UserViewModel, DateTime> performs = new Dictionary<UserViewModel, DateTime>();

        public ThresholdRequirementViewModel() { }

        public ThresholdRequirementViewModel(int amount, int timeSpan)
        {
            this.Amount = amount;
            this.TimeSpan = timeSpan;
        }
    }
}
