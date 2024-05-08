using Godot;
using System;
using AstroRaider2.Utility.NodeTree;

public partial class FlyingCam : Camera3D
{

	private float _movementSpeed = 15;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
			Rotate(this.Basis.X,mouseMotion.Relative.Y / -100);
			
			// Apply left/right rotation using PHYSICS BODY Y axis. (Left and Right)
			this.RotateY(mouseMotion.Relative.X / -100);
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

	private void HandleInput(double delta)
	{
		if (Input.IsActionPressed("MoveForward"))
		{
			this.GlobalPosition = (this.Basis.Z * (_movementSpeed * -1) * (float)delta);
		}

		if (Input.IsActionPressed("MoveBackward"))
		{
			this.GlobalPosition = (this.Basis.Z * (_movementSpeed) * (float)delta);
		}
		
		if (Input.IsActionPressed("MoveLeft"))
		{
			this.GlobalPosition = (this.Basis.X * (_movementSpeed * -1) * (float)delta);
		}
		
		if (Input.IsActionPressed("MoveRight"))
		{
			this.GlobalPosition = (this.Basis.X * (_movementSpeed) * (float)delta);
		}
		if (Input.IsActionPressed("MoveUp"))
		{
			this.GlobalPosition = (this.Basis.Y * (_movementSpeed) * (float)delta);
		}
		if (Input.IsActionPressed("MoveDown"))
		{
			this.GlobalPosition = (this.Basis.Y * (_movementSpeed * -1) * (float)delta);
		}
	}
}
