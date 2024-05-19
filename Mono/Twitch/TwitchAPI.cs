using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AstroRaider2.Utility.NodeTree;
using Godot;
using Godot.Collections;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Chat.Emotes.GetChannelEmotes;
using TwitchLib.EventSub.Core.Models.Predictions;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchOverlay.Mono;

[GlobalClass]
public partial class TwitchAPI : Node
{
	public TwitchLib.Api.TwitchAPI twitchApi;
	public EventSubWebsocketClient _websocketClient;

	private string _channelId;

	private System.Collections.Generic.Dictionary<string, string> _conditionTemplate;

	private NodeRef<GlobalSceneSignals> _globalSceneSignals;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_globalSceneSignals = new NodeRef<GlobalSceneSignals>(GetTree().Root, "@GlobalSceneSignals");
		
		Dictionary _jsonAuthFile;
		string JsonFileToText = FileAccess.GetFileAsString("res://Auth.json");
		_jsonAuthFile = (Dictionary)Json.ParseString(JsonFileToText);
		
		// Get Channel ID from Auth.Json
		_channelId = (string)_jsonAuthFile["Channel_ID"];

		// Make twitchApi and _websocketClient real
		twitchApi = new TwitchLib.Api.TwitchAPI();
		_websocketClient = new EventSubWebsocketClient();
		
		
		// Set settings
		twitchApi.Settings.Secret = (string)_jsonAuthFile["Client_Secret"];
		twitchApi.Settings.AccessToken = (string)_jsonAuthFile["Access_Token"];
		twitchApi.Settings.ClientId = (string)_jsonAuthFile["Client_ID"];
		twitchApi.Settings.Scopes = new List<AuthScopes>()
		{
			AuthScopes.Bits_Read,
			AuthScopes.Channel_Bot,
			AuthScopes.Channel_Moderate,
			AuthScopes.Chat_Edit,
			AuthScopes.Chat_Read,
			AuthScopes.User_Bot,
			AuthScopes.Whisper_Edit,
			AuthScopes.Whisper_Read,
			AuthScopes.Channel_Edit_Commercial,
			AuthScopes.Channel_Read_Goals,
			AuthScopes.Channel_Read_Polls,
			AuthScopes.Channel_Read_Predictions,
			AuthScopes.Channel_Read_Redemptions,
			AuthScopes.Channel_Read_Subscriptions,
			AuthScopes.Channel_Read_Hype_Train,
			AuthScopes.Channel_Read_VIPs,
			AuthScopes.Moderator_Manage_Announcements,
			AuthScopes.User_Manage_Whispers,
			AuthScopes.User_Read_Chat,
			AuthScopes.User_Write_Chat,
			AuthScopes.User_Read_Subscriptions,
			AuthScopes.Channel_Read_Hype_Train,
			AuthScopes.Moderator_Manage_Chat_Messages,
			AuthScopes.Moderator_Read_Chat_Settings,
			AuthScopes.Channel_Manage_Ads,
			AuthScopes.Channel_Read_Ads,
			AuthScopes.Moderator_Read_Followers,
			AuthScopes.Moderator_Manage_Banned_Users
		};
		// Refresh the auth key (please fucking work)
		twitchApi.Auth.RefreshAuthTokenAsync((string)_jsonAuthFile["Access_Token"], (string)_jsonAuthFile["Client_Secret"],
			(string)_jsonAuthFile["Client_ID"]);
		
		// Set up condition and transport dictionary for subscriptions.
		_conditionTemplate = new System.Collections.Generic.Dictionary<string, string>()
		{
			{ "to_broadcaster_user_id", _channelId },
			{ "broadcaster_user_id", _channelId },
			{ "user_id", _channelId },
			{ "moderator_user_id", _channelId }
		};
		
		// Setup error handling
		_websocketClient.ErrorOccurred += (sender, args) =>
		{
			GD.Print(args.Exception.Message);
			return Task.CompletedTask;
		};
		
		// Connect Signals from Websocket to GlobalSceneSignals
		// NOTE: Chat messages are handled by ChatHandler
		_websocketClient.WebsocketConnected += WebsocketClientOnWebsocketConnected;
		_websocketClient.ChannelBan += WebsocketClientOnChannelBan;
		_websocketClient.ChannelCheer += WebsocketClientOnChannelCheer;
		_websocketClient.ChannelFollow += WebsocketClientOnChannelFollow;
		_websocketClient.ChannelRaid += WebsocketClientOnChannelRaid;
		_websocketClient.ChannelSubscribe += WebsocketClientOnChannelSubscribe;
		_websocketClient.ChannelGoalBegin += WebsocketClientOnChannelGoalBegin;
		_websocketClient.ChannelGoalEnd += WebsocketClientOnChannelGoalEnd;
		_websocketClient.ChannelGoalProgress += WebsocketClientOnChannelGoalProgress;
		_websocketClient.ChannelPollBegin += WebsocketClientOnChannelPollBegin;
		_websocketClient.ChannelPollEnd += WebsocketClientOnChannelPollEnd;
		_websocketClient.ChannelPollProgress += WebsocketClientOnChannelPollProgress;
		_websocketClient.ChannelPredictionBegin += WebsocketClientOnChannelPredictionBegin;
		_websocketClient.ChannelPredictionEnd += WebsocketClientOnChannelPredictionEnd;
		_websocketClient.ChannelPredictionLock += WebsocketClientOnChannelPredictionLock;
		_websocketClient.ChannelPredictionProgress += WebsocketClientOnChannelPredictionProgress;
		_websocketClient.ChannelSubscriptionGift += WebsocketClientOnChannelSubscriptionGift;
		_websocketClient.ChannelSubscriptionMessage += WebsocketClientOnChannelSubscriptionMessage;
		_websocketClient.ChannelHypeTrainBegin += WebsocketClientOnChannelHypeTrainBegin;
		_websocketClient.ChannelHypeTrainEnd += WebsocketClientOnChannelHypeTrainEnd;
		_websocketClient.ChannelHypeTrainProgress += WebsocketClientOnChannelHypeTrainProgress;
		_websocketClient.ChannelPointsCustomRewardRedemptionAdd += WebsocketClientOnChannelPointsCustomRewardRedemptionAdd;
		
		_websocketClient.ConnectAsync();
	}

	// Helper Functions
	private static Array<Dictionary> _TransformPredictionOutcomes(PredictionOutcomes[] outcomes)
	{
		Array<Dictionary> translatedOutcomes = new Array<Dictionary>();
		foreach (var nextOutcome in outcomes)
		{
			Dictionary nextDictionary = new Dictionary();
			nextDictionary.Add(nameof(nextOutcome.Title), nextOutcome.Title);
			nextDictionary.Add(nameof(nextOutcome.Color), nextOutcome.Color);
			nextDictionary.Add(nameof(nextOutcome.Id), nextOutcome.Id);
			if (nextOutcome.ChannelPoints != null)
				nextDictionary.Add(nameof(nextOutcome.ChannelPoints), (Variant)nextOutcome.ChannelPoints);
			else
			{
				nextDictionary.Add(nameof(nextOutcome.ChannelPoints), 0);
			}

			translatedOutcomes.Add(nextDictionary);
		}

		return translatedOutcomes;
	}

	public void SendChatMessage(String message, String replyMessageId = "")
	{
		twitchApi.Helix.Chat.SendChatMessage(_channelId, _channelId, message, replyMessageId);
	}
	
	// Regen token function
	public void GenerateNewToken()
	{
		string authUrl = twitchApi.Auth.GetAuthorizationCodeUrl("http://localhost:3000", twitchApi.Settings.Scopes, true, "bleebeldorp4i37962",
			twitchApi.Settings.ClientId);
		DisplayServer.ClipboardSet(authUrl);
	}
	
	
	// Connection events
	private Task WebsocketClientOnWebsocketConnected(object sender, WebsocketConnectedArgs args)
	{
		// Array of events to connect
		var eventSubscriptions = new (string Type, string Version)[]
		{
			("channel.chat.message","1"),
			("channel.follow","2"),
			("channel.chat.message_delete","1"),
			("channel.subscribe","1"),
			("channel.subscription.gift", "1"),
			("channel.subscription.message","1"),
			("channel.cheer","1"),
			("channel.raid","1"),
			("channel.ban","1"),
			("channel.channel_points_automatic_reward_redemption.add","1"),
			("channel.channel_points_custom_reward_redemption.add","1"),
			("channel.poll.begin","1"),
			("channel.poll.progress","1"),
			("channel.poll.end","1"),
			("channel.prediction.begin","1"),
			("channel.prediction.progress","1"),
			("channel.prediction.lock","1"),
			("channel.prediction.end","1"),
			("channel.goal.begin","1"),
			("channel.goal.progress","1"),
			("channel.goal.end","1"),
			("channel.hype_train.begin","1"),
			("channel.hype_train.progress","1"),
			("channel.hype_train.end","1"),
			("user.whisper.message","1")
		};
		
		GD.Print("TwitchApi.cs: Connected websocket server successfully!");

		foreach (var _event in eventSubscriptions)
		{
			GD.Print("TwitchAPI.cs: Subscribing to ",_event.Type);
			twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(_event.Type, _event.Version,_conditionTemplate,EventSubTransportMethod.Websocket,_websocketClient.SessionId);
		}
		
		GD.Print("Sub total: ", twitchApi.Helix.EventSub.GetEventSubSubscriptionsAsync().Result.Total);

		twitchApi.Helix.Chat.SendChatMessage(_channelId, _channelId, "[BOT]: Connected to Twitch Chat!");
		
		return Task.CompletedTask;
	}

	
	// Twitch Events
	private Task WebsocketClientOnChannelPointsCustomRewardRedemptionAdd(object sender, ChannelPointsCustomRewardRedemptionArgs args)
	{
		GD.Print(args.Notification.Payload.Event.Reward.Title, " has been redeemed!");
		
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChannelPoint,
			args.Notification.Payload.Event.UserName, args.Notification.Payload.Event.Reward.Title,
			args.Notification.Payload.Event.UserInput)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelHypeTrainProgress(object sender, ChannelHypeTrainProgressArgs args)
	{
		GD.Print("TwitchAPI.cs: Hype train not implemented");
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelHypeTrainEnd(object sender, ChannelHypeTrainEndArgs args)
	{
		GD.Print("TwitchAPI.cs: Hype train not implemented");
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelHypeTrainBegin(object sender, ChannelHypeTrainBeginArgs args)
	{
		GD.Print("TwitchAPI.cs: Hype train not implemented");
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelSubscriptionMessage(object sender, ChannelSubscriptionMessageArgs args)
	{
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.MessageSubscription,
			args.Notification.Payload.Event.UserName, args.Notification.Payload.Event.Tier,
			args.Notification.Payload.Event.Message.Text, args.Notification.Payload.Event.CumulativeMonths,
			args.Notification.Payload.Event.DurationMonths)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelSubscriptionGift(object sender, ChannelSubscriptionGiftArgs args)
	{
		string username;
	
		if (args.Notification.Payload.Event.IsAnonymous)
		{
			username = "Anonymous";
		}
		else
		{
			username = args.Notification.Payload.Event.UserName;
		}
		
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.GiftSubscription,
			username, args.Notification.Payload.Event.Tier,
			args.Notification.Payload.Event.Total)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelPredictionProgress(object sender, ChannelPredictionProgressArgs args)
	{
		Array<Dictionary> stuffToSend = _TransformPredictionOutcomes(args.Notification.Payload.Event.Outcomes);

		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChannelPrediction,
			args.Notification.Payload.Event.Title,
			stuffToSend,
			"progress",
			0)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelPredictionLock(object sender, ChannelPredictionLockArgs args)
	{
		Array<Dictionary> stuffToSend = _TransformPredictionOutcomes(args.Notification.Payload.Event.Outcomes);

		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChannelPrediction,
			args.Notification.Payload.Event.Title,
			stuffToSend,
			"lock",
			0)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelPredictionEnd(object sender, ChannelPredictionEndArgs args)
	{
		Array<Dictionary> stuffToSend = _TransformPredictionOutcomes(args.Notification.Payload.Event.Outcomes);

		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChannelPrediction,
			args.Notification.Payload.Event.Title,
			stuffToSend,
			"end",
			0)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelPredictionBegin(object sender, ChannelPredictionBeginArgs args)
	{
		GD.Print("TwitchAPI.cs: Prediction has begun!");
		Array<Dictionary> stuffToSend = _TransformPredictionOutcomes(args.Notification.Payload.Event.Outcomes);
		
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChannelPrediction,
			args.Notification.Payload.Event.Title,
			stuffToSend,
			"begin",
			0)).CallDeferred();
		
		return Task.CompletedTask;
	}
	

	private Task WebsocketClientOnChannelPollProgress(object sender, ChannelPollProgressArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelPollEnd(object sender, ChannelPollEndArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelPollBegin(object sender, ChannelPollBeginArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelGoalProgress(object sender, ChannelGoalProgressArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelGoalEnd(object sender, ChannelGoalEndArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelGoalBegin(object sender, ChannelGoalBeginArgs args)
	{
		throw new NotImplementedException();
	}

	private Task WebsocketClientOnChannelSubscribe(object sender, ChannelSubscribeArgs args)
	{
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.Subscription,
			args.Notification.Payload.Event.UserName, args.Notification.Payload.Event.Tier,
			args.Notification.Payload.Event.IsGift)).CallDeferred();
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelRaid(object sender, ChannelRaidArgs args)
	{
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.Raid,
			args.Notification.Payload.Event.FromBroadcasterUserName, args.Notification.Payload.Event.Viewers)).CallDeferred();
		
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelFollow(object sender, ChannelFollowArgs args)
	{
		Callable.From(() =>
			_globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.Follow,
				args.Notification.Payload.Event.UserName)).CallDeferred();
		GD.Print("Received a follow!");

		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelCheer(object sender, ChannelCheerArgs args)
	{
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.Cheer,
			args.Notification.Payload.Event.UserName, args.Notification.Payload.Event.Message,
			args.Notification.Payload.Event.Bits)).CallDeferred();
		GD.Print("Received a cheer!");
		return Task.CompletedTask;
	}

	private Task WebsocketClientOnChannelBan(object sender, ChannelBanArgs args)
	{
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.Ban)).CallDeferred();
		return Task.CompletedTask;
	}
	
	private Task WebsocketClientOnChannelChatMessage(object sender, ChannelChatMessageArgs args)
	{
		GD.Print(args.Notification.Payload.Event.ChatterUserName);
		GD.Print(args.Notification.Payload.Event.Message.Text);
		Callable.From(() => _globalSceneSignals.Node.EmitSignal(GlobalSceneSignals.SignalName.ChatMessage,
			args.Notification.Payload.Event.ChatterUserName, args.Notification.Payload.Event.Message.Text,
			args.Notification.Payload.Event.ChatterUserId, args.Notification.Payload.Event.MessageId)).CallDeferred();
		
		return Task.CompletedTask;
	}
	
	
}
