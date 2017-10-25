
/**
 *  Haptics Framework
 *
 *  Copyright 2017 by UHH HCI 
 *  Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using UnityEngine;
using System.Collections;

using System.IO.Ports;
using System;

public class SerialComm : MonoBehaviour
{
    private enum SCOMMANDS
    {
        SAON,
        SBON,
        SCON,
        SDON,
        SAOFF,
        SBOFF,
        SCOFF,
        SDOFF
    };

    public string port = "COM4";
    public int portBaudRate = 9600;
    public int portReadTimeout = 50;

    private SerialPort stream;

    void Start ()
    {
        stream = new SerialPort(port, portBaudRate);
        stream.ReadTimeout = portReadTimeout;
        stream.Open();

        StartCoroutine
        (
            AsynchronousReadFromArduino
            (
                (string s) => Debug.Log(s)
                //,() => Debug.LogError("Error reading the serial-comm stream!")
                //,10f // Timeout in seconds
            )
        );
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) WriteToArduino(SCOMMANDS.SAON.ToString());
        if (Input.GetKeyUp(KeyCode.Alpha1)) WriteToArduino(SCOMMANDS.SAOFF.ToString());

        if (Input.GetKeyDown(KeyCode.Alpha2)) WriteToArduino(SCOMMANDS.SBON.ToString());
        if (Input.GetKeyUp(KeyCode.Alpha2)) WriteToArduino(SCOMMANDS.SBOFF.ToString());

        if (Input.GetKeyDown(KeyCode.Alpha3)) WriteToArduino(SCOMMANDS.SCON.ToString());
        if (Input.GetKeyUp(KeyCode.Alpha3)) WriteToArduino(SCOMMANDS.SCOFF.ToString());

        if (Input.GetKeyDown(KeyCode.Alpha4)) WriteToArduino(SCOMMANDS.SDON.ToString());
        if (Input.GetKeyUp(KeyCode.Alpha4)) WriteToArduino(SCOMMANDS.SDOFF.ToString());
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
