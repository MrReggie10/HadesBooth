using System;
using UnityEngine;

public enum FixtureID { Conductor, Lyre, Bad, Good }
public class Fixture
{
    int address; // starting address, 0 to 511

    // given address will be (1,512), need to remap to (0,511)
    public Fixture(int address)
    {
        if (address < 1 || address > 512)
        {
            Debug.LogError("Fixture cannot have a starting address of " + address);
            this.address = 0;
            return;
        }
        this.address = address - 1;
    }

    // uses color32 with alpha as dimmer
    public void Write(byte[] universe, Color32 color)
    {
        if (universe == null)
        {
            Debug.LogError("Universe is null");
            return;
        }
        if (address < 0 || address + 3 >= universe.Length)
        {
            Debug.LogError($"Write out of bounds: address={address}");
            return;
        }

        universe[address]     = color.r;
        universe[address + 1] = color.g;
        universe[address + 2] = color.b;
        universe[address + 3] = color.a; // alpha = dimmer
    }
}
