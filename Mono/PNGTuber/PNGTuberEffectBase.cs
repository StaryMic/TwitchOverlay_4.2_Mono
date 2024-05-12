using Godot;

namespace TwitchOverlay.Mono.PNGTuber;
[GlobalClass]
public partial class PNGTuberEffectBase : Resource
{
    public virtual void ResetEffect(){}

    public virtual void ProcessEffect(Node2D target){}
}