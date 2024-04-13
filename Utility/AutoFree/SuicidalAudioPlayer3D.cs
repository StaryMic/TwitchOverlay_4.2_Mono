using Godot;
using System;

namespace AstroRaider2.Utility.Autokill;

[GlobalClass]
 
public partial class SuicidalAudioPlayer3D : AudioStreamPlayer3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Finished += FreeYourself;
	}


	private void FreeYourself()
	{
		QueueFree();
	}
}
