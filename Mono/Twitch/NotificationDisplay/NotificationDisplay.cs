using Godot;
using System;
using Godot.Collections;
using TwitchOverlay.Mono.Twitch;

public partial class NotificationDisplay : Control
{
	// Scene Objects
	private RichTextLabel _topLabel;
	private RichTextLabel _bottomLabel;
	private TextureRect _textureRect;
	private AnimationPlayer _animationPlayer;
	
	// Signals
	[Signal]
	public delegate void ShowNotificationEventHandler(Dictionary<string,string> NotificationInfo);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_topLabel = GetNode<RichTextLabel>("NotificationPanel/VBoxContainer/TopLabel");
		_bottomLabel = GetNode<RichTextLabel>("NotificationPanel/VBoxContainer/BottomLabel");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		_animationPlayer.Play("Appear");
		
		ShowNotification += OnShowNotification;
	}

	private void OnShowNotification(Dictionary<string,string> notificationinfo)
	{
		GD.Print("NotificationDisplay.cs: Received OnShowNotification Signal");
		// Types are as follows.
		// Follows, Subscriptions, Subscription Messages, GiftSubs, Cheers, Raids 
		switch (notificationinfo["NotificationType"])
		{
			case EventAttributes.NotificationTypeFollow:
			{
				_topLabel.Text = RandomizeTopMessage();
				_bottomLabel.Text = $"[center]Thank you [Rainbow]{notificationinfo["Username"]}[/Rainbow] for the follow!";
				_animationPlayer.Play("Appear");
				break;
			}
			
			case EventAttributes.NotificationTypeSubscription:
			{
				_topLabel.Text = RandomizeTopMessage();
				switch (notificationinfo["Gifted"])
				{
					case "true":
						_bottomLabel.Text =
							$"[center][Rainbow]{notificationinfo["Username"]}[/Rainbow] was given a subscription!";
						break;
					case "false":
						_bottomLabel.Text =
							$"[center]Thank you [Rainbow]{notificationinfo["Username"]}[/Rainbow] for the sub!";
						break;
				}
				_animationPlayer.Play("Appear");
				break;
			}

			case EventAttributes.NotificationTypeSubscriptionMessage:
			{
				_topLabel.Text = RandomizeTopMessage();
				_bottomLabel.Text = $"[center]Thank you [Rainbow]{notificationinfo["Username"]}[/Rainbow] for the sub!";
				_animationPlayer.Play("Appear");
				break;
			}

			case EventAttributes.NotificationTypeGiftedSubscription:
			{
				_topLabel.Text = RandomizeTopMessage();
				_bottomLabel.Text = $"[center]Everybody please say thanks to [Rainbow]{notificationinfo["Username"]}[/Rainbow] for the [Rainbow]{notificationinfo["TotalGifts"]}[/Rainbow] gift subs!";
				_animationPlayer.Play("Appear");
				break;
			}

			case EventAttributes.NotificationTypeCheer:
			{
				_topLabel.Text = RandomizeTopMessage();
				_bottomLabel.Text = $"[center]Thank you [Rainbow]{notificationinfo["Username"]}[/Rainbow] for the [Rainbow]{notificationinfo["Bits"]}[/Rainbow] bits!";
				_animationPlayer.Play("Appear");
				break;
			}
			
			case EventAttributes.NotificationTypeRaid:
			{
				_topLabel.Text = "[center][Rumble]OH SHIT A RAID!";
				_bottomLabel.Text = $"[center][Rumble][Rainbow]{notificationinfo["Username"]}[/Rainbow] HAS SENT [Rainbow]{notificationinfo["RaidViewers"]}[/Rainbow] AFTER US";
				_animationPlayer.Play("Appear");
				break;
			}
			default:
				GD.Print("Invalid notification message.");
				break;
		}
	}
	private string RandomizeTopMessage()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		string chosenMessage = "MESSAGE FAILED TO BE GRABBED.";
		switch (rng.RandiRange(0,5))
		{
			case 0:
			{
				chosenMessage = "[center][Rumble]HOLY FUCKING BINGLE!!!";
				break;
			}
			case 1:
			{
				chosenMessage = "[center][rainbow]AMAZING!!!";
				break;
			}
			case 2:
			{
				chosenMessage = "[center][rainbow]INCREDIBLE!!!";
				break;
			}
			case 3:
			{
				chosenMessage = "[center]I can't thank you enough!";
				break;
			}
			case 4:
			{
				chosenMessage = "[center]Thank you for your support!";
				break;
			}
			case 5:
			{
				chosenMessage = "[center]Your support means a lot to me!";
				break;
			}
		}

		return chosenMessage;
	}
}
