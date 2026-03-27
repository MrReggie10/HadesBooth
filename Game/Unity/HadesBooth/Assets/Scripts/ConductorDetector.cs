using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine;
using UnityEngine.UI;

public class ConductorDetector : MonoBehaviour
{
    [SerializeField] protected Vector3 hsvLowerBounds;
    [SerializeField] protected Vector3 hsvUpperBounds = new(180, 255, 255);
    [SerializeField] protected DisplayWebCam webcam;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected RectTransform dot;
    protected Image dotImage;

    public Note? currentNote { get; protected set; }

    protected SimpleBlobDetector blobDetector;

    protected void Start()
    {
        SimpleBlobDetector_Params param = new SimpleBlobDetector_Params();
        param.set_filterByArea(false);
        param.set_filterByCircularity(false);
        param.set_filterByConvexity(false);
        param.set_filterByColor(true);
        param.set_blobColor(255);

        blobDetector = SimpleBlobDetector.create(param);
        
        if (!dot?.TryGetComponent(out dotImage) ?? false) Debug.LogWarning("Conductor dot does not have an image");
    }

    protected Mat GetMaskedImage()
    {
        Mat rgbMat = webcam.GetRGBCameraImageMatrix();
        using Mat hsvMat = new Mat();
        Mat mask = new Mat();
        
        Imgproc.cvtColor(rgbMat, hsvMat, Imgproc.COLOR_RGB2HSV);

        Scalar lower = new Scalar(hsvLowerBounds.x, hsvLowerBounds.y, hsvLowerBounds.z);
        Scalar upper = new Scalar(hsvUpperBounds.x, hsvUpperBounds.y, hsvUpperBounds.z);
        Core.inRange(hsvMat, lower, upper, mask);
        
        return mask;
    }

    // Update is called once per frame
    void Update()
    {
        if (!webcam.IsReady) return;
        using Mat mask = GetMaskedImage();
        MatOfKeyPoint keypointMat = new MatOfKeyPoint();
        blobDetector.detect(mask, keypointMat);
        KeyPoint[] keypoints = keypointMat.toArray();
        // Debug.Log($"There are {keypoints.Length} keypoints");
        if (keypoints.Length == 0)
        {
            dot.position = new Vector3(-5000, -5000, 0);
            currentNote = null;
            return;
        }
        KeyPoint largest = keypoints[0];
        foreach (KeyPoint kp in keypoints)
        {
            if(kp.size > largest.size)
            {
                largest = kp;
            }
            //Debug.Log($"Keypoint centered at ({kp.pt.x}, {kp.pt.y}) and size is {kp.size}");
        }

        bool leftX = largest.pt.x <= webcam.width / 2f;
        bool upperY = largest.pt.y <= webcam.height / 2f;
        if (leftX)
        {
            if (upperY) currentNote = Notes.Red;
            else currentNote = Notes.Cyan;
        }
        else
        {
            if (upperY) currentNote = Notes.Blue;
            else currentNote = Notes.Yellow;
        }
        
        DisplayDot(largest.pt, currentNote.Value.noteColor);
    }

    void DisplayDot(Point imageFramePoint, NoteColor color)
    {
        if (!dot || !canvas) return;
        float webcamX = (float)imageFramePoint.x;
        float webcamY = (float)imageFramePoint.y;
        float canvasWidth = canvas.pixelRect.width;
        float canvasHeight = canvas.pixelRect.height;
        float canvasX = canvasWidth / webcam.width * webcamX - canvasWidth / 2f;
        float canvasY = -canvasHeight / webcam.height * webcamY + canvasHeight / 2f;
        dot.localPosition = new Vector3(canvasX, canvasY, 0);
        if (dotImage)
        {
            switch (color)
            {
                case NoteColor.Red:
                    dotImage.color = Color.red;
                    break;
                case NoteColor.Blue:
                    dotImage.color = Color.blue;
                    break;
                case NoteColor.Cyan:
                    dotImage.color = Color.cyan;
                    break;
                case NoteColor.Yellow:
                    dotImage.color = Color.yellow;
                    break;
            }
        }
    }
}
