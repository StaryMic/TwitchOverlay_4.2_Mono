using Godot;
using System;
using TwitchOverlay.Mono.WebcamServer;

public partial class Sprite2D_TextureGrabTest : Sprite2D
{
    private WebcamServer cameraServer;
    public override void _Ready()
    {
        cameraServer = GetTree().Root.GetNode<WebcamServer>("Overlay/WebcamServer");
        this.Texture = cameraServer.GetWebcamTexture();
    }

    public override void _Process(double delta)
    {
        if (this.Texture == null)
        {
            this.Texture = cameraServer.GetWebcamTexture();
        }
    }
}
