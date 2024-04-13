using Godot;
using AstroRaider2.Utility.NodeTree;
using Godot.Collections;

public partial class EventHandler : Node
{
	private NodeRef<Node> _websocketClientReference;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_websocketClientReference = new NodeRef<Node>(GetTree().Root, "@WebsocketClient");

		_websocketClientReference.Node.Connect("send_notification_to_event_handler", ParseWebsocketMessage(JsonData));
	}

	private void ParseWebsocketMessage(Dictionary JsonInfo)
	{
		GD.Print(JsonInfo.ToString());
	}
}

oi shithead you were doing stuff here lmao.