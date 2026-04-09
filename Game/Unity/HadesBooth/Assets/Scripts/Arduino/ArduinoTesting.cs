using System;
using System.Linq;
using UnityEngine;
public class ArduinoTesting : MonoBehaviour
{
    [SerializeField] public SerialController sender;
    string[] validMessages = { "r", "y", "b", "c" };
    // This function is called by the Arduino MessageListner whenever any item is scanned.
    public void HandleMessage(string message)
    {
        message = message.Trim();
        Debug.Log("Note Played: " + message);
        if (message = "r")
        {
            sender.SendSerialMessage("L 12 0 0 r 200 0 k 200 1 r 400 1 k 400 2 r 600 2 k 600 3 r 800 3 k 800 4 r 1000 4 k 1000 5 r 1200 5 k");
        }
        else if (message = "b")
        {
            sender.SendSerialMessage("L 12 0 0 b 200 0 k 200 1 b 400 1 k 400 2 b 600 2 k 600 3 b 800 3 k 800 4 b 1000 4 k 1000 5 b 1200 5 k");
        }
        else if (message = "c")
        {
            sender.SendSerialMessage("L 12 0 0 c 200 0 k 200 1 c 400 1 k 400 2 c 600 2 k 600 3 c 800 3 k 800 4 c 1000 4 k 1000 5 c 1200 5 k");
        }
        else if (message = "y")
        {
            sender.SendSerialMessage("L 12 0 0 y 200 0 k 200 1 y 400 1 k 400 2 y 600 2 k 600 3 y 800 3 k 800 4 y 1000 4 k 1000 5 y 1200 5 k");
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         sender.SendSerialMessage("r");
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Y))
    //     {
    //         sender.SendSerialMessage("y");
    //     }
    //     else if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         sender.SendSerialMessage("b");
    //     }
    //     else if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         sender.SendSerialMessage("c");
    //     }
    // }
}
