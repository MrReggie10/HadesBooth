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
        if (validMessages.Contains(message))
        {
            Debug.Log("Note Played: " + message);
            sender.SendSerialMessage(message);
        }
        else
        {
            Debug.Log("Invalid message: " + message);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            sender.SendSerialMessage("r");
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            sender.SendSerialMessage("y");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            sender.SendSerialMessage("b");
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            sender.SendSerialMessage("c");
        }
    }
}
