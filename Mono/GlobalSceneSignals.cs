using Godot;

namespace TwitchOverlay.Mono;
[GlobalClass]
public partial class GlobalSceneSignals : Node
{
    [Signal] // Reset Physics Objects to Initial Position.
    public delegate void ResetPhysicsObjectsToInitialPositionEventHandler();
}