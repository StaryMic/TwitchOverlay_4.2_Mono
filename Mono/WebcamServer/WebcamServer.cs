using System.Runtime.InteropServices;
using Godot;
using OpenCvSharp;
using OpenCvSharp.Tracking;
using OpenCvSharp.Face;
using OpenCvSharp.XImgProc;

namespace TwitchOverlay.Mono.WebcamServer;
[GlobalClass]
public partial class WebcamServer : Node
{
    
    public VideoCapture _capture = VideoCapture.FromCamera(1);
    private static Mat _image = new Mat();

    private byte[] ByteData;
    public Image CameraImage = new Image();
    public ImageTexture CamTexture;

    public override void _Ready()
    {
        _capture.ConvertRgb = true;
        _capture.Open(1,VideoCaptureAPIs.DSHOW); //Opens camera at index 0... whatever that index is is unknown.
        CamTexture = new ImageTexture();
    }

    public override void _Process(double delta)
    {
        if (_capture.IsOpened())
        {
            if (_image != null)
            {
                if (_capture.Read(_image))
                {
                    ByteData = new byte[_image.Width * _image.Height * _image.Channels()];
                    GD.Print($"Depth: {_capture.RetrieveMat().Depth()}");
                    GD.Print(_image.Channels());
                    Marshal.Copy(_image.CvtColor(ColorConversionCodes.BGR2RGB).Data, ByteData, 0, ByteData.Length);
                    CameraImage.SetData(_capture.FrameWidth, _capture.FrameHeight, false, Image.Format.Rgb8, ByteData);
                    
                    CamTexture.SetImage(CameraImage);
                }
            }
        }
    }

    public ImageTexture GetWebcamTexture()
    {
        return CamTexture;
    }
}