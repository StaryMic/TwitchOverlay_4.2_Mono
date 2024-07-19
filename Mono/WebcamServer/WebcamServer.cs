using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;
using OpenCvSharp;

namespace TwitchOverlay.Mono.WebcamServer;
[GlobalClass]
public partial class WebcamServer : Node
{
    public int CameraIndex = 1;
    
    public VideoCapture Capture = VideoCapture.FromCamera(1);
    private static Mat _image = new Mat();

    private byte[] _byteData;
    public Image CameraImage = new Image();
    public ImageTexture CamTexture;

    private bool _statustracker;
    public override void _Ready()
    {
        Capture.ConvertRgb = true;
        Capture.Open(CameraIndex,VideoCaptureAPIs.DSHOW); //Opens camera at index 0... whatever that index is is unknown.
        CamTexture = new ImageTexture();
    }

    public override void _ExitTree()
    {
        Capture.Release();
    }

    public void ToggleCamera()
    {
        if (Capture.IsOpened())
        {
            Capture.Release();
        }
        else
        {
            Capture.Open(CameraIndex, VideoCaptureAPIs.DSHOW);
        }
    }

    public override void _Process(double delta)
    {
        if (Capture.IsOpened())
        {
            // Send signals to appropriate places.
            ChangeWebcamStatus(true);
            
            if (_image != null)
            {
                if (Capture.Read(_image))
                {
                    Mat appliedEffectsMat = ProcessEffectList(_image); //Apply effects
                    
                    _byteData = new byte[appliedEffectsMat.Width * appliedEffectsMat.Height * appliedEffectsMat.Channels()];
                    // GD.Print(_image.Width,"X",_image.Height);
                    Marshal.Copy(appliedEffectsMat.CvtColor(ColorConversionCodes.BGR2RGB).Data, _byteData, 0, _byteData.Length);
                    CameraImage.SetData(Capture.FrameWidth, Capture.FrameHeight, false, Image.Format.Rgb8, _byteData);
                    
                    CamTexture.SetImage(CameraImage);
                }
            }
        }

        if (!Capture.IsOpened())
        {
            ChangeWebcamStatus(false);
        }
    }
    
    //Signal stuff
    
    [Signal]
    public delegate void WebcamConnectionStatusChangeEventHandler(bool status);

    private void ChangeWebcamStatus(bool Status)
    {
        if (_statustracker != Status)
        {
            EmitSignal(SignalName.WebcamConnectionStatusChange, Status);
            _statustracker = Status;
        }
    }

    public ImageTexture GetWebcamTexture()
    {
        return CamTexture;
    }



    [Export] private Array<OpenCVEffect.OpenCVEffect> _cvEffects;
    
    private Mat ProcessEffectList(Mat inputMat)
    {
        Mat storedMat = inputMat; //Store the input Mat.
        // _storedMat is used for every effect, acting as the output of the effect, passing it onto the next effect that needs it
        foreach (OpenCVEffect.OpenCVEffect effect in _cvEffects)
        {
            storedMat = effect.ProcessEffect(storedMat);
        }

        return storedMat; //This will be the culmination of every effect put into one Mat.
    }
}