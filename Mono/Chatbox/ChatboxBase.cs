using Godot;
using System;
using AstroRaider2.Utility.NodeTree;
using TwitchOverlay.Mono;

public partial class ChatboxBase : Control
{
	private GlobalSceneSignals _globalSceneSignals;

	private PackedScene _chatMessagePackedScene;
	private NodeRef<VBoxContainer> _messageStorage;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_messageStorage = new NodeRef<VBoxContainer>(this, "@Messages");
		
		_globalSceneSignals = GetTree().Root.GetChild<Node3D>(0).GetNode<GlobalSceneSignals>("./GlobalSceneSignals");
		_globalSceneSignals.ChatMessage += MessageSent;

		_chatMessagePackedScene = ResourceLoader.Load<PackedScene>("res://Mono/Chatbox/ChatMessage.tscn");
		
	}
	
	public void MessageSent(string Username, string Message, string userId, string messageId)
	{
		var _chatMessageInstance = _chatMessagePackedScene.Instantiate();
		_chatMessageInstance.Set("Username",Username);
		_chatMessageInstance.Set("Message",Message);
		_chatMessageInstance.Name = messageId;
		_messageStorage.Node.AddChild(_chatMessageInstance);
		//Fix this later make it work pls
	}
}
