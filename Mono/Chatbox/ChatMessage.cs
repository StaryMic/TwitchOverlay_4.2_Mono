using Godot;
using System;
using AstroRaider2.Utility.NodeTree;

public partial class ChatMessage : PanelContainer
{
	public String Username;
	public String Message;

	public NodeRef<RichTextLabel> UsernameLabel;
	public NodeRef<RichTextLabel> MessageLabel;
	public NodeRef<Timer> Timer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set up references
		UsernameLabel = new NodeRef<RichTextLabel>(this, "@Username");
		MessageLabel = new NodeRef<RichTextLabel>(this, "@Message");
		Timer = new NodeRef<Timer>(this, "@KillTimer");
		
		// Set text
		UsernameLabel.Node.Text = Username;
		MessageLabel.Node.Text = Message;

		// Die after timer is done
		Timer.Node.Timeout += () => FadeOutAndDie();

	}

	private void FadeOutAndDie()
	{
		Tween _tween = CreateTween();
		_tween.SetEase(Tween.EaseType.In);
		_tween.SetTrans(Tween.TransitionType.Expo);
		_tween.BindNode(this);
		_tween.TweenProperty(this, "modulate:a", 0, 2);
		_tween.TweenCallback(Callable.From(QueueFree));

	}
}
