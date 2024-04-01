using Godot;
using System;
using Godot.Collections;
using TwitchOverlay.Mono;

[GlobalClass]
[Icon("res://Images/ScriptIcons/RigidBody3DPlus.svg")]

public partial class RigidBodyPlus : RigidBody3D
{
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

	private void OnCollision(Node body)
	{
		GD.Print("We collided.");
		GD.Print(body.Name.ToString());
		GD.Print(this.LinearVelocity.ToString());
		GD.Print(this.AngularVelocity.ToString());
		
		// Get velocity on impact and store it.
		float impactVelocity = Mathf.Abs((LinearVelocity.X + LinearVelocity.Y + LinearVelocity.Z) / 3);
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
			GD.Print(newAudioStreamPlayer.VolumeDb);
			
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
