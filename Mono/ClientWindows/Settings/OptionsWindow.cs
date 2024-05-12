using Godot;
using System;
using TwitchOverlay.Mono;
using TwitchOverlay.Mono.WebcamServer;

public partial class OptionsWindow : Window
{
	//================================================
	// Options Menu references
	//  - Debug
	private Button _connectButton;
	private Button _disconnectButton;
	private Button _authKeyButton;
	private Button _testFollowButton;
	//  - Devices
	//		- Audio
	private OptionButton _inputOptions;
	private OptionButton _outputOptions;
	//		- Video
	private SpinBox _camIndexSpinner;
	private Button _resetCamButton;
	//  - Signals
	//		- Global
	private Button _rbplusResetButton;
	//		- Fixes
	private Button _forceEndPredictionButton;
	//  - Toggles
	private Button _toggleCameraButton;
	private Button _toggleBitCupButton;
	private Button _toggleChatboxButton;
	private Button _toggleNotificationsButton;
	//================================================
	
	//================================================
	// Outside Node References
	private GlobalSceneSignals _globalSceneSignalsRef;
	private TwitchAPI _twitchApiRef;
	private WebcamServer _webcamServerRef;
	private ChatboxBase _chatboxBaseRef;
	private EventQueue _eventQueueRef;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Set up outside references
		_globalSceneSignalsRef = GetTree().Root.GetChild(0).GetNode<GlobalSceneSignals>("GlobalSceneSignals");
		_twitchApiRef = GetTree().Root.GetChild(0).GetNode<TwitchAPI>("TwitchAPI");
		_webcamServerRef = GetTree().Root.GetChild(0).GetNode<WebcamServer>("WebcamServer");
		_chatboxBaseRef = GetTree().Root.GetChild(0).GetNode<ChatboxBase>("ChatboxBase");
		_eventQueueRef = GetTree().Root.GetChild(0).GetNode<EventQueue>("EventQueue");

		this.Unfocusable = false;
		


		// Options Menu references
		//  - Debug
		_connectButton = this.GetNode<Button>("SettingsTabs/Debug/Connect");
		_connectButton.Pressed += () => _twitchApiRef._websocketClient.DisconnectAsync().Start();
		_disconnectButton = this.GetNode<Button>("SettingsTabs/Debug/Disconnect");
		_disconnectButton.Pressed += () => _twitchApiRef._websocketClient.ConnectAsync().Start();
		_authKeyButton = this.GetNode<Button>("SettingsTabs/Debug/TokenGen");
		_authKeyButton.Pressed += () => _twitchApiRef.GenerateNewToken();
		
		//  - Devices
		//		- Audio
		_inputOptions = this.GetNode<OptionButton>("SettingsTabs/Devices/Audio/InputOptions");

		foreach (var input in AudioServer.GetInputDeviceList())
		{
			_inputOptions.AddItem(input);
		}

		_inputOptions.ItemSelected += index => { AudioServer.InputDevice = _inputOptions.GetItemText((int)index); };

		_outputOptions = this.GetNode<OptionButton>("SettingsTabs/Devices/Audio/OutputOptions");
		foreach (var output in AudioServer.GetOutputDeviceList())
		{
			_outputOptions.AddItem(output);
		}

		_outputOptions.ItemSelected += index => { AudioServer.OutputDevice = _outputOptions.GetItemText((int)index); };
		AudioServer.OutputDevice = "Wave Link SFX (Elgato Wave:3)";
		
		//		- Video
		_camIndexSpinner = this.GetNode<SpinBox>("SettingsTabs/Devices/Video/CamIndexSpinner");
		_camIndexSpinner.ValueChanged += value => _webcamServerRef.CameraIndex = (int)value; // Set Camera Index
		_resetCamButton = this.GetNode<Button>("SettingsTabs/Devices/Video/ResetCam");
		_resetCamButton.Pressed += () => _webcamServerRef.ToggleCamera(); // Toggle Camera
		//  - Signals
		//		- Global
		_rbplusResetButton = GetNode<Button>("SettingsTabs/Signals/Global/RBPReset");
		_rbplusResetButton.Pressed += () => _globalSceneSignalsRef.EmitSignal(GlobalSceneSignals.SignalName.ResetPhysicsObjectsToInitialPosition);
		//		- Fixes
		_forceEndPredictionButton = GetNode<Button>("SettingsTabs/Signals/Fixes/ForceEndPrediction");
		_forceEndPredictionButton.Pressed += () =>
			_globalSceneSignalsRef.EmitSignal(GlobalSceneSignals.SignalName.ForceEndPrediction);
		//  - Toggles
		_toggleCameraButton = GetNode<Button>("SettingsTabs/Toggles/ToggleCamera");
		_toggleCameraButton.Pressed += () => _webcamServerRef.ToggleCamera();
		
		_toggleBitCupButton = GetNode<Button>("SettingsTabs/Toggles/ToggleBitCup");
		
		_toggleChatboxButton = GetNode<Button>("SettingsTabs/Toggles/ToggleChatbox");
		_toggleChatboxButton.Pressed += () => _chatboxBaseRef.Visible = !_chatboxBaseRef.Visible;
		
		_toggleNotificationsButton = GetNode<Button>("SettingsTabs/Toggles/ToggleNotifications");
		_toggleNotificationsButton.Pressed += () => _eventQueueRef.EmitSignal(EventQueue.SignalName.ToggleQueue);

		//================================================
	}
}
