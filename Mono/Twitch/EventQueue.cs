using System;
using Godot;
using AstroRaider2.Utility.Timers;
using Godot.Collections;
using TwitchOverlay.Mono;
using TwitchOverlay.Mono.Twitch;

[GlobalClass]
public partial class EventQueue : Node
{
	public bool IsProcessingQueue = true;

	private GlobalSceneSignals _globalSceneSignalsRef;
	private CooldownTimer _queueTimer;
	
	// Connected Event Object References
	private NotificationDisplay _notificationDisplay;

	private Array<Godot.Collections.Dictionary<string, string>> _eventQueue;
	// EVERYTHING IS A STRING SO IT DOESN'T COMPLAIN ABOUT COMPARING TO VARIANTS

	private Godot.Collections.Dictionary<string,string> _templateEventDictionary = new Godot.Collections.Dictionary<string,string>()
	{
		{"NotificationType", "_default"},
		{"Username", "_default"},
		{"SubTier", "_default"},
		{"Gifted", "false"},
		{"TotalGifts", "0"},
		{"Message", "_defaultMessage"},
		{"CumulativeMonths", "0"},
		{"Bits", "0"},
		{"RaidViewers", "0"}
	};

	private Array<String> _processedUsernames;
	
	// SIGNALS
	[Signal]
	public delegate void ToggleQueueEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_eventQueue = new Array<Godot.Collections.Dictionary<string, string>>();
		_globalSceneSignalsRef = GetTree().Root.GetChild(0).GetNode<GlobalSceneSignals>("GlobalSceneSignals");
		_notificationDisplay = GetTree().Root.GetChild(0).GetNode<NotificationDisplay>("EventObjects/NotificationDisplay");
		_queueTimer = new CooldownTimer(7);
		_queueTimer.ResetCooldown();
		AddNotificationToQueue(EventAttributes.NotificationTypeFollow, "TESTTHINGY", "", false, 0, "", 0, 0, 0);
		AddNotificationToQueue(EventAttributes.NotificationTypeRaid, "RaidTester", "", false, 0, "", 0, 0, 69420);
		
		_globalSceneSignalsRef.Follow += GlobalSceneSignalsRefOnFollow;
		_globalSceneSignalsRef.Subscription += GlobalSceneSignalsRefOnSubscription;
		_globalSceneSignalsRef.GiftSubscription += GlobalSceneSignalsRefOnGiftSubscription;
		_globalSceneSignalsRef.MessageSubscription += GlobalSceneSignalsRefOnMessageSubscription;
		_globalSceneSignalsRef.Cheer += GlobalSceneSignalsRefOnCheer;
		_globalSceneSignalsRef.Raid += GlobalSceneSignalsRefOnRaid;

		ToggleQueue += () =>
		{
			IsProcessingQueue = !IsProcessingQueue;
		};
	}

	private void GlobalSceneSignalsRefOnRaid(string username, int totalviewers)
	{
		AddNotificationToQueue(EventAttributes.NotificationTypeRaid, username, "", false, 0, "", 0, 0, totalviewers);
	}

	private void GlobalSceneSignalsRefOnCheer(string username, string message, int bits)
	{
		AddNotificationToQueue(EventAttributes.NotificationTypeCheer, username, "", false, 0, message, 0, bits, 0);
	}

	private void GlobalSceneSignalsRefOnMessageSubscription(string username, string tier, string message, int totalsubmonths, int durationmonths)
	{
		AddNotificationToQueue(EventAttributes.NotificationTypeSubscriptionMessage, username, tier, false, 0, message, totalsubmonths, 0, 0);
	}

	private void GlobalSceneSignalsRefOnGiftSubscription(string username, string tier, int totalgifts)
	{
		AddNotificationToQueue(EventAttributes.NotificationTypeGiftedSubscription, username, tier, false, totalgifts, "", 0, 0, 0);
	}

	private void GlobalSceneSignalsRefOnSubscription(string username, string tier, bool gifted)
	{
		AddNotificationToQueue(EventAttributes.NotificationTypeSubscription, username, tier, gifted, 0, "", 0, 0, 0);
	}

	private void GlobalSceneSignalsRefOnFollow(string username)
	{
		if (!_processedUsernames.Contains(username))
		{
			AddNotificationToQueue(EventAttributes.NotificationTypeFollow, username, "", false, 0, "", 0, 0, 0);
			_processedUsernames.Add(username);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_queueTimer.HasCooldownElapsed() && _eventQueue.Count >= 1 && IsProcessingQueue)
		{
			_notificationDisplay.EmitSignal(NotificationDisplay.SignalName.ShowNotification, _eventQueue[0]);
			_eventQueue.RemoveAt(0);
			_queueTimer.ResetCooldown();
		}
	}

	public void AddNotificationToQueue(string NotificationType, string Username, string SubTier, bool Gifted,
		int TotalGifts, string Message, int CumulativeMonths, int Bits, int RaidViewers)
	{
		Godot.Collections.Dictionary<string, string> filledDictionary = _templateEventDictionary.Duplicate();
		filledDictionary["NotificationType"] = NotificationType;
		filledDictionary["Username"] = Username;
		filledDictionary["SubTier"] = SubTier;
		filledDictionary["Gifted"] = Gifted.ToString();
		filledDictionary["TotalGifts"] = TotalGifts.ToString();
		filledDictionary["Message"] = Message;
		filledDictionary["CumulativeMonths"] = CumulativeMonths.ToString();
		filledDictionary["Bits"] = Bits.ToString();
		filledDictionary["RaidViewers"] = RaidViewers.ToString();
		
		_eventQueue.Add(filledDictionary);
	}
}
