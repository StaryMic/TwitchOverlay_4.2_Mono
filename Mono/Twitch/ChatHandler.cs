using System;
using System.Threading.Tasks;
using AstroRaider2.Utility.Timers;
using Godot;
using Godot.Collections;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace TwitchOverlay.Mono.Twitch;

[GlobalClass]
public partial class ChatHandler : Node
{
    private TwitchAPI _twitchApiRef;
    private GlobalSceneSignals _globalSceneSignals;
    private Timer _timer;

    private CooldownTimer _cooldownTimer;
    private int _messageSentThreshold = 15;
    
    private int _messagesSent;

    [Export] private Array<String> _timerMessages;
    
    public override void _Ready()
    {
        _twitchApiRef = GetTree().Root.GetChild(0).GetNode<TwitchAPI>("TwitchAPI");
        _globalSceneSignals = GetTree().Root.GetChild(0).GetNode<GlobalSceneSignals>("GlobalSceneSignals");
        _timer = GetNode<Timer>("CheckTimer");
        _cooldownTimer = new CooldownTimer(TimeSpan.FromMinutes(1));

        _cooldownTimer.ResetCooldown();
        
        _timer.Timeout += TimerOnTimeout;
        _twitchApiRef._websocketClient.ChannelChatMessage += WebsocketClientOnChannelChatMessage;
    }

    private void TimerOnTimeout()
    {
        if (_messagesSent >= _messageSentThreshold && _cooldownTimer.HasCooldownElapsed())
        {
            _messageSentThreshold = _messagesSent + 15;
            _cooldownTimer.ResetCooldown();
            
            _twitchApiRef.SendChatMessage("[BOT]: " + _timerMessages.PickRandom());
        }
    }

    private Task WebsocketClientOnChannelChatMessage(object sender, ChannelChatMessageArgs args)
    {
        _messagesSent++;
        
        string message = args.Notification.Payload.Event.Message.Text;
        string username = args.Notification.Payload.Event.ChatterUserName;
        string userId = args.Notification.Payload.Event.ChatterUserId;
        string messageId = args.Notification.Payload.Event.MessageId;

        if (message.StartsWith("!"))
        {
            // Send to a command handler.
            GD.Print("ChatHandler.CS: Got a chat command.");
            Callable.From(() => _globalSceneSignals.EmitSignal(GlobalSceneSignals.SignalName.CommandMessage,
                username, message, userId, messageId)).CallDeferred();
            
            return Task.CompletedTask;
        }

        if (message.Contains("[BOT]") && username == "starry_mic")
        {
            GD.Print("ChatHandler.CS: Bot has sent a message. Handle this later.");
            
            Callable.From(() => _globalSceneSignals.EmitSignal(GlobalSceneSignals.SignalName.BotMessage,
                username, message, userId, messageId)).CallDeferred();
            
            return Task.CompletedTask;
        }
        
        Callable.From(() => _globalSceneSignals.EmitSignal(GlobalSceneSignals.SignalName.ChatMessage,
            username, message, userId, messageId)).CallDeferred();
        
        GD.Print(message);
        return Task.CompletedTask;
    }
}