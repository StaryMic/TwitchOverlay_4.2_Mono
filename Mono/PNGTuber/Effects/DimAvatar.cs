using Godot;
using OpenCvSharp.Dnn;

namespace TwitchOverlay.Mono.PNGTuber.Effects;

[GlobalClass]
public partial class DimAvatar : PNGTuberEffectBase
{
    [Export] public float Value = 1;
    public override void ProcessEffect(Node2D target)
    {
        target.SelfModulate = new Color(Value,Value,Value);
    }

    public override void ResetEffect(Node2D target)
    {
        target.SelfModulate = Colors.White;
    }
}