using Godot;

namespace TwitchOverlay.Mono;
[GlobalClass]
public partial class GlobalSceneSignals : Node
{
    [Signal] // Reset Physics Objects to Initial Position.
    public delegate void ResetPhysicsObjectsToInitialPositionEventHandler();

    [Signal]
    public delegate void ChatMessageEventHandler(string Username, string Message, string UserID, string MessageId);
}