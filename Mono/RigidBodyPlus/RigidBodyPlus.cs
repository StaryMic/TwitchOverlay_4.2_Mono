using Godot;
using Godot.Collections;

namespace TwitchOverlay.Mono.RigidBodyPlus;

[GlobalClass]
[Icon("res://Images/ScriptIcons/RigidBody3DPlus.svg")]

public partial class RigidBodyPlus : RigidBody3D
{
	
	// Debug
	[Export(PropertyHint.Enum,"None,Collision,LinearVelocity")] public int DebugMode;
	
	// Global signal connection
	private GlobalSceneSignals GlobalSignalRef;
	
	// Impact sounds
	[ExportCategory("Physics Sounds")] [Export]
	public Array<AudioStream> _ImpactSounds;
	
	private AudioStreamPlayer3D newAudioStreamPlayer;
	private AudioStreamPlayer3D instanceAudio;
	private float MinimumVolume = -20; // in dB
	private float MaximumVolume = -15; // in dB

	private int soundsPlayed;
	
	// Object Resetting
	private Vector3 _startingGlobalPosition;
	private Vector3 _startingGlobalRotation;
	private Tween _tween;

	
	public override void _Ready()
	{
		// Set up global signal ref.
		GlobalSignalRef = GetTree().Root.GetChild<Node3D>(0).GetNode<GlobalSceneSignals>("./GlobalSceneSignals");
		
		// Bind signals
		GlobalSignalRef.ResetPhysicsObjectsToInitialPosition += ResetToStartingTransform;
		BodyEntered += OnCollision;
		
		
		// Store starting position
		_startingGlobalPosition = this.GlobalPosition;
		_startingGlobalRotation = this.GlobalRotation;
	}

	public override void _Process(double delta)
	{
		switch (DebugMode)
		{
			case 0: // None
				break;
			case 1: // Collision
				break;
			case 2: // Linear Velocity
				GD.Print(this.Name," ",LinearVelocity.ToString());
				break;
		}
	}

	// Physics Array Stuff.
	public Array<Vector3> _linearVelocityHistory = new Array<Vector3>();
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		_linearVelocityHistory.Insert(0,state.LinearVelocity);
		_linearVelocityHistory.Resize(2);

		if (DebugMode == 2)
		{
			GD.Print("==========");
			GD.Print(_linearVelocityHistory.ToString());
			GD.Print("==========");
		}
	}

	private void OnCollision(Node body)
	{
		if (DebugMode == 1)
		{
			GD.Print("We collided.");
			GD.Print(body.Name.ToString());
			GD.Print(this.LinearVelocity.ToString());
			GD.Print(this.AngularVelocity.ToString());
		}
		
		// Get velocity on impact and store it.
		// This uses the previous frame's velocity to get the impact speed to work around some collision stuff in Godot.
		float impactVelocity = Mathf.Abs(((_linearVelocityHistory[1].X + _linearVelocityHistory[1].Y + _linearVelocityHistory[1].Z) / 3) * Mass);
		GD.Print(impactVelocity);

		if (impactVelocity >= 1) // in Meters per second???
		{
			// Create and store template audio stream player.
			newAudioStreamPlayer = new AudioStreamPlayer3D();
			newAudioStreamPlayer.Bus = "RigidBodyImpacts";
			newAudioStreamPlayer.Autoplay = true;
			newAudioStreamPlayer.VolumeDb = 0;
			newAudioStreamPlayer.AttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.InverseDistance;
			newAudioStreamPlayer.MaxDb = -15;
			newAudioStreamPlayer.PanningStrength = (float)1.4;
			newAudioStreamPlayer.UnitSize = (float)5;
			newAudioStreamPlayer.AttenuationFilterDb = -6;
			newAudioStreamPlayer.Stream = _ImpactSounds.PickRandom();
			
			soundsPlayed++; //increment sounds played.
			
			// Map audio to sound volume
			// Select the lower value so we don't go over our volume limit
			newAudioStreamPlayer.VolumeDb = Mathf.Min(MinimumVolume + (impactVelocity / 2), MaximumVolume);
			GD.Print("Volume: ",newAudioStreamPlayer.VolumeDb);
			
			// Change name so it doesn't complain.
			newAudioStreamPlayer.Name = "ImpactNoise" + soundsPlayed;
			
			// Spawn audio player in scene.
			AddSibling(newAudioStreamPlayer);
			
			// Set player position in world space.
			newAudioStreamPlayer.GlobalPosition = this.GlobalPosition;
		}
	}
	//Object Resetting code
	private void ResetToStartingTransform()
	{
		GD.Print("RESETTING PHYSICS OBJECTS");
		PrepareForTween();

		_tween = CreateTween();
		_tween.SetParallel(true);
		_tween.SetTrans(Tween.TransitionType.Quad);
		_tween.TweenProperty(this,"position",_startingGlobalPosition,1);
		_tween.TweenProperty(this,"rotation",_startingGlobalRotation,1);
		_tween.Chain().TweenCallback(Callable.From(RestorePhysicsAfterTween)); // Don't know if this is needed.
	}
	
	private void PrepareForTween()
	{
		this.Freeze = true;
	}

	private void RestorePhysicsAfterTween()
	{
		this.Freeze = false;
	}
	
}