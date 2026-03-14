using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine;

public class ConductorDetector : MonoBehaviour
{
    [SerializeField] protected Vector3 hsvLowerBounds;
    [SerializeField] protected Vector3 hsvUpperBounds = new(180, 255, 255);
    [SerializeField] protected DisplayWebCam webcam;
    
    public int wandX { get; protected set; }
    public int wandY { get; protected set; }

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
    }

    protected Mat GetMaskedImage()
    {
        Mat rgbMat = webcam.GetRGBCameraImageMatrix();
        Mat hsvMat = new Mat();
        Mat mask = new Mat();
        
        Imgproc.cvtColor(rgbMat, hsvMat, Imgproc.COLOR_RGB2HSV);

        Scalar lower = new Scalar(hsvLowerBounds.x, hsvLowerBounds.y, hsvLowerBounds.z);
        Scalar upper = new Scalar(hsvUpperBounds.x, hsvUpperBounds.y, hsvUpperBounds.z);
        Core.inRange(hsvMat, lower, upper, mask);
        
        hsvMat.Dispose();
        return mask;
    }

    // Update is called once per frame
    void Update()
    {
        Mat mask = GetMaskedImage();
        Debug.Log($"Mask is size {mask.rows()}x{mask.cols()}");
        foreach (int val in mask.AsSpan<int>())
        {
            if (val != 255) print("Got not white pixel");
        }
        MatOfKeyPoint keypointMat = new MatOfKeyPoint();
        blobDetector.detect(mask, keypointMat);
        KeyPoint[] keypoints = keypointMat.toArray();
        Debug.Log($"There are {keypoints.Length} keypoints");
        foreach (KeyPoint kp in keypoints)
        {
            Debug.Log($"Keypoint centered at ({kp.pt.x}, {kp.pt.y}) and size is {kp.size}");
        }
    }
}
