using Godot;
using System;
using TwitchOverlay.Mono;
using TwitchOverlay.Mono.Twitch;

[GlobalClass]
public partial class ChatCommandHandler : Node
{
	// Global Scene Signal Node
	private GlobalSceneSignals _globalSceneSignals;
	
	// External Nodes
	private GoveeLightHandler _goveeLightHandler;
	private ChatHandler _chatHandler;
	private TwitchAPI _twitchApi;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_globalSceneSignals = GetTree().Root.GetChild(0).GetNode<GlobalSceneSignals>("GlobalSceneSignals");
		_chatHandler = GetTree().Root.GetChild(0).GetNode<ChatHandler>("ChatHandler");
		_goveeLightHandler = GetTree().Root.GetChild(0).GetNode<GoveeLightHandler>("GoveeLightHandler");
		_twitchApi = GetTree().Root.GetChild(0).GetNode<TwitchAPI>("TwitchAPI");
		
		_globalSceneSignals.CommandMessage += HandleChatCommands;
	}

	private void HandleChatCommands(string username, string message, string userid, string messageid)
	{
		String[] splitMessage = message.Split(" ", StringSplitOptions.TrimEntries);
		switch (splitMessage[0])
		{
			case "!LEDColor":
				GD.Print("ChatCommandHandler.cs: Got LEDColor");
				int R;
				int G;
				int B;
				
				if (int.TryParse(splitMessage[1], out R) &&
				    int.TryParse(splitMessage[2], out G) &&
				    int.TryParse(splitMessage[3], out B) &&
				    splitMessage.Length == 4)
				{
					_goveeLightHandler.ChangeColor(R,G,B);
				}
				else
				{
					_twitchApi.SendChatMessage("[BOT]: Incorrect arguments. Check your parameters and try again.", messageid);
				}
				break;
			
			case "!Flashbang":
				GD.Print("Flashbang going out!");
				_goveeLightHandler.Flashbang();
				break;
		}
	}
}
