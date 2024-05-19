using Godot;
using Godot.Collections;

namespace TwitchOverlay.Mono;
[GlobalClass]
public partial class GlobalSceneSignals : Node
{
    [Signal] // Reset Physics Objects to Initial Position.
    public delegate void ResetPhysicsObjectsToInitialPositionEventHandler();

    [Signal] // Emergency Reset for PredictionSlider
    public delegate void ForceEndPredictionEventHandler();

    [Signal]
    public delegate void ChatMessageEventHandler(string Username, string Message, string UserID, string MessageId);

    [Signal]
    public delegate void SubscriptionEventHandler(string Username, string Tier, bool Gifted);

    [Signal]
    public delegate void GiftSubscriptionEventHandler(string Username, string Tier, int TotalGifts);

    [Signal]
    public delegate void MessageSubscriptionEventHandler(string Username, string Tier, string Message, int TotalSubMonths,
        int DurationMonths);
    
    [Signal]
    public delegate void CheerEventHandler(string Username, string Message, int Bits);

    [Signal]
    public delegate void RaidEventHandler(string Username, int TotalViewers);

    [Signal]
    public delegate void ChannelPointEventHandler(string Username, string RewardTitle, string UserInput);

    [Signal]
    public delegate void BanEventHandler();

    [Signal]
    public delegate void ChannelPollEventHandler(string Title, Array<Dictionary> Choices, string PollStatus);

    [Signal]
    public delegate void ChannelPredictionEventHandler(string Title, Array<Dictionary> Outcomes,
        string PredictionStatus, int WinningOutcomeID);

    [Signal]
    public delegate void HypeTrainEventHandler(int TotalPoints, int CurrentLevel, int PointGoal, bool Expired);

    [Signal]
    public delegate void GoalEventHandler(string Type, string Description, int CurrentAmount, int TargetAmount,
        bool IsAchieved); // https://dev.twitch.tv/docs/eventsub/eventsub-reference/#goals-event

    [Signal]
    public delegate void FollowEventHandler(string Username);

    [Signal]
    public delegate void BotMessageEventHandler(string Username, string Message, string UserID, string MessageId);

    [Signal]
    public delegate void CommandMessageEventHandler(string Username, string Message, string UserID, string MessageId);
}