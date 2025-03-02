using Godot;
using System;
using System.Linq;
using AstroRaider2.Utility.Timers;
using Godot.Collections;
using TwitchOverlay.Mono.PNGTuber;

public partial class PNGTuber : Window
{
	// ==================================================================================
	// SETTINGS
	[Export] public PNGTuberAvatarResource Avatar;
	[Export] public int MicrophoneSmoothingSamples = 25;

	[ExportCategory("Thresholds")]
	[Export] public float TalkThreshold = 0f;
	[Export] public float ScreamThreshold = 0f;
	[Export] private float _mouthCloseDelaySeconds = 0.25f;
	private CooldownTimer _mouthCloseDelayTimer;
	private CooldownTimer _blinkTimer;
	
	// State Effects
	[ExportCategory("State Effects")] 
	[Export] private PNGTuberEffectBase _quietEffect;
	[Export] private PNGTuberEffectBase _speakingEffect;
	[Export] private PNGTuberEffectBase _quietBlinkEffect;
	[Export] private PNGTuberEffectBase _speakingBlinkEffect;
	[Export] private PNGTuberEffectBase _screamEffect;
	
	// Preloaded Images
	private CompressedTexture2D _quietImage;
	private CompressedTexture2D _talkingImage;
	private CompressedTexture2D _quietBlinkImage;
	private CompressedTexture2D _talkingBlinkImage;
	private CompressedTexture2D _screamingImage;
	
	// Mic Monitoring
	private float _micLevel;
	private float _filteredMicLevel;
	
	// Simple Mic Filtering (Average this array)
	private Array<float> _audioLevelHistory = new Array<float>();
	
	// Node References
	private Sprite2D _avatarDisplay;
	
	// RNG
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	// Debug labels. Move to OptionsWindow later.
	private Label _rawLabel;
	private Label _filteredLabel;
	private Label _fpsLabel;
	
	// Current state (Default to quiet)
	public PngTuberEnum MicState = PngTuberEnum.Quiet;
	private PngTuberEnum _prevState = PngTuberEnum.Quiet;
	private bool _stateLocked = false;
	private int _stateLockFrameTimer = 0;
	private int _stateLockFrameDuration = 15;
	
	// ==================================================================================
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Avatar is null)
		{
			GD.Print("PNGTuber.cs: Failed to load basic avatar. Make sure you set an avatar in the Export properties.");
			QueueFree();
		}
		SetupAvatar(Avatar);
		AudioServer.InputDevice = "Wave Link MicrophoneFX (Elgato Wave:3)";
		_rawLabel = GetNode<Label>("VBoxContainer/RawLabel");
		_filteredLabel = GetNode<Label>("VBoxContainer/FilteredLabel");
		_fpsLabel = GetNode<Label>("VBoxContainer/FPSLabel");
		_avatarDisplay = GetNode<Sprite2D>("Sprite2D");
		_mouthCloseDelayTimer = new CooldownTimer(_mouthCloseDelaySeconds);
		_mouthCloseDelayTimer.ResetCooldown();
		_blinkTimer = new CooldownTimer(1);
		_blinkTimer.ResetCooldown();
		
		// Center image
		_avatarDisplay.Position = _avatarDisplay.GetParent<Window>().Size / 2;
	}

	public void SetupAvatar(PNGTuberAvatarResource _avatar)
	{
		_quietImage = _avatar.QuietImage;
		_talkingImage = _avatar.TalkingImage;
		_quietBlinkImage = _avatar.QuietBlinkImage;
		_talkingBlinkImage = _avatar.TalkingBlinkImage;
		_screamingImage = _avatar.ScreamImage;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_micLevel = AudioServer.GetBusPeakVolumeRightDb(AudioServer.GetBusIndex("MICROPHONE"), 0);
		_filteredMicLevel = -1000;
		if (_micLevel >= -40) // Mute threshold.
		{
			// Filter audio in case this is needed.
			if (_audioLevelHistory.Count > MicrophoneSmoothingSamples)
			{
				_audioLevelHistory.Resize(MicrophoneSmoothingSamples);
			}
			_audioLevelHistory.Add(_micLevel);
			_filteredMicLevel = _audioLevelHistory.Average();
			
		}
		// Clear history if we go silent, so we aren't filtering weird data.
		if (_micLevel < TalkThreshold)
		{
			_audioLevelHistory.Clear();
			_filteredMicLevel = -1000;
		}
		
		// State lock check
		if (_stateLocked)
		{
			_stateLockFrameTimer++;
			if (_stateLockFrameTimer >= _stateLockFrameDuration)
			{
				_stateLockFrameTimer = 0;
				_stateLocked = false;
			}
		}
		
		// State Machine Stuff.
		MicrophoneStateMachine();
		ProcessAvatarEffects();
		
		// Debug Label Text
		_filteredLabel.Text = "FilteredDB: " + _filteredMicLevel;
		_rawLabel.Text = "RawMic: " + _micLevel;
		_fpsLabel.Text = "FPS: " + Performance.GetMonitor(Performance.Monitor.TimeFps);
	}

	private void MicrophoneStateMachine()
	{
		if (_micLevel >= TalkThreshold)
		{
			_mouthCloseDelayTimer.ResetCooldown();
			// Reset because we are talking.
			
			if (_filteredMicLevel >= ScreamThreshold)
			{
				// SCREAM
				_blinkTimer.ResetCooldown(); // No blinking. Constantly reset while yelling.
				ChangeState(PngTuberEnum.Screaming);
				return;
			}
			if (_blinkTimer.HasCooldownElapsed())
			{
				// Blink while speaking.
				ChangeState(PngTuberEnum.SpeakingBlink);
				_blinkTimer = new CooldownTimer(RandomizeBlinkTime());
				_blinkTimer.ResetCooldown();
				return;
			}
			
			ChangeState(PngTuberEnum.Speaking);
		}

		// If we have stopped talking, and we can close our mouth.
		if (_micLevel < TalkThreshold && _mouthCloseDelayTimer.HasCooldownElapsed())
		{
			if (_blinkTimer.HasCooldownElapsed())
			{
				ChangeState(PngTuberEnum.QuietBlink);
				_blinkTimer = new CooldownTimer(RandomizeBlinkTime());
				_blinkTimer.ResetCooldown();
				return;
			}
			ChangeState(PngTuberEnum.Quiet);
		}
	}

	private void ChangeState(PngTuberEnum state)
	{
		if (!_stateLocked)
		{
			switch (state)
			{
				case PngTuberEnum.Quiet:
					_avatarDisplay.Texture = _quietImage;
					MicState = PngTuberEnum.Quiet;
					break;
				case PngTuberEnum.Speaking:
					_avatarDisplay.Texture = _talkingImage;
					MicState = PngTuberEnum.Speaking;
					break;
				case PngTuberEnum.QuietBlink:
					_avatarDisplay.Texture = _quietBlinkImage;
					_stateLockFrameDuration = 7;
					_stateLocked = true;
					MicState = PngTuberEnum.QuietBlink;
					break;
				case PngTuberEnum.SpeakingBlink:
					_avatarDisplay.Texture = _talkingBlinkImage;
					_stateLockFrameDuration = 7;
					_stateLocked = true;
					MicState = PngTuberEnum.SpeakingBlink;
					break;
				case PngTuberEnum.Screaming:
					_avatarDisplay.Texture = _screamingImage;
					_stateLockFrameDuration = 15;
					_stateLocked = true;
					MicState = PngTuberEnum.Screaming;
					break;
			}
		}
	}

	private void ProcessAvatarEffects()
	{
		if (MicState != _prevState) // If we are going to change states
		{
			GD.Print("Changing PNGTuber State");
			switch (_prevState) // Reset effects
			{
				case PngTuberEnum.Quiet:
					if (_quietEffect != null) _quietEffect.ResetEffect(_avatarDisplay);
					break;
				case PngTuberEnum.QuietBlink:
					if (_quietBlinkEffect != null) _quietBlinkEffect.ResetEffect(_avatarDisplay);
					break;
				case PngTuberEnum.Speaking:
					if (_speakingEffect != null) _speakingEffect.ResetEffect(_avatarDisplay);
					break;
				case PngTuberEnum.SpeakingBlink:
					if (_speakingBlinkEffect != null) _speakingBlinkEffect.ResetEffect(_avatarDisplay);
					break;
				case PngTuberEnum.Screaming:
					if (_screamEffect != null) _screamEffect.ResetEffect(_avatarDisplay);
					break;
			}
		}
		switch(MicState) // Apply new effects (or keep applying them)
		// Then set the previous state to the current state.
		{
			case PngTuberEnum.Quiet:
				if (_quietEffect != null) _quietEffect.ProcessEffect(_avatarDisplay);
				_prevState = MicState;
				break;
			case PngTuberEnum.QuietBlink:
				if (_quietBlinkEffect != null) _quietBlinkEffect.ProcessEffect(_avatarDisplay);
				_prevState = MicState;
				break;
			case PngTuberEnum.Speaking:
				if (_speakingEffect != null) _speakingEffect.ProcessEffect(_avatarDisplay);
				_prevState = MicState;
				break;
			case PngTuberEnum.SpeakingBlink:
				if (_speakingBlinkEffect != null) _speakingBlinkEffect.ProcessEffect(_avatarDisplay);
				_prevState = MicState;
				break;
			case PngTuberEnum.Screaming:
				if (_screamEffect != null) _screamEffect.ProcessEffect(_avatarDisplay);
				_prevState = MicState;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	private float RandomizeBlinkTime()
	{
		float value = Avatar.BlinkInterval + (Avatar.BlinkIntervalRandomization * _rng.RandfRange(-1, 1));
		GD.Print(value);
		return value;
	}
}
