using System;
using System.Diagnostics;
using AstroRaider2.Utility.Timers;
using Godot;

namespace TwitchOverlay.Mono.PNGTuber.Effects;

[GlobalClass]
public partial class RandomMovement : PNGTuberEffectBase
{
    // Private Variables
    private Vector2 _startingPosition;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    private Vector2 _targetPosition;
    private int _frameTimer;
    private int _nextRepositionFrame = 0;
    
    // Effect Parameters
    [Export] private float _range = 100f;
    [Export] private float _movementSpeed = 10f;
    [Export] private int _repositionFrequencyInFrames = 10;
    
    public override void ProcessEffect(Node2D target)
    {
        // If the current frame is a reposition frame.
        if (_frameTimer >= _nextRepositionFrame)
        {
            _nextRepositionFrame += _repositionFrequencyInFrames;
            // Randomize target position.
            // Takes in Range (in pixels) as an argument.
        
            _targetPosition = new Vector2(_rng.RandfRange(-_range, _range), _rng.RandfRange(-_range, _range)) + target.GetParent<Window>().Size/2;
        }
        
        // Start moving toward the target position.
        // We get a vector from Sin and Cos.
        float angleToTarget = target.GetAngleTo(_targetPosition);
        target.GlobalPosition += new Vector2(Mathf.Sin(angleToTarget), Mathf.Cos(angleToTarget)) * _movementSpeed;
        
        // Increment the frame timer.
        _frameTimer++;

        // Debug
        GD.Print("Target Position: " + _targetPosition);
        GD.Print("Angle To TargetPosition: " + Mathf.RadToDeg(angleToTarget));
        GD.Print("Current Position: " + target.Position);
        GD.Print("Distance: " + target.GlobalPosition.DistanceTo(_targetPosition));
    }

    public override void ResetEffect(Node2D target)
    {
        // Center Sprite to screen again
        target.Position = target.GetParent<Window>().Size / 2;
    }
}