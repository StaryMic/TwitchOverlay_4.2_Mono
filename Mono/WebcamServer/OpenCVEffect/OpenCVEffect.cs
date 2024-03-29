using Godot;
using OpenCvSharp;
using System;

namespace TwitchOverlay.Mono.WebcamServer.OpenCVEffect;
[GlobalClass]
public partial class OpenCVEffect : Node
{
    public virtual Mat ProcessEffect(Mat inputMat)
    {
        return new Mat();
    }
}
