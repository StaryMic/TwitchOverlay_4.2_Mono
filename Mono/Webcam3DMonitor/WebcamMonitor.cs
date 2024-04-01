using Godot;
using System;
using TwitchOverlay.Mono;
using TwitchOverlay.Mono.WebcamServer;

public partial class WebcamMonitor : MeshInstance3D
{
	private GlobalSceneSignals GlobalSignalRef;
	
	private WebcamServer _webcam;
	private Material _material;
	private RigidBody3D _rigidBody;
	private CollisionShape3D _collision;

	private AnimatedTexture _staticTexture;

	private Vector3 _startingPosition;
	private Vector3 _startingRotation;
	private Transform3D _startingTransform;

	private Tween _tween;
	

	
	public override void _Ready()
	{
		// Get Nodes
		GlobalSignalRef = GetTree().Root.GetNode<GlobalSceneSignals>("./Overlay/GlobalSceneSignals");
		_webcam = GetTree().Root.GetNode<WebcamServer>("./Overlay/WebcamServer");
		_rigidBody = GetParent<RigidBody3D>();
		_collision = GetNode<CollisionShape3D>("../Collision");
		
		// Get starting position and rotation and store it.
		_startingPosition = _rigidBody.Position;
		_startingRotation = _rigidBody.Rotation;
		
		_staticTexture = GD.Load<AnimatedTexture>("res://Images/Effects/TVStatic/TVStatic.tres");
		_material = this.GetSurfaceOverrideMaterial(1);
		_material.Set("emission_texture",_webcam.CamTexture);
		
		// Connect Signals
		GlobalSignalRef.ResetPhysicsObjectsToInitialPosition += ResetObjects;
		_webcam.WebcamConnectionStatusChange += WebcamOnWebcamConnectionStatusChange;
	}

	private void WebcamOnWebcamConnectionStatusChange(bool status)
	{
		if (status)
		{
			_material.Set("emission_texture",_webcam.CamTexture);
		}

		if (!status)
		{
			_material.Set("emission_texture",_staticTexture);
		}
	}

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
		_collision.Disabled = true;
		_rigidBody.Freeze = true;
	}

	private void RestorePhysicsAfterTween()
	{
		_collision.Disabled = false;
		_rigidBody.Freeze = false;
	}
}
