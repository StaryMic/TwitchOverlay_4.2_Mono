using Godot;
using Godot.Collections;
using AstroRaider2.Utility.Timers;

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
	
	[ExportCategory("Sound Settings")]
	[Export]
	private float MinimumVolume = -20; // in dB
	[Export]
	private float MaximumVolume = -15; // in dB

	[Export]
	private AudioStreamPlayer3D.AttenuationModelEnum _soundAttenuationModel = AudioStreamPlayer3D.AttenuationModelEnum.Logarithmic;
	[Export]
	private float _unitSize = 10;
	[Export(PropertyHint.Range,"-24,6")]
	private float _maxDB = 3;
	[Export]
	private float _maxDistanceInMeters = 0;
	[Export]
	private string _bus = "Master";
	[Export]
	private AudioStreamPlayer3D.DopplerTrackingEnum _dopplerTracking = AudioStreamPlayer3D.DopplerTrackingEnum.Disabled;

	[Export] private bool _randomizePitch;
	[Export(PropertyHint.Range,"0.01,4")] private float _minimumPitchScale = 1;
	[Export(PropertyHint.Range,"0.01,4")] private float _maximumPitchScale = 1;

	private int soundsPlayed;
	
	// Object Resetting
	[ExportCategory("RBPlus Properties")]
	[Export]
	private bool _importantObject;
	private Vector3 _startingGlobalPosition;
	private Vector3 _startingGlobalRotation;
	private Tween _tween;

	private CooldownTimer _soundCooldown;

	
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

		_soundCooldown = new CooldownTimer(0.15);
		_soundCooldown.ResetCooldown();
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

		if (this.Position.Y < -500 && !_importantObject) // If we are out of bounds and not important.
		{
			GD.Print("Out of bounds. Freeing RBPlus object.");
			QueueFree();
		}

		if (this.Position.Y < -500 && _importantObject) // If we are out of bounds and important AF
		{
			GD.Print("Out of bounds. Freezing important object");
			this.Position += Vector3.Up * 75;
			this.Freeze = true;
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
		if (DebugMode == 1)
		{
			GD.Print(impactVelocity);
		}

		if (impactVelocity >= 1 && _soundCooldown.HasCooldownElapsed()) // in Meters per second. Checks if cooldown has passed.
		{
			AudioStreamPlayer3D newAudioStreamPlayer;
			
			GD.Print(GetContactCount());
			GD.Print("Cooldown elapsed? : ",_soundCooldown.HasCooldownElapsed());
			
			// Reset Cooldown
			_soundCooldown.ResetCooldown();
			
			// Create and store template audio stream player.
			newAudioStreamPlayer = new AudioStreamPlayer3D();
			newAudioStreamPlayer.Bus = _bus;
			newAudioStreamPlayer.Autoplay = false;
			newAudioStreamPlayer.VolumeDb = 0;
			newAudioStreamPlayer.AttenuationModel = _soundAttenuationModel;
			newAudioStreamPlayer.MaxDb = _maxDB;
			newAudioStreamPlayer.PanningStrength = (float)1.4;
			newAudioStreamPlayer.UnitSize = _unitSize;
			newAudioStreamPlayer.AttenuationFilterDb = -6;
			newAudioStreamPlayer.MaxDistance = _maxDistanceInMeters;
			newAudioStreamPlayer.Stream = _ImpactSounds.PickRandom();
			newAudioStreamPlayer.DopplerTracking = _dopplerTracking;
			
			// Randomize pitch scale according to settings
			if (_randomizePitch) // If random pitch is enabled.
			{
				RandomNumberGenerator rng = new RandomNumberGenerator();
				newAudioStreamPlayer.PitchScale = rng.RandfRange(_minimumPitchScale, _maximumPitchScale);
			}
			
			soundsPlayed++; //increment sounds played.
			
			// Map audio to sound volume
			// Select the lower value, so we don't go over our volume limit
			newAudioStreamPlayer.VolumeDb = Mathf.Min(MinimumVolume + (impactVelocity / 2), MaximumVolume);
			GD.Print("Volume: ",newAudioStreamPlayer.VolumeDb);
			
			// Change name so it doesn't complain.
			newAudioStreamPlayer.Name = "ImpactNoise" + soundsPlayed;
			
			// Add Autokill script
			newAudioStreamPlayer.SetScript("res://Mono/RigidBodyPlus/AudioAutoKill.gd");
			
			// Spawn audio player in scene.
			AddSibling(newAudioStreamPlayer);
			
			// Connect Finished to QueueFree
			newAudioStreamPlayer.Finished += newAudioStreamPlayer.QueueFree;
			
			// Play audio
			newAudioStreamPlayer.Play();
			
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