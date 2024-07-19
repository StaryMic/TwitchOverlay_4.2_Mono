using Godot;
using OpenCvSharp;
using TwitchOverlay.Mono.WebcamServer.OpenCVEffect;

// https://dev.to/azure/opencv-detect-and-blur-faces-using-haar-cascades-in-c-1ao5
// Code mainly stolen from here.

[GlobalClass]
public partial class FaceTracking : OpenCVEffect
{
    private HaarDetectionTypes _haarDetectionTypes = HaarDetectionTypes.DoRoughSearch;
    private CascadeClassifier _faceCascade = new CascadeClassifier("Mono/WebcamServer/OpenCVEffect/haarcascade_frontalface_default.xml");
    public override Mat ProcessEffect(Mat inputMat)
    {
        Mat grayMat;
        
        grayMat = inputMat.CvtColor(ColorConversionCodes.RGB2GRAY);

        var faces = _faceCascade.DetectMultiScale(grayMat, 1.3, 5);

        foreach (var face in faces)
        {
            Cv2.Rectangle(inputMat,face,Scalar.Aquamarine,10);
        }

        return inputMat;
    }
    
}
