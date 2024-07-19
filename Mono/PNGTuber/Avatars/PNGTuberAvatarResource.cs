using Godot;

[GlobalClass]
public partial class PNGTuberAvatarResource : Resource
{
    [ExportCategory("Avatar Images")]
    [Export] public CompressedTexture2D QuietImage;
    [Export] public CompressedTexture2D TalkingImage;
    [Export] public CompressedTexture2D QuietBlinkImage;
    [Export] public CompressedTexture2D TalkingBlinkImage;
    [Export] public CompressedTexture2D ScreamImage;

    [ExportCategory("Avatar Parameters")]
    [Export] public float BlinkInterval;
    [Export] public float BlinkIntervalRandomization;
}
