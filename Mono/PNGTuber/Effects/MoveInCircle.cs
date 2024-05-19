using Godot;

namespace TwitchOverlay.Mono.PNGTuber.Effects;

[GlobalClass]
public partial class MoveInCircle : PNGTuberEffectBase
{
    [Export] public float AmplitudeInPixels = 25;
    [Export] public float Speed = 1000;
    public override void ProcessEffect(Node2D target)
    {
        target.Position = (new Vector2(Mathf.Sin(Time.GetTicksMsec() / Speed),
            Mathf.Cos(Time.GetTicksMsec() / Speed)) * AmplitudeInPixels) + (target.GetParent<Window>().Size / 2);
    }

    public override void ResetEffect(Node2D target)
    {
        // Center Sprite to screen again
        target.Position = target.GetParent<Window>().Size / 2;
    }
}