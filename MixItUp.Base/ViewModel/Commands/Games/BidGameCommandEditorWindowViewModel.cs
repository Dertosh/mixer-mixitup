﻿using MixItUp.Base.Model.Commands;
using MixItUp.Base.Model.Commands.Games;
using MixItUp.Base.Model.Currency;
using MixItUp.Base.Model.User;
using MixItUp.Base.Util;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixItUp.Base.ViewModel.Games
{
    public class BidGameCommandEditorWindowViewModel : GameCommandEditorWindowViewModelBase
    {
        public IEnumerable<UserRoleEnum> StarterRoles { get { return EnumHelper.GetEnumList<UserRoleEnum>(); } }

        public UserRoleEnum SelectedStarterRole
        {
            get { return this.selectedStarterRole; }
            set
            {
                this.selectedStarterRole = value;
                this.NotifyPropertyChanged();
            }
        }
        private UserRoleEnum selectedStarterRole;

        public int InitialAmount
        {
            get { return this.initialAmount; }
            set
            {
                this.initialAmount = value;
                this.NotifyPropertyChanged();
            }
        }
        private int initialAmount;

        public int TimeLimit
        {
            get { return this.timeLimit; }
            set
            {
                this.timeLimit = value;
                this.NotifyPropertyChanged();
            }
        }
        private int timeLimit;

        public CustomCommandModel StartedCommand
        {
            get { return this.startedCommand; }
            set
            {
                this.startedCommand = value;
                this.NotifyPropertyChanged();
            }
        }
        private CustomCommandModel startedCommand;

        public CustomCommandModel NewTopBidderCommand
        {
            get { return this.newTopBidderCommand; }
            set
            {
                this.newTopBidderCommand = value;
                this.NotifyPropertyChanged();
            }
        }
        private CustomCommandModel newTopBidderCommand;

        public CustomCommandModel NotEnoughPlayersCommand
        {
            get { return this.notEnoughPlayersCommand; }
            set
            {
                this.notEnoughPlayersCommand = value;
                this.NotifyPropertyChanged();
            }
        }
        private CustomCommandModel notEnoughPlayersCommand;

        public CustomCommandModel GameCompleteCommand
        {
            get { return this.gameCompleteCommand; }
            set
            {
                this.gameCompleteCommand = value;
                this.NotifyPropertyChanged();
            }
        }
        private CustomCommandModel gameCompleteCommand;

        public BidGameCommandEditorWindowViewModel(BidGameCommandModel command)
            : base(command)
        {
            this.SelectedStarterRole = command.StarterRole;
            this.InitialAmount = command.InitialAmount;
            this.TimeLimit = command.TimeLimit;
            this.StartedCommand = command.StartedCommand;
            this.NewTopBidderCommand = command.NewTopBidderCommand;
            this.NotEnoughPlayersCommand = command.NotEnoughPlayersCommand;
            this.GameCompleteCommand = command.GameCompleteCommand;
        }

        public BidGameCommandEditorWindowViewModel(CurrencyModel currency)
            : base(currency)
        {
            this.SelectedStarterRole = UserRoleEnum.Mod;
            this.InitialAmount = 100;
            this.TimeLimit = 60;
            this.StartedCommand = this.CreateBasicChatCommand(string.Format(MixItUp.Base.Resources.GameCommandBidStartedExample, currency.Name));
            this.NewTopBidderCommand = this.CreateBasicChatCommand(string.Format(MixItUp.Base.Resources.GameCommandBidNewTopBidderExample, currency.Name));
            this.NotEnoughPlayersCommand = this.CreateBasicChatCommand(MixItUp.Base.Resources.GameCommandNotEnoughPlayersExample);
            this.GameCompleteCommand = this.CreateBasicChatCommand(string.Format(MixItUp.Base.Resources.GameCommandBidGameCompleteExample, currency.Name));
        }

        public override Task<CommandModelBase> GetCommand()
        {
            return Task.FromResult<CommandModelBase>(new BidGameCommandModel(this.Name, this.GetChatTriggers(), this.SelectedStarterRole, this.InitialAmount, this.TimeLimit, this.StartedCommand, this.NewTopBidderCommand,
                this.NotEnoughPlayersCommand, this.GameCompleteCommand));
        }

        public override async Task<Result> Validate()
        {
            Result result = await base.Validate();
            if (!result.Success)
            {
                return result;
            }

            if (this.InitialAmount < 0)
            {
                return new Result(MixItUp.Base.Resources.GameCommandBidInitialAmountMustBePositive);
            }

            if (this.TimeLimit < 0)
            {
                return new Result(MixItUp.Base.Resources.GameCommandTimeLimitMustBePositive);
            }

            return new Result();
        }
    }
}