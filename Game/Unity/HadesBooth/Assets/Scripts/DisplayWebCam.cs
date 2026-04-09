using System;
using System.Collections;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class DisplayWebCam : MonoBehaviour
{
    [SerializeField] private int cameraWidth;
    [SerializeField] private int cameraHeight;
    [SerializeField] private int cameraToUse;

    public WebCamTexture webcamTex { get; protected set; }
    public bool IsReady { get; protected set; }
    public int width => webcamTex.width;
    public int height => webcamTex.height;

    protected Texture2D cachedTexture2d;
    protected Mat cachedRgbaImg;
    protected Mat cachedRgbImg;
    
    public RawImage rawImage { get; protected set; }
    
    IEnumerator Start()
    {
        IsReady = false;
        
        WebCamDevice[] devices = WebCamTexture.devices;
        string camName = devices[cameraToUse].name;
        Debug.Log($"Using camera {camName}");
        webcamTex = new WebCamTexture(camName, cameraWidth, cameraHeight);

        RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = webcamTex;
        
        webcamTex.Play();

        // Wait until camera initializes
        while (webcamTex.width < 100)
            yield return null;

        float aspect = (float)webcamTex.width / webcamTex.height;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(
            rectTransform.sizeDelta.y * aspect,
            rectTransform.sizeDelta.y
        );
        
        cachedRgbaImg = new Mat(webcamTex.height, webcamTex.width, CvType.CV_8UC4);
        cachedRgbImg = new Mat(webcamTex.height, webcamTex.width, CvType.CV_8UC3);
        cachedTexture2d = new Texture2D(webcamTex.width, webcamTex.height, TextureFormat.RGBA32, false);

        IsReady = true;
    }

    protected void Update()
    {
        // This is needed for the webcam to display on screen properly, not sure it's needed in general
        var request = AsyncGPUReadback.Request(webcamTex);
        request.WaitForCompletion();
    }
    
    public Texture2D GetCameraTexture2D()
    {
        if (!IsReady) return null;
        Utils.textureToTexture2D(webcamTex, cachedTexture2d);
        return cachedTexture2d;
    }

    public Mat GetRGBACameraImageMatrix()
    {
        if (!IsReady) return null;
        Utils.texture2DToMat(GetCameraTexture2D(), cachedRgbaImg);
        return cachedRgbaImg;
    }
    
    public Mat GetRGBCameraImageMatrix()
    {
        if (!IsReady) return null;
        Imgproc.cvtColor(GetRGBACameraImageMatrix(), cachedRgbImg, Imgproc.COLOR_RGBA2RGB);
        return cachedRgbImg;
    }
    
    protected void OnDestroy()
    {
        if (cachedTexture2d) Destroy(cachedTexture2d);
        cachedRgbaImg?.Dispose();
        cachedRgbImg?.Dispose();
    }
}
