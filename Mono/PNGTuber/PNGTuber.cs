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
	[Export] public PNGTuberAvatarResource _Avatar;
	[Export] public int MicrophoneSmoothingSamples = 25;

	[ExportCategory("Thresholds")]
	[Export] private float _talkThreshold = 0f;
	[Export] private float _screamThreshold = 0f;
	[Export] private float _mouthCloseDelaySeconds = 0.5f;
	private CooldownTimer _mouthCloseDelayTimer;
	private CooldownTimer _blinkTimer;
	
	// Preloaded Images
	private CompressedTexture2D QuietImage;
	private CompressedTexture2D TalkingImage;
	private CompressedTexture2D QuietBlinkImage;
	private CompressedTexture2D TalkingBlinkImage;
	private CompressedTexture2D ScreamingImage;
	
	// Mic Monitoring
	private float _micLevel;
	private float _filteredMicLevel;
	
	// Simple Mic Filtering (Average this array)
	private Array<float> AudioLevelHistory = new Array<float>();
	
	// Node References
	private TextureRect _avatarDisplay;
	
	// RNG
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	// Debug labels. Move to OptionsWindow later.
	private Label _rawLabel;
	private Label _filteredLabel;
	
	// Current state (Default to quiet)
	public PngTuberEnum MicState = PngTuberEnum.Quiet;
	private bool stateLocked = false;
	private int stateLockFrameTimer = 0;
	
	// ==================================================================================
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (_Avatar is null)
		{
			GD.Print("PNGTuber.cs: Failed to load basic avatar. Make sure you set an avatar in the Export properties.");
			QueueFree();
		}
		SetupAvatar(_Avatar);
		AudioServer.InputDevice = "Wave Link MicrophoneFX (Elgato Wave:3)";
		_rawLabel = GetNode<Label>("VBoxContainer/RawLabel");
		_filteredLabel = GetNode<Label>("VBoxContainer/FilteredLabel");
		_avatarDisplay = GetNode<TextureRect>("CenterContainer/TextureRect");
		_mouthCloseDelayTimer = new CooldownTimer(_mouthCloseDelaySeconds);
		_mouthCloseDelayTimer.ResetCooldown();
		_blinkTimer = new CooldownTimer(1);
		_blinkTimer.ResetCooldown();
	}

	public void SetupAvatar(PNGTuberAvatarResource _avatar)
	{
		QuietImage = _avatar.QuietImage;
		TalkingImage = _avatar.TalkingImage;
		QuietBlinkImage = _avatar.QuietBlinkImage;
		TalkingBlinkImage = _avatar.TalkingBlinkImage;
		ScreamingImage = _avatar.ScreamImage;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_micLevel = AudioServer.GetBusPeakVolumeRightDb(AudioServer.GetBusIndex("MICROPHONE"), 0);
		_filteredMicLevel = -1000;
		if (_micLevel >= -40) // Mute threshold.
		{
			// Filter audio in case this is needed.
			if (AudioLevelHistory.Count > MicrophoneSmoothingSamples)
			{
				AudioLevelHistory.Resize(MicrophoneSmoothingSamples);
			}
			AudioLevelHistory.Add(_micLevel);
			_filteredMicLevel = AudioLevelHistory.Average();
			
		}
		// Clear history if we go silent, so we aren't filtering weird data.
		if (_micLevel < _talkThreshold)
		{
			AudioLevelHistory.Clear();
			_filteredMicLevel = -1000;
		}
		
		// State lock check
		if (stateLocked)
		{
			stateLockFrameTimer++;
			if (stateLockFrameTimer >= 10)
			{
				stateLockFrameTimer = 0;
				stateLocked = false;
			}
		}
		
		// State Machine Stuff.
		MicrophoneStateMachine();
		
		// Debug Label Text
		_filteredLabel.Text = "FilteredDB: " + _filteredMicLevel;
		_rawLabel.Text = "RawMic: " + _micLevel;
	}

	private void MicrophoneStateMachine()
	{
		if (_filteredMicLevel >= _talkThreshold)
		{
			_mouthCloseDelayTimer.ResetCooldown();
			// Reset because we are talking.
			
			if (_filteredMicLevel >= _screamThreshold)
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
		if (_filteredMicLevel < _talkThreshold && _mouthCloseDelayTimer.HasCooldownElapsed())
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
		if (!stateLocked)
		{
			switch (state)
			{
				case PngTuberEnum.Quiet:
					_avatarDisplay.Texture = QuietImage;
					break;
				case PngTuberEnum.Speaking:
					_avatarDisplay.Texture = TalkingImage;
					break;
				case PngTuberEnum.QuietBlink:
					_avatarDisplay.Texture = QuietBlinkImage;
					stateLocked = true;
					break;
				case PngTuberEnum.SpeakingBlink:
					_avatarDisplay.Texture = TalkingBlinkImage;
					stateLocked = true;
					break;
				case PngTuberEnum.Screaming:
					_avatarDisplay.Texture = ScreamingImage;
					break;
			}
		}
	}

	private float RandomizeBlinkTime()
	{
		float value = _Avatar.BlinkInterval + (_Avatar.BlinkIntervalRandomization * _rng.RandfRange(-1, 1));
		GD.Print(value);
		return value;
	}
}
