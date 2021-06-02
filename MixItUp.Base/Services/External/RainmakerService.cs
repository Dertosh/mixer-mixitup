﻿using MixItUp.Base.Model.User;
using MixItUp.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MixItUp.Base.Services.External
{
    [DataContract]
    public class RainmakerChannel
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
    }

    [DataContract]
    public class RainmakerDonation
    {
        [JsonProperty("distributionId")]
        public string DistributionId { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        public RainmakerDonation() { }

        public UserDonationModel ToGenericDonation()
        {
            return new UserDonationModel()
            {
                Source = UserDonationSourceEnum.Rainmaker,

                ID = this.DistributionId.ToString(),
                Username = this.Name,
                Message = this.Message,

                Amount = Math.Round(this.Amount, 2),

                DateTime = this.CreatedAt,
            };
        }
    }

    public class RainmakerService : OAuthExternalServiceBase
    {
        private const string BaseAddress = "https://rainmaker.gg/api/v2/";

        private const string ClientID = "0ff4b414d6ec2296b824cd8a11ff75ff";
        private const string AuthorizationUrl = "https://rainmaker.gg/oauth/authorize?response_type=code&client_id={0}&redirect_uri={1}&scopes=channel:tips:view";
        private const string TokenUrl = "https://rainmaker.gg/api/v2/oauth/authorize";

        private RainmakerChannel channel;

        public bool WebSocketConnected { get; private set; }

        private ISocketIOConnection socket;

        public RainmakerService(ISocketIOConnection socket)
            : base(RainmakerService.BaseAddress)
        {
            this.socket = socket;
        }

        public override string Name { get { return "Rainmaker"; } }

        public override async Task<Result> Connect()
        {
            try
            { 
                string authorizationCode = await this.ConnectViaOAuthRedirect(string.Format(RainmakerService.AuthorizationUrl, RainmakerService.ClientID, OAuthExternalServiceBase.DEFAULT_OAUTH_LOCALHOST_URL));
                if (!string.IsNullOrEmpty(authorizationCode))
                {
                    JObject payload = new JObject();
                    payload["grant_type"] = "authorization_code";
                    payload["client_id"] = RainmakerService.ClientID;
                    payload["code"] = authorizationCode;
                    payload["redirect_uri"] = OAuthExternalServiceBase.DEFAULT_OAUTH_LOCALHOST_URL;

                    this.token = await this.PostAsync<OAuthTokenModel>(RainmakerService.TokenUrl, AdvancedHttpClient.CreateContentFromObject(payload), autoRefreshToken: false);
                    if (this.token != null)
                    {
                        token.authorizationCode = authorizationCode;
                        return await this.InitializeInternal();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ex);
            }
            return new Result(false);
        }

        public override async Task Disconnect()
        {
            if (this.socket != null)
            {
                this.socket.OnDisconnected -= Socket_OnDisconnected;
                await this.socket.Disconnect();
            }
            this.WebSocketConnected = false;

            this.token = null;
        }

        public async Task<RainmakerChannel> GetChannel()
        {
            try
            {
                JArray jarray = await this.GetAsync<JArray>("channels");
                if (jarray != null && jarray.Count > 0)
                {
                    return jarray.First.ToObject<RainmakerChannel>();
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
            return null;
        }

        protected override async Task RefreshOAuthToken()
        {
            if (this.token != null)
            {
                JObject payload = new JObject();
                payload["grant_type"] = "refresh_token";
                payload["client_id"] = RainmakerService.ClientID;
                payload["refresh_token"] = this.token.refreshToken;
                payload["redirect_uri"] = OAuthExternalServiceBase.DEFAULT_OAUTH_LOCALHOST_URL;

                this.token = await this.PostAsync<OAuthTokenModel>(RainmakerService.TokenUrl, AdvancedHttpClient.CreateContentFromObject(payload), autoRefreshToken: false);
            }
        }

        protected override async Task<Result> InitializeInternal()
        {
            this.channel = await this.GetChannel();
            if (this.channel != null)
            {
                if (await this.ConnectSocket())
                {
                    this.TrackServiceTelemetry("Rainmaker");
                    return new Result();
                }
                return new Result(Resources.RainmakerSocketFailed);
            }
            return new Result(Resources.RainmakerUserDataFailed);
        }

        protected override async Task<AdvancedHttpClient> GetHttpClient(bool autoRefreshToken = true)
        {
            AdvancedHttpClient client = await base.GetHttpClient(autoRefreshToken);
            if (this.token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", this.token.accessToken);
            }
            return client;
        }

        private async void Socket_OnDisconnected(object sender, EventArgs e)
        {
            ChannelSession.DisconnectionOccurred("Rainmaker");

            do
            {
                await Task.Delay(5000);
            } while (!await this.ConnectSocket());

            ChannelSession.ReconnectionOccurred("Rainmaker");
        }

        private async Task<bool> ConnectSocket()
        {
            try
            {
                this.WebSocketConnected = false;
                this.socket.OnDisconnected -= Socket_OnDisconnected;
                await this.socket.Disconnect();

                this.socket.Listen("disconnect", (data) =>
                {
                    this.Socket_OnDisconnected(null, new EventArgs());
                });

                this.socket.Listen("error", (data) =>
                {
                    this.Socket_OnDisconnected(null, new EventArgs());
                });

                this.socket.Listen("connect", (data) =>
                {
                    this.socket.Send("subscribe", "{ event: '" + $"channel:{this.channel.ID}:tip" + "' }");

                    this.WebSocketConnected = true;
                });

                this.socket.Listen("authenticate", (data) =>
                {
                    this.socket.Send("subscribe", "{ event: '" + $"channel:{this.channel.ID}:tip" + "' }");

                    this.WebSocketConnected = true;
                });

                this.socket.Listen($"channel:{this.channel.ID}:tip", async (data) =>
                {
                    try
                    {
                        if (data != null)
                        {
                            RainmakerDonation donation = JSONSerializerHelper.DeserializeFromString<RainmakerDonation>(data.ToString());
                            if (donation != null && !string.IsNullOrEmpty(donation.Name) && donation.Amount >= 0)
                            {
                                await EventService.ProcessDonationEvent(EventTypeEnum.RainmakerDonation, donation.ToGenericDonation());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });

                await this.socket.Connect("wss://rainmaker.gg");

                this.socket.Send("authenticate", "{ oauth: '" + this.token.accessToken + "' }");

                for (int i = 0; i < 10 && !this.WebSocketConnected; i++)
                {
                    await Task.Delay(1000);
                }

                if (this.WebSocketConnected)
                {
                    this.socket.OnDisconnected += Socket_OnDisconnected;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return this.WebSocketConnected;
        }
    }
}
