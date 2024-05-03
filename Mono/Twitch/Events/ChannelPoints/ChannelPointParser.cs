using Godot;
using System;
using OpenCvSharp;
using TwitchLib.Api.Helix.Models.Ads;
using TwitchOverlay.Mono;
using Window = Godot.Window;

[GlobalClass]
public partial class ChannelPointParser : Node
{
	private GlobalSceneSignals _globalSceneSignalsRef;
	private bool _globalSceneSignalFound = false;
	
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	// Outside references
	private TwitchAPI _twitchApiRef;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set up references
		_globalSceneSignalsRef = GetTree().Root.GetChild(0).GetNodeOrNull<GlobalSceneSignals>("GlobalSceneSignals");
		_twitchApiRef = GetTree().Root.GetChild(0).GetNode<TwitchAPI>("TwitchAPI");
		
		if (_globalSceneSignalsRef == null)
		{
			GD.Print("ChannelPointParser.cs: Failed to find GlobalSceneSignals.");
			_globalSceneSignalFound = false;
		}
		else
		{
			_globalSceneSignalFound = true;
		}

		if (_globalSceneSignalFound)
		{
			_globalSceneSignalsRef.ChannelPoint += GlobalSceneSignalsRefOnChannelPoint;
		}
	}

	private void GlobalSceneSignalsRefOnChannelPoint(string username, string rewardtitle, string userinput)
	{
		switch (rewardtitle)
		{
			case "Ad Time":
				StartCommercialRequest commercialRequest = new StartCommercialRequest();
				commercialRequest.Length = 30;
				commercialRequest.BroadcasterId = "121720350";
				_twitchApiRef.twitchApi.Helix.Ads.StartCommercialAsync(commercialRequest);
				break;
			
			case "Benadryl":
				// Roll a random number to determine if he appears.
				// Make sure he doesn't already exist
				if (_rng.RandiRange(0,25) == 14 && DoesNodeExistInSceneTree("EventObjects/TheHatMan") == false)
				{
					//Summon the Hat Man
					var TheHatMan = ResourceLoader.Load<PackedScene>("res://Mono/Twitch/Events/ChannelPoints/Benadryl/TheHatMan.tscn");
					Node2D TheHatManInstance = TheHatMan.Instantiate() as Node2D;
					GetTree().Root.GetChild(0).GetNode("EventObjects").AddChild(TheHatManInstance);
					TheHatManInstance.Position = new Vector2(910, 618);
				}
				break;
			
			case "Popup":
				OS.Alert(userinput, username);
				break;
			
			case "Hydrate!":
				break;
		}
	}
	
	// HELPER FUNCTIONS
	public bool DoesNodeExistInSceneTree(string NodePath)
	{
		if (GetTree().Root.GetChild(0).GetNodeOrNull(NodePath) == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
