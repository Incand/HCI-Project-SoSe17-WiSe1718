using UnityEngine;
using System.Collections;

using System.IO.Ports;
using System;

public class SerialComm : MonoBehaviour
{
    private enum SCOMMANDS
    {
        SAON,
        SAOFF,
    };
    
    public string port = "COM10";
    public int portBaudRate = 115200;
    public int portReadTimeout = 50;

    public float distance = 0.0f;

    private SerialPort stream;

    void readDistance(String s)
    {
        if(!float.TryParse(s, out distance))
        {
            distance = -1.0f;
        }
        //Debug.Log(s);
    }


    void Start ()
    {
        stream = new SerialPort(port, portBaudRate);
        stream.ReadTimeout = portReadTimeout;
        stream.Open();

        StartCoroutine
        (
            AsynchronousReadFromArduino
            (
                readDistance
                //,() => Debug.LogError("Error reading the serial-comm stream!")
                //,10f // Timeout in seconds
            )
        );
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) WriteToArduino(SCOMMANDS.SAON.ToString());
        if (Input.GetKeyUp(KeyCode.Alpha1)) WriteToArduino(SCOMMANDS.SAOFF.ToString());
    }

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
}
