using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TriggerBuffer : MonoBehaviour
{
    private HapStickController _hapCon;

    private Thread _thread;
    private bool threadRunning;
    private bool threadedTaskComplete;

    private Queue<string> _buffer = new Queue<string>();
    [SerializeField]
    private byte _triggerDelayMS = 5;

    void Start()
    {
        _hapCon = GetComponent<HapStickController>();

        Random.InitState(System.DateTime.Now.Millisecond);

        _thread = new Thread(MultiThreadedMethod);
        _thread.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddSetParametersCommand();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            for (int i = 0; i < 2000; i++)
            {
                AddPlayAllCommand(
                    (byte)Random.Range(190, 255),
                    (byte)Random.Range(190, 255),
                    (byte)Random.Range(190, 255),
                    (byte)Random.Range(190, 255),
                    (byte)Random.Range(190, 255)
                );
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            AddStopAllCommand();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            threadedTaskComplete = true;
        }
    }

    private void MultiThreadedMethod()
    {
        threadRunning = true;
        threadedTaskComplete = false;

        while (threadRunning && !threadedTaskComplete)
        {
            //_hapCon.triggerPiezo(true);

            //*
            lock (_buffer)
            {
                if (_buffer.Count > 0)
                {
                    if(_hapCon.triggerCommand(_buffer.Dequeue()) == 3) // internal error
                    {
                        _buffer.Clear();
                    }
                }
            }
            //*/

            Thread.Sleep(_triggerDelayMS);
        }

        threadRunning = false;
    }

    public void AddSetParametersCommand()
    {
        _hapCon.triggerCommand(_hapCon.buildSetParametersCommand());
    }

    public void AddPlayAllCommand(byte amplitud1, byte amplitud2, byte amplitud3, byte amplitud4, byte amplitud5)
    {
        lock (_buffer)
        {
            _buffer.Enqueue(_hapCon.buildPlayAllCommand(amplitud1, amplitud2, amplitud3, amplitud4, amplitud5));
        }
    }

    public void AddStopAllCommand()
    {
        _hapCon.triggerCommand(_hapCon.buildStopAllCommand());
    }

    private void OnDisable()
    {
        if (threadRunning)
        {
            threadRunning = false;
            _thread.Join();
        }
    }
}