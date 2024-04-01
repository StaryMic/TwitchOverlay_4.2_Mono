using System;
using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;
using OpenCvSharp;

namespace TwitchOverlay.Mono.WebcamServer;
[GlobalClass]
public partial class WebcamServer : Node
{
    
    public VideoCapture _capture = VideoCapture.FromCamera(1);
    private static Mat _image = new Mat();

    private byte[] ByteData;
    public Image CameraImage = new Image();
    public ImageTexture CamTexture;

    private bool statustracker;
    public override void _Ready()
    {
        _capture.ConvertRgb = true;
        _capture.Open(1,VideoCaptureAPIs.DSHOW); //Opens camera at index 0... whatever that index is is unknown.
        CamTexture = new ImageTexture();
    }

    public override void _ExitTree()
    {
        _capture.Release();
    }

    public void ToggleCamera()
    {
        if (_capture.IsOpened())
        {
            _capture.Release();
        }
        else
        {
            _capture.Open(1, VideoCaptureAPIs.DSHOW);
        }
    }

    public override void _Process(double delta)
    {
        if (_capture.IsOpened())
        {
            // Send signals to appropriate places.
            ChangeWebcamStatus(true);
            
            if (_image != null)
            {
                if (_capture.Read(_image))
                {
                    Mat appliedEffectsMat = ProcessEffectList(_image); //Apply effects
                    
                    ByteData = new byte[appliedEffectsMat.Width * appliedEffectsMat.Height * appliedEffectsMat.Channels()];
                    // GD.Print(_image.Width,"X",_image.Height);
                    Marshal.Copy(appliedEffectsMat.CvtColor(ColorConversionCodes.BGR2RGB).Data, ByteData, 0, ByteData.Length);
                    CameraImage.SetData(_capture.FrameWidth, _capture.FrameHeight, false, Image.Format.Rgb8, ByteData);
                    
                    CamTexture.SetImage(CameraImage);
                }
            }
        }

        if (!_capture.IsOpened())
        {
            ChangeWebcamStatus(false);
        }
    }
    
    //Signal stuff
    
    [Signal]
    public delegate void WebcamConnectionStatusChangeEventHandler(bool status);

    private void ChangeWebcamStatus(bool Status)
    {
        if (statustracker != Status)
        {
            EmitSignal(SignalName.WebcamConnectionStatusChange, Status);
            statustracker = Status;
        }
    }

    public ImageTexture GetWebcamTexture()
    {
        return CamTexture;
    }



    [Export] private Array<OpenCVEffect.OpenCVEffect> _cvEffects;
    
    private Mat ProcessEffectList(Mat _inputMat)
    {
        Mat _storedMat = _inputMat; //Store the input Mat.
        // _storedMat is used for every effect, acting as the output of the effect, passing it onto the next effect that needs it
        foreach (OpenCVEffect.OpenCVEffect effect in _cvEffects)
        {
            _storedMat = effect.ProcessEffect(_storedMat);
        }

        return _storedMat; //This will be the culmination of every effect put into one Mat.
    }
}