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
        if (Input.GetKeyDown(KeyCode.B))
        {
            dmx.PlayCue(Cue.Blackout);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            dmx.PlayCue(Cue.Wait);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            dmx.PlayCue(Cue.GoodEnding);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            dmx.PlayCue(Cue.BadEnding);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            dmx.PlayCue(Cue.Spring0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            dmx.PlayCue(Cue.Spring1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            dmx.PlayCue(Cue.Spring2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            dmx.PlayCue(Cue.Spring3);
        }
    }
}
