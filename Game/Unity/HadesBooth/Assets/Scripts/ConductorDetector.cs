using System.Collections;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.UnityUtils;

public class ConductorDetector : MonoBehaviour
{
    [SerializeField] protected Vector3 hsvLowerBounds;
    [SerializeField] protected Vector3 hsvUpperBounds = new(180, 255, 255);
    [SerializeField] protected float minCircularity = 0f;
    [SerializeField] protected float maxCircularity = 1f;
    [SerializeField] protected float minInertia = 0f;
    [SerializeField] protected float maxInertia = 1f;
    [SerializeField] protected int minArea = 1;
    [SerializeField] protected int maxArea = int.MaxValue;
    [SerializeField] protected DisplayWebCam webcam;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected RectTransform dot;
    [SerializeField] protected RawImage maskedImage;
    [SerializeField] protected RectTransform maskDot;
    protected Image dotImage;
    protected Image maskedDotImage;
    protected Texture2D maskedTexture;
    protected RectTransform dotParent;
    protected RectTransform maskDotParent;

    public Note? currentNote { get; protected set; }

    protected SimpleBlobDetector blobDetector;

    protected void Start()
    {
        Utils.setDebugMode(true, true);
        SimpleBlobDetector_Params param = GetBlobParams();
        blobDetector = SimpleBlobDetector.create(param);
        
        if (!dot?.TryGetComponent(out dotImage) ?? false) Debug.LogWarning("Conductor dot does not have an image");
        if (!maskDot?.TryGetComponent(out maskedDotImage) ?? false) Debug.LogWarning("Conductor mask dot does not have an image");
        if (!dot?.transform.parent.TryGetComponent(out dotParent) ?? false) Debug.LogWarning("Dot's parent has no rect transform");
        if (!maskDot?.transform.parent.TryGetComponent(out maskDotParent) ?? false) Debug.LogWarning("Mask dot's parent has no rect transform");
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

    protected SimpleBlobDetector_Params GetBlobParams()
    {
        SimpleBlobDetector_Params param = new SimpleBlobDetector_Params();
        
        param.set_filterByArea(minArea > 0 || maxArea < int.MaxValue);
        param.set_minArea(minArea);
        param.set_maxArea(maxArea);
        
        param.set_filterByCircularity(minCircularity > 0f || maxCircularity < 1f);
        param.set_minCircularity(minCircularity == 0f ? 0.001f : minCircularity);
        param.set_maxCircularity(maxCircularity);
        
        param.set_filterByInertia(minInertia > 0f || maxInertia < 1f);
        param.set_minInertiaRatio(minInertia == 0f ? 0.001f : minInertia);
        param.set_minInertiaRatio(maxInertia);
        
        param.set_filterByConvexity(false);
        param.set_filterByColor(true);
        param.set_blobColor(255);
        
        return param;
    }

    // Update is called once per frame
    void Update()
    {
        if (!webcam.IsReady) return;
        using Mat mask = GetMaskedImage();
        if (!maskedTexture) maskedTexture = new Texture2D(mask.width(), mask.height(), TextureFormat.R8, false);
        Utils.matToTexture2D(mask, maskedTexture);
        maskedImage.texture = maskedTexture;
        MatOfKeyPoint keypointMat = new MatOfKeyPoint();
        blobDetector.setParams(GetBlobParams());
        blobDetector.detect(mask, keypointMat);
        KeyPoint[] keypoints = keypointMat.toArray();
        // Debug.Log($"There are {keypoints.Length} keypoints");
        if (keypoints.Length == 0)
        {
            if (dot) dot.position = new Vector3(-5000, -5000, 0);
            if (maskDot) maskDot.position = new Vector3(-5000, -5000, 0);
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
            if (upperY) currentNote = Notes.Blue;
            else currentNote = Notes.Yellow;
        }
        else
        {
            if (upperY) currentNote = Notes.Red;
            else currentNote = Notes.Cyan;
        }
        
        DisplayDot(largest.pt, currentNote.Value.noteColor, dot, dotImage, dotParent);
        DisplayDot(largest.pt, currentNote.Value.noteColor, maskDot, maskedDotImage, maskDotParent);
    }

    void DisplayDot(Point imageFramePoint, NoteColor color, RectTransform dotRect, Image dotImage, RectTransform parentRect)
    {
        if (!dotRect || !dotImage) return;
        float webcamX = (float)imageFramePoint.x;
        float webcamY = (float)imageFramePoint.y;
        float canvasWidth = parentRect.rect.width;
        float canvasHeight = parentRect.rect.height;
        float canvasX = canvasWidth / webcam.width * webcamX - canvasWidth / 2f;
        float canvasY = -canvasHeight / webcam.height * webcamY + canvasHeight / 2f;
        dotRect.localPosition = new Vector3(canvasX, canvasY, 0);
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
