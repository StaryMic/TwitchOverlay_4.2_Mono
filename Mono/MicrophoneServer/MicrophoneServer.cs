using Godot;
using System;
using AstroRaider2.Utility;
using AstroRaider2.Utility.NodeTree;

[GlobalClass]
public partial class MicrophoneServer : Node
{
	private AudioEffectCapture _micCapture;
	private AudioEffectRecord _micRecorder;
	private NodeRef<AudioStreamPlayer> _audioPlayer;

	private int effectIndex = AudioServer.GetBusIndex("MICROPHONE");
	public override void _Ready()
	{
		_micRecorder = (AudioEffectRecord)AudioServer.GetBusEffect(effectIndex, 1);
		_micCapture = (AudioEffectCapture)AudioServer.GetBusEffect(effectIndex, 0);
		_audioPlayer = new NodeRef<AudioStreamPlayer>(GetTree().Root, "@AudioStreamPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(_micCapture.GetFramesAvailable());
		GD.Print(_micCapture.GetBufferLengthFrames());
		GD.Print(_micCapture.GetBuffer(_micCapture.GetFramesAvailable()).Stringify());
		GD.Print(_micCapture.CanGetBuffer(128));
	}
}
