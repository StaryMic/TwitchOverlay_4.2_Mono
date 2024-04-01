using Godot;
using System;
using System.Linq;
using Godot.Collections;

[GlobalClass]

public partial class RigidBodyPlus : RigidBody3D
{
	[ExportCategory("Physics Sounds")] [Export]
	public Array<AudioStream> _ImpactSounds;

	[Export] public Array<AudioStream> _ScrapingSounds;
	
	public override void _Ready()
	{
		BodyEntered += OnCollision;
	}

	private void OnCollision(Node body)
	{
		// Body is the other item that interacted with us.
		
		this.force
	}

	public override void _PhysicsProcess(double delta)
	{
		//Implement sounds here.
	}
}
