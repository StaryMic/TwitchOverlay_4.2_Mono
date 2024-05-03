using Godot;
using System;

public partial class Bit2D : RigidBody2D
{
	public int Value = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Mass += Mathf.Min(1,Value / 10);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
