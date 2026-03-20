/*
Lighting controller Demo
Uses https://github.com/sugi-cho/ArtNet.Unity/blob/master/README.md

dmxData (byte array) is repeatedly sent over ArtNet (on 127.0.0.1) to QLC+ (to ENTTEC Open DMX USB)

By: Taylor Roberts
*/

using UnityEngine;


// testing lighting script behavior, will improve later - Taylor
public class DmxSender : MonoBehaviour
{
    public DmxController controller;
    byte[] dmxData = new byte[512];

    void Start()
    {
        dmxData[1] = 255;
    }


    void Update()
    {
        byte intensity = (byte)Mathf.PingPong(Time.time * 255f, 255f);
        dmxData[0] = intensity;
        controller.Send(0, dmxData);
    }
}
