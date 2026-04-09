/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

// Modified by Taylor Roberts

using UnityEngine;
using System.Collections;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */
public class MessageListener : MonoBehaviour
{
    private enum ArduinoType { Lyre, Guitar, Conductor, FlowerWall }
    [SerializeField] private ArduinoType arduinoType;
    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private ArduinoTesting arduinoTester;
    

    // Invoked when a line of data is received from the serial device.
    public void OnMessageArrived(string msg)
    {
        Debug.Log(arduinoType.ToString() + ": " + msg);
        if (gameStatus != null)
        {
            if (arduinoType == ArduinoType.Lyre) gameStatus.HandleMessage(msg, 0);
            else if (arduinoType == ArduinoType.Guitar) gameStatus.HandleMessage(msg, 1);
        }
        
        else // B mode
        {
            if (arduinoType == ArduinoType.Lyre) arduinoTester.HandleMessage(msg, 0);
            else if (arduinoType == ArduinoType.Guitar) arduinoTester.HandleMessage(msg, 1);
        }
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    public void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        else
            Debug.Log("Connection attempt failed or disconnection detected");
    }
}
