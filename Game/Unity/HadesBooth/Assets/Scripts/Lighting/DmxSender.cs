/*
Lighting controller Demo
Uses https://github.com/sugi-cho/ArtNet.Unity/blob/master/README.md

dmxData (byte array) is repeatedly sent over ArtNet (on 127.0.0.1) to QLC+ (to ENTTEC Open DMX USB)

By: Taylor Roberts
*/

using System;
using UnityEngine;

public enum Cue {Blackout, Wait, GoodEnding, BadEnding, Spring0, Spring1, Spring2, Spring3}
// testing lighting script behavior, will improve later - Taylor
public class DmxSender : MonoBehaviour
{
    public DmxController controller;
    byte[] dmxData = new byte[512]; // this is zero indexed! 

    // array of all fixtures
    Fixture[] fixtures = new Fixture[Enum.GetValues(typeof(FixtureID)).Length];

    void Awake()
    {
        fixtures[(int)FixtureID.Conductor] = new Fixture(1);
        fixtures[(int)FixtureID.Lyre]      = new Fixture(5);
        fixtures[(int)FixtureID.Bad]       = new Fixture(9);
        fixtures[(int)FixtureID.Good]      = new Fixture(13);
    }

    void Update()
    {
        controller.Send(0, dmxData);
    }

    public void Set(FixtureID id, Color32 color)
    {
        var fixture = fixtures[(int)id];
        if (fixture == null)
        {
            Debug.LogError($"Fixture {id} not initialized");
            return;
        }
        fixture.Write(dmxData, color);
    }

    public void PlayCue(Cue cue)
    {
        // first, blackout everything
        foreach (var fixture in fixtures)
        {
            fixture.Write(dmxData, new Color32(0, 0, 0, 0));
        }
        switch (cue)
        {
            case Cue.Blackout:
                // everything already blacked out
                break;
            case Cue.Wait:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(250, 96, 12, 127));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(242, 50, 29, 127));
                break;
            case Cue.GoodEnding:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(138, 230, 69, 200));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(69, 174, 255, 200));
                fixtures[(int)FixtureID.Good].Write(dmxData, new Color32(255,255,255,255));
                break;
            case Cue.BadEnding:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(250, 96, 12, 127));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(242, 50, 29, 127));
                fixtures[(int)FixtureID.Bad].Write(dmxData, new Color32(255,255,255,255));                
                break;
            case Cue.Spring0:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(225, 255, 135, 100));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(255, 207, 135, 100));
                break;
            case Cue.Spring1:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(138, 230, 69, 127));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(69, 174, 255, 127));
                break;
            case Cue.Spring2:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(0, 255, 0, 255));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(238, 255, 0, 255));
                break;
            case Cue.Spring3:
                fixtures[(int)FixtureID.Conductor].Write(dmxData, new Color32(255, 0, 170, 255));
                fixtures[(int)FixtureID.Lyre].Write(dmxData, new Color32(0, 255, 255, 255));
                break;
            default:
                Debug.LogError($"cue {cue} not programmed");
                break;
        }
    }
}
