using Godot;

public partial class BottleManager : Node
{
    private MeshInstance3D _bottleMeshInstance;
    private RBPlusThruster _thruster;
    private AudioStreamPlayer3D _audio;
    private GpuParticles3D _particles3D;

    private Tween _tween;
    
    public override void _Ready()
    {
        _bottleMeshInstance = this.GetParent().GetNode<MeshInstance3D>("Cube");
        _thruster = this.GetParent().GetNode<RBPlusThruster>("RBPlusThruster");
        _audio = this.GetParent().GetNode<AudioStreamPlayer3D>("Sodarushing");
        _particles3D = this.GetParent().GetNode<GpuParticles3D>("GPUParticles3D");

        _particles3D.Emitting = true;

        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.In);
        _tween.SetTrans(Tween.TransitionType.Expo);
        _tween.Parallel().TweenProperty(_thruster, "_thrustMagnitude", 0,5);
        _tween.Parallel().TweenProperty(_bottleMeshInstance, "blend_shapes/Deflate", 1, 5);
        _tween.Parallel().TweenProperty(_audio, "volume_db", -80, 5);
        _tween.TweenInterval(2);
        _tween.TweenProperty(_bottleMeshInstance, "scale", new Vector3(0.01f, 0.01f, 0.01f), 1);
        _tween.TweenCallback(Callable.From(this.GetParent().QueueFree));
    }
}