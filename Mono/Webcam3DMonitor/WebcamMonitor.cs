using Godot;
using TwitchOverlay.Mono;
using TwitchOverlay.Mono.RigidBodyPlus;
using TwitchOverlay.Mono.WebcamServer;
using AstroRaider2.Utility.Timers;

public partial class WebcamMonitor : Control
{
	// Booleans
	private bool _isUsingRBPlus;
	private bool _isGlitching;
	
	// Signals
	private GlobalSceneSignals GlobalSignalRef;
	private RigidBodyPlus _rigidBodyPlus;
	[Signal]
	public delegate void DebugGlitchEventHandler();
	
	// Webcam and related objects
	private WebcamServer _webcam;
	private TextureRect _textureDisplay;
	private RigidBody3D _rigidBody;
	private CollisionShape3D _collision;

	// Textures
	private AnimatedTexture _staticTexture;

	// Object Resetting
	private Vector3 _startingPosition;
	private Vector3 _startingRotation;
	private Transform3D _startingTransform;
	private Tween _tween;
	
	// RNG
	RandomNumberGenerator _rng = new RandomNumberGenerator();
	

	
	public override void _Ready()
	{
		Owner.PrintTree();
		GD.Print(Owner.ToString());
		// Check if we are using RigidBodyPlus
		if ((RigidBodyPlus)Owner != null)
		{
			_isUsingRBPlus = true;
			_rigidBodyPlus = (RigidBodyPlus)Owner;
		}
		GD.Print("RBPluss?: ",_isUsingRBPlus.ToString());
		
		// Get Nodes
		GlobalSignalRef = GetTree().Root.GetChild<Node3D>(0).GetNode<GlobalSceneSignals>("./GlobalSceneSignals");
		_webcam = GetTree().Root.GetChild<Node3D>(0).GetNode<WebcamServer>("./WebcamServer");
		_rigidBody = (RigidBody3D)Owner;
		_textureDisplay = this.GetChild<TextureRect>(0);
		_collision = Owner.GetNode<CollisionShape3D>("./CollisionShape3D");
		
		// Get starting position and rotation and store it.
		_startingPosition = _rigidBody.Position;
		_startingRotation = _rigidBody.Rotation;
		
		// Set up webcam and static
		_staticTexture = GD.Load<AnimatedTexture>("res://Images/Effects/TVStatic/TVStatic.tres");
		_textureDisplay.Texture = _webcam.CamTexture;
		
		// Connect Signals
		GlobalSignalRef.ResetPhysicsObjectsToInitialPosition += ResetObjects;
		_webcam.WebcamConnectionStatusChange += WebcamOnWebcamConnectionStatusChange;
		DebugGlitch += OnDebugGlitch;

		if (_isUsingRBPlus)
		{
			_rigidBodyPlus.BodyEntered += OnCollision;
		}
	}
	void OnDebugGlitch()
	{
		GlitchStatic(true);
	}
	public override void _Process(double delta)
	{
		if (_isGlitching)
		{
			GlitchStatic(false); //Process glitch
		}
	}


	private void OnCollision(Node body) //Only runs when the object is using RigidBodyPlus
	{
		float selfLinearVelocity;
		float otherLinearVelocity = 0;
		
		GD.Print(_rigidBodyPlus._linearVelocityHistory[1].ToString());
		
		if (body is RigidBody3D _otherRigid)
		{
			otherLinearVelocity = (Mathf.Abs(_otherRigid.LinearVelocity.X) + Mathf.Abs(_otherRigid.LinearVelocity.Y) + Mathf.Abs(_otherRigid.LinearVelocity.Z)) / 3;
		}
		selfLinearVelocity = (Mathf.Abs(_rigidBodyPlus._linearVelocityHistory[1].X) + Mathf.Abs(_rigidBodyPlus._linearVelocityHistory[1].Y) + Mathf.Abs(_rigidBodyPlus._linearVelocityHistory[1].Z)) / 3;
		
		if (selfLinearVelocity + otherLinearVelocity >= 1)
		{
			GD.Print("Ouchie! I'm damaged!");
			GlitchStatic(true);
		}
	}

	// Glitch Vars
	private CooldownTimer glitchTime = null;
	private CooldownTimer randomTime = null;
	private bool glitchSwitch;
	public void GlitchStatic(bool Start)
	{
		float currentRng = _rng.RandfRange(0.1f,0.20f);
		
		if (Start && _webcam._capture.IsOpened()) // If we are being told to start a new glitch sequence and the camera is ON
		{
			GD.Print("Glitch Start");
			glitchTime = new CooldownTimer(_rng.RandfRange(2,4));
			glitchTime.ResetCooldown();
			randomTime = new CooldownTimer(currentRng);
			randomTime.ResetCooldown();
			_isGlitching = true;
		}

		if (!Start && glitchTime != null) // If we are processing the glitch.
		{
			if (randomTime.HasCooldownElapsed()) // If cooldown expired, switch to webcam or static
			{
				randomTime = new CooldownTimer(currentRng); // Set up new timer and begin it
				randomTime.ResetCooldown();
				
				glitchSwitch = !glitchSwitch; //Invert switch
				
				if (glitchSwitch)
				{
					_textureDisplay.Texture =  _webcam.CamTexture;
				}

				if (!glitchSwitch)
				{
					_textureDisplay.Texture =  _staticTexture;
				}
			}
				

			if (glitchTime != null && glitchTime.HasCooldownElapsed()) // Once the time has expired
			{
				_isGlitching = false;
				_textureDisplay.Texture = _webcam.CamTexture;
			}
		}
		
		
	}


	private void WebcamOnWebcamConnectionStatusChange(bool status)
	{
		if (status)
		{
			_textureDisplay.Texture = _webcam.CamTexture;
		}

		if (!status)
		{
			_textureDisplay.Texture = _staticTexture;
		}
	}

	// Reset Object Functions
	private void ResetObjects()
	{
		GD.Print("RESETTING PHYSICS OBJECTS");
		PrepareForTween();

		_tween = _rigidBody.CreateTween();
		_tween.SetParallel(true);
		_tween.SetTrans(Tween.TransitionType.Quad);
		_tween.TweenProperty(_rigidBody,"position",_startingPosition,1);
		_tween.TweenProperty(_rigidBody,"rotation",_startingRotation,1);
		_tween.Chain().TweenCallback(Callable.From(RestorePhysicsAfterTween)); // Don't know if this is needed.
	}

	private void PrepareForTween()
	{
		_rigidBody.Freeze = true;
	}

	private void RestorePhysicsAfterTween()
	{
		_rigidBody.Freeze = false;
	}
}
