using Godot;
using System;
using AstroRaider2.Utility.NodeTree;

public partial class RigidFlyingCamera : RigidBody3D
{
	private Camera3D _CameraRef;

	private float _movementSpeed = 15;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_CameraRef = this.GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	// Get input from Keyboard and Mouse
	public override void _Input(InputEvent @event) // Use for singular presses and mouse movement. Not for held keys or repeating actions.
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			// Mouse Movement
			GD.Print("Mouse has moved");
			GD.Print(mouseMotion.Relative);
			
			// Apply rotation to CAMERA's X Axis (up and down)
			_CameraRef.Rotate(_CameraRef.Basis.X,mouseMotion.Relative.Y / -100);
			
			// Apply left/right rotation using PHYSICS BODY Y axis. (Left and Right)
			_CameraRef.RotateY(mouseMotion.Relative.X / -100);
		}

		if (@event is InputEventMouseButton mouseButton && @event.IsPressed())
		{
			// Mouse Buttons
			GD.Print("Mouse button has been pressed");
			GD.Print(mouseButton.ButtonIndex.ToString());
		}

		if (@event is InputEventKey key){
			
			// Quit game
			if (key.Keycode == Key.Escape)
			{
				GetTree().Quit();
			}

			if (key.Keycode == Key.V && key.Pressed) //Noclip
			{
				NodeRef<CollisionShape3D> collider = new NodeRef<CollisionShape3D>(this, "@CollisionShape3D");
				collider.Node.Disabled = !collider.Node.Disabled;
			}
			
			//More Actions here later
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Nothing here atm.
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleInput(delta);
	}

	private Vector3 _maxMovementSpeed = new Vector3(25,25,25);
	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if (state.LinearVelocity.Length() > _maxMovementSpeed.Length())
		{
			state.LinearVelocity.Clamp(_maxMovementSpeed * -1, _maxMovementSpeed);
		}
	}

	private void HandleInput(double delta)
	{
		if (Input.IsActionPressed("MoveForward"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.Z * (_movementSpeed * -1) * (float)delta);
		}

		if (Input.IsActionPressed("MoveBackward"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.Z * (_movementSpeed) * (float)delta);
		}
		
		if (Input.IsActionPressed("MoveLeft"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.X * (_movementSpeed * -1) * (float)delta);
		}
		
		if (Input.IsActionPressed("MoveRight"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.X * (_movementSpeed) * (float)delta);
		}
		if (Input.IsActionPressed("MoveUp"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.Y * (_movementSpeed) * (float)delta);
		}
		if (Input.IsActionPressed("MoveDown"))
		{
			this.ApplyCentralImpulse(_CameraRef.Basis.Y * (_movementSpeed * -1) * (float)delta);
		}
	}
}
