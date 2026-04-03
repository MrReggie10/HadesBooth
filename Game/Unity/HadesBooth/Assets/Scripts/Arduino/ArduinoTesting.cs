using System;
using System.Linq;
using UnityEngine;
public class ArduinoTesting : MonoBehaviour
{
    string[] validMessages = { "r", "y", "b", "c" };
    // This function is called by the Arduino MessageListner whenever any item is scanned.
    public void HandleMessage(string message) {
        if (validMessages.Contains(message))
        {
            Debug.Log("Note Played: " + message);
        }
        else
        {
            Debug.Log("Invalid message: " + message);
        }
    }
}
