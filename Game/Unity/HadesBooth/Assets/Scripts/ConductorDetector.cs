using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.ImgprocModule;
using UnityEngine;

public class ConductorDetector : MonoBehaviour
{
    [SerializeField] protected Vector3 hsvLowerBounds;
    [SerializeField] protected Vector3 hsvUpperBounds = new(180, 255, 255);
    [SerializeField] protected DisplayWebCam webcam;
    [SerializeField] protected RectTransform dot;

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
        if (!webcam.IsReady) return;
        Mat mask = GetMaskedImage();
        MatOfKeyPoint keypointMat = new MatOfKeyPoint();
        blobDetector.detect(mask, keypointMat);
        KeyPoint[] keypoints = keypointMat.toArray();
        Debug.Log($"There are {keypoints.Length} keypoints");
        if (keypoints.Length == 0)
        {
            dot.position = new Vector3(-5000, -5000, 0);
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
        float dotPositionX = ((float)largest.pt.x)/1280*800;
        float dotPositionY = (-(float)largest.pt.y)/960*450 + 450;
        dot.position = new Vector3(dotPositionX, dotPositionY, 0);
    }
}
