using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LEDSending : MonoBehaviour
{
    [SerializeField] private SerialController strip;

    private int counter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < 60)
        {
            StartCoroutine(SendRed());
        }
        else if (counter < 120)
        {
            StartCoroutine(SendBlue());
        }
        else if (counter < 180)
        {
            StartCoroutine(SendYellow());
        }
        else
        {
            StartCoroutine(SendCyan());
        }

        counter++;
        if(counter >= 240)
        {
            counter = 0;
        }
    }

    private IEnumerator SendRed()
    {
        strip.SendSerialMessage("r\n");
        yield return 0;
    }

    private IEnumerator SendBlue()
    {
        strip.SendSerialMessage("b\n");
        yield return 0;
    }

    private IEnumerator SendYellow()
    {
        strip.SendSerialMessage("y\n");
        yield return 0;
    }

    private IEnumerator SendCyan()
    {
        strip.SendSerialMessage("c\n");
        yield return 0;
    }
}
