using System;
using UnityEngine;


public class DmxTester : MonoBehaviour
{
    public DmxSender dmx;
    void Start()
    {
        dmx.PlayCue(Cue.Blackout);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            dmx.PlayCue(Cue.Wait);   
        }
        // byte intensity = (byte)Mathf.PingPong(Time.time * 255f, 255f);
        // dmxData[0] = intensity;
    }
}
