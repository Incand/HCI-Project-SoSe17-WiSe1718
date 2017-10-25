using UnityEngine;
using System.Collections;

using System.IO.Ports;
using System;
using System.Text;

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
            stream.Parity = Parity.None;
            stream.StopBits = StopBits.One;
            stream.DataBits = 8;
            stream.Handshake = Handshake.None;
            ///stream.ReadTimeout = portReadTimeout;
            stream.DataReceived += DataReceivedHandler;

            stream.Open();
            Debug.Log("Serial stream opened OK!");
        }
        catch (Exception e)
        {
            Debug.Log("Could not open serial port: " + e.Message);

        }
    }


    void Update()
    {
        try
        {
            Debug.Log(ReadLineNonBlocking());
        }
        catch (Exception e)
        {
            Debug.Log("Error reading input " + e.ToString());
        }
    }

    private StringBuilder sb = new StringBuilder();
    string ReadLineNonBlocking()
    {
        int len = stream.BytesToRead;
        if (len == 0)
            return "";

        // read the buffer
        byte[] buffer = new byte[len];
        stream.Read(buffer, 0, len);
        sb.Append(ASCIIEncoding.ASCII.GetString(buffer));

        // got EOL?
        if (sb.Length < 2 ||
            sb[sb.Length - 2] != '\r' ||
            sb[sb.Length - 1] != '\n')
            return "";

        // if we are here, we got both EOL chars
        string entireLine = sb.ToString();
        sb.Length = 0;
        return entireLine;
    }

    SerialPort sp;
    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        sp = (SerialPort)sender;
        if (!float.TryParse(sp.ReadLine(), out distance))
        {
            distance = -1.0f;
        }
        Debug.Log(distance);
    }
}
