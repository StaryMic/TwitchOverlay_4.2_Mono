using Godot;
using System;
using AstroRaider2.Utility.NodeTree;
using Godot.Collections;
using TwitchOverlay.Mono;

public partial class ChatboxBase : Control
{
	private GlobalSceneSignals _globalSceneSignals;

	private PackedScene _chatMessagePackedScene;
	private NodeRef<VBoxContainer> _messageStorage;
	private NodeRef<AudioStreamPlayer> _chatSound;

	[Export] private Array<AudioStream> _sounds;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_messageStorage = new NodeRef<VBoxContainer>(this, "@Messages");
		
		_globalSceneSignals = GetTree().Root.GetChild<Node3D>(0).GetNode<GlobalSceneSignals>("./GlobalSceneSignals");
		_globalSceneSignals.ChatMessage += MessageSent;

		_chatSound = new NodeRef<AudioStreamPlayer>(this, "@ChatSound");

		_chatMessagePackedScene = ResourceLoader.Load<PackedScene>("res://Mono/Chatbox/ChatMessage.tscn");
		
	}
	
	public void MessageSent(string Username, string Message, string userId, string messageId)
	{
		var _chatMessageInstance = _chatMessagePackedScene.Instantiate();
		_chatMessageInstance.Set("Username",Username);
		_chatMessageInstance.Set("Message",Message);
		_chatMessageInstance.Name = messageId;
		_messageStorage.Node.AddChild(_chatMessageInstance);

		_chatSound.Node.Stream = _sounds.PickRandom();
		_chatSound.Node.Play();
	}
}
