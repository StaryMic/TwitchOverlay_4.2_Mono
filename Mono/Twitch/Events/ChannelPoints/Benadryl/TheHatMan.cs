using Godot;
using System;
using System.Collections.Generic;
using AstroRaider2.Utility.Timers;

public partial class TheHatMan : Sprite2D
{
	private CooldownTimer _cooldownTimer;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	private Tween tween;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_cooldownTimer = new CooldownTimer(_rng.RandiRange(120, 240));
		_cooldownTimer.ResetCooldown();

		tween = CreateTween();
		tween.TweenProperty(this, "self_modulate:a", 0.3f, 45);
		tween.Play();
	}

	public override void _Process(double delta)
	{
		if (_cooldownTimer.HasCooldownElapsed())
		{
			Tween tween = CreateTween();
			tween.TweenProperty(this, "self_modulate:a", 0, 45);
			tween.TweenCallback(callback: Callable.From(QueueFree));
			tween.Play();
		}
	}
}
