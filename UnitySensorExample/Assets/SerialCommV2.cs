using UnityEngine;
using System.Collections;

using System.IO.Ports;
using System;

public class SerialCommV2 : MonoBehaviour
{
    public string port = "COM4";
    public int portBaudRate = 115200;
    public int portReadTimeout = 2;

    public float distance = 0.0f;

    SerialPort stream;

    // Use this for initialization
    void Start()
    {
        try
        {
            stream = new SerialPort(port, portBaudRate);
            stream.ReadTimeout = portReadTimeout;
            stream.Open();
            stream.DataReceived += DataReceivedHandler;
        }
        catch (Exception e)
        {
            Debug.Log("Could not open serial port: " + e.Message);

        }
    }

    SerialPort sp;
    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        sp = (SerialPort)sender;
        if (!float.TryParse(sp.ReadLine(), out distance))
        {
            distance = -1.0f;
        }
        //Debug.Log(distance);
    }
}
