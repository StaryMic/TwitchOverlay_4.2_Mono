using Godot;

[GlobalClass]
[Icon("res://Images/ScriptIcons/Bootstrap-Bootstrap-Bootstrap-rocket.svg")]
public partial class RBPlusThruster : Node3D
{
    private RigidBody3D _rigidBody3D;
    [Export] public float _thrustMagnitude = 0f;
    public override void _Ready()
    {
        // Assume parent is Rigidbody3d or inherits it
        _rigidBody3D = this.GetParent<RigidBody3D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        _rigidBody3D.ApplyForce((-this.GlobalBasis.Y * _thrustMagnitude));
    }
}