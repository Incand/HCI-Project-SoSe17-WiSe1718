
/**
 *  Haptics Framework Plugin for Unity
 *
 *  UHH HCI 
 *  Author: Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Linq;
using System.Collections;

public class HapStickController : MonoBehaviour
{
    [SerializeField]
    public GameObject cube;

    [Space(5)]
    [Header("Bluetooth parameters")]
    public string deviceName = "HCI_HapStick";
    public string donglePort = "COM11";
    public bool debugPackets = false;
    public bool debugPPS = !false; // Packets per second
    public String BatteryLevel = ""; // Batteries stick around 3.7V then slowly sink down to 3.2V. TODO: Display a warning below 3.7V 
    //public
    int packetsReceived = 0;
    
    [Space(5)]
    [Header("IMU Calibration parameters")]
    public short AngleOffsetX = 0;
    public short AngleOffsetY = 0;
    public short AngleOffsetZ = 0;
    public String CalibrationScore = "Fully calibrated = 3333";
    [Tooltip("Values from 0 to 3 for SGAM (System, Gyroscope, Accelerometer and Magnetometer)")]
    public string CalibrationStatus = "0000";

    [Space(5)]
    [Header("I2C Indexes for actuators")]
    [Range(1, 5)] public byte piezoIndex = 1;

    const float DRV2667_MIN_FREQ_BASE = 7.8125f;
    const float DRV2667_MAX_FREQ_BASE = 255 * DRV2667_MIN_FREQ_BASE;
    [Space(5)]
    [Header("Piezo actuator parameters")]
    [Range(0, 255)] public byte amplitud = 255;
    [Range(8, 1992)] public int frequency = 1200; // Frequency 7.8125 Hz -- 1992,1875 Hz
    [Range(0, 255)] public byte cycles = 8;
    [Range(0, 50)] public byte envelope = 1;
    public float durationMS = 0; //ms

    [Space(5)]
    [Header("Distance (cm) and Joystick status")]
    public short UltrasonicSensorDistance = 0;
    public short LaserSensorDistance = 0;
    short JoystickX = 0;
    short JoystickY = 0;

    //****************************************************************************************************************************

    [DllImport("BLECONTROLLER", EntryPoint = "ToggleDebug", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ToggleDebug();

    [DllImport("BLECONTROLLER", EntryPoint = "ClearQueue", CallingConvention = CallingConvention.Cdecl)]
    public static extern void ClearQueue();

    [DllImport("BLECONTROLLER", EntryPoint = "Connect", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Connect(string deviceName, string donglePort);

    [DllImport("BLECONTROLLER", EntryPoint = "Disconnect", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Disconnect();
    
    [DllImport("BLECONTROLLER", EntryPoint = "ReceivePacket", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ReceivePacket(IntPtr packet);

    [DllImport("BLECONTROLLER", EntryPoint = "SendPacket", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SendPacket(IntPtr packet, byte length);

    //****************************************************************************************************************************

    static System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
    short LostPacketCounter = 0;
    String EncodedID;
    byte[] packet = new byte[20];
    bool packetFound = false;
    
    //******************************************************

    long QuaternionT1 = sw.ElapsedMilliseconds;
    long QuaternionT2 = sw.ElapsedMilliseconds;
    byte QuaternionPacketCounter = 0;
    byte QuaternionPacketLength = 20;

    String QuaternionPacket1ID = BitConverter.ToString(Encoding.UTF8.GetBytes("Q1")).Replace("-", "");

    byte[] QuaternionPacketBytes = new byte[8]; // 4 short values

    const int Q_FACTOR = 32767;
    float[] quaternionData = new float[4];
    Quaternion centeringQuaternion = Quaternion.identity;
    Quaternion deviceQuaternion = Quaternion.identity;
    bool waitingForFirstQuaternion = true;

    //******************************************************

    long DistanceT1 = sw.ElapsedMilliseconds;
    long DistanceT2 = sw.ElapsedMilliseconds;
    byte DistancePacketCounter = 0;
    byte DistancePacketLength = 20;

    String DistancePacketID = BitConverter.ToString(Encoding.UTF8.GetBytes("DI")).Replace("-", "");

    byte[] DistancePacketBytes = new byte[8]; // 2 float values

    short[] DistanceData = new short[4];

    //******************************************************

    byte CalibrationOffsetPacketLength = 20;

    String CalibrationOffsetPacket1ID = BitConverter.ToString(Encoding.UTF8.GetBytes("C1")).Replace("-", "");
    String CalibrationOffsetPacket2ID = BitConverter.ToString(Encoding.UTF8.GetBytes("C2")).Replace("-", "");
    String CalibrationOffsetPacket3ID = BitConverter.ToString(Encoding.UTF8.GetBytes("C3")).Replace("-", "");

    byte[] CalibrationOffsetPacketBytes = new byte[8]; // 4 ushort values

    ushort[] CalibrationOffsets = new ushort[11];

    //******************************************************

    String CalibrationStatusPacketID = BitConverter.ToString(Encoding.UTF8.GetBytes("CS")).Replace("-", "");

    //******************************************************

    String BatteryLevelPacketID = BitConverter.ToString(Encoding.UTF8.GetBytes("BA")).Replace("-", "");

    //******************************************************

    void Update()
    {   
        durationMS = 1000.0f * ((float)cycles / (float)frequency); // TODO: add the envelope time. Page 20 on http://www.ti.com/lit/ds/symlink/drv2667.pdf

        if (Input.GetKeyDown(KeyCode.C))
        {
            recenter();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            triggerPiezo("1");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            triggerPiezo("5");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            triggerPiezo(true, "255,18,7,9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            triggerPiezo(true, "255,25,50,9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            triggerPiezo(true, "100,288,15,9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            triggerPiezo(true, "255,50,5,9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            triggerPiezo(true, "255,40,2,3");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            triggerPiezo(false);
        }

        //***************************************************************

        packetFound = false;
        IntPtr ip = Marshal.AllocHGlobal(20);
        try
        {
            ip = ReceivePacket(ip);
            if(!ip.Equals(IntPtr.Zero))
            {
                Marshal.Copy(ip, packet, 0, packet.Length);
                packetFound = true;
            }
        }
        catch(Exception e)
        {
            Debug.Log(string.Format("Error marshalling packet >>> {0}", e.Message));
        }
        finally
        {
            Marshal.FreeHGlobal(ip);
        }
        if (packetFound)
        {
            packetsReceived++;

            EncodedID = Encoding.UTF8.GetString(packet.Take(4).ToArray());

            if (DistancePacketID.Equals(EncodedID))
            {
                for (int i = 0; i <= DistancePacketLength - DistancePacketID.Length - 1; i += 2)
                {
                    DistancePacketBytes[i / 2] = Convert.ToByte((char)packet[i + DistancePacketID.Length] + "" + (char)packet[i + 1 + DistancePacketID.Length], 16);
                }

                unsafe
                {
                    fixed (byte* pBuffer = DistancePacketBytes)
                    {
                        short* i = (short*)pBuffer;
                        DistanceData[0] = i[0];
                        DistanceData[1] = i[1];
                        DistanceData[2] = i[2];
                        DistanceData[3] = i[3];
                    }
                }

                UltrasonicSensorDistance    = DistanceData[0];
                LaserSensorDistance         = DistanceData[1];
                //JoystickX = DistanceData[2];
                //JoystickY = DistanceData[3];

                if (debugPackets) Debug.Log(string.Format("Distances & XY: {0}", DistanceData[0]));

                if (debugPPS)
                {
                    DistanceT2 = sw.ElapsedMilliseconds;
                    DistancePacketCounter++;
                    if (DistanceT2 - DistanceT1 >= 1000)
                    {
                        Debug.Log(string.Format("-------------------------------------------FPS-DistancesXY: {0} >> {1}", DistancePacketCounter, LostPacketCounter));
                        DistanceT1 = DistanceT2;
                        DistancePacketCounter = 0;
                        LostPacketCounter = 0;
                    }
                }
            }
            else if (QuaternionPacket1ID.Equals(EncodedID))
            {
                for (int i = 0; i <= QuaternionPacketLength - QuaternionPacket1ID.Length - 1; i += 2)
                {
                    QuaternionPacketBytes[i / 2] = Convert.ToByte((char)packet[i + QuaternionPacket1ID.Length] + "" + (char)packet[i + 1 + QuaternionPacket1ID.Length], 16);
                }

                unsafe
                {
                    fixed (byte* pBuffer = QuaternionPacketBytes)
                    {
                        short* i = (short*)pBuffer;
                        quaternionData[0] = (float)i[0] / Q_FACTOR;
                        quaternionData[1] = (float)i[1] / Q_FACTOR;
                        quaternionData[2] = (float)i[2] / Q_FACTOR;
                        quaternionData[3] = (float)i[3] / Q_FACTOR;
                    }
                }

                deviceQuaternion.Set(quaternionData[1], quaternionData[2], quaternionData[3], quaternionData[0]);

                if(waitingForFirstQuaternion == true)
                {
                    recenter();
                    waitingForFirstQuaternion = false;
                }

                cube.transform.rotation =
                    Quaternion.Euler(AngleOffsetX, AngleOffsetY, AngleOffsetZ) *
                    (centeringQuaternion * deviceQuaternion);

                if (debugPackets) Debug.Log(string.Format("RX: {0} {1} {2} {3}", quaternionData[0], quaternionData[1], quaternionData[2], quaternionData[3]));

                if (debugPPS)
                {
                    QuaternionT2 = sw.ElapsedMilliseconds;
                    QuaternionPacketCounter++;
                    if (QuaternionT2 - QuaternionT1 >= 1000)
                    {
                        Debug.Log(string.Format("-------------------------------------------FPS-Q: {0} >> {1}", QuaternionPacketCounter, LostPacketCounter));
                        QuaternionT1 = QuaternionT2;
                        QuaternionPacketCounter = 0;
                        LostPacketCounter = 0;
                    }
                }
            }
            else if (CalibrationOffsetPacket1ID.Equals(EncodedID))
            {
                for (int i = 0; i <= CalibrationOffsetPacketLength - CalibrationOffsetPacket1ID.Length - 1; i += 2)
                {
                    CalibrationOffsetPacketBytes[i / 2] = Convert.ToByte((char)packet[i + CalibrationOffsetPacket1ID.Length] + "" + (char)packet[i + 1 + CalibrationOffsetPacket1ID.Length], 16);
                }

                unsafe
                {
                    fixed (byte* pBuffer = CalibrationOffsetPacketBytes)
                    {
                        ushort* us = (ushort*)pBuffer;
                        CalibrationOffsets[0] = us[0];
                        CalibrationOffsets[1] = us[1];
                        CalibrationOffsets[2] = us[2];
                        CalibrationOffsets[3] = us[3];
                    }
                }
            }
            else if (CalibrationOffsetPacket2ID.Equals(EncodedID))
            {
                for (int i = 0; i <= CalibrationOffsetPacketLength - CalibrationOffsetPacket2ID.Length - 1; i += 2)
                {
                    CalibrationOffsetPacketBytes[i / 2] = Convert.ToByte((char)packet[i + CalibrationOffsetPacket2ID.Length] + "" + (char)packet[i + 1 + CalibrationOffsetPacket2ID.Length], 16);
                }

                unsafe
                {
                    fixed (byte* pBuffer = CalibrationOffsetPacketBytes)
                    {
                        ushort* us = (ushort*)pBuffer;
                        CalibrationOffsets[4] = us[0];
                        CalibrationOffsets[5] = us[1];
                        CalibrationOffsets[6] = us[2];
                        CalibrationOffsets[7] = us[3];
                    }
                }
            }
            else if (CalibrationOffsetPacket3ID.Equals(EncodedID))
            {
                for (int i = 0; i <= CalibrationOffsetPacketLength - CalibrationOffsetPacket3ID.Length - 1; i += 2)
                {
                    CalibrationOffsetPacketBytes[i / 2] = Convert.ToByte((char)packet[i + CalibrationOffsetPacket3ID.Length] + "" + (char)packet[i + 1 + CalibrationOffsetPacket3ID.Length], 16);
                }

                unsafe
                {
                    fixed (byte* pBuffer = CalibrationOffsetPacketBytes)
                    {
                        ushort* us = (ushort*)pBuffer;
                        CalibrationOffsets[8] = us[0];
                        CalibrationOffsets[9] = us[1];
                        CalibrationOffsets[10] = us[2];
                    }
                }

                CalibrationSettingsSaveLoad.Save(CalibrationOffsets);
            }
            else if (CalibrationStatusPacketID.Equals(EncodedID))
            {
                CalibrationStatus = "" + (char)packet[4] + (char)packet[5] + (char)packet[6] + (char)packet[7];
            }
            else if (BatteryLevelPacketID.Equals(EncodedID))
            {
                BatteryLevel = "" + (char)packet[4] + "." + (char)packet[5] + " Volts";
            }
            else
            {
                //string convertedText = Encoding.UTF8.GetString(data);
                //if(convertedText.StartsWith("T"))
                //AddToLog(string.Format("RX: {0}", convertedText.Substring(1)));
                Debug.Log(
                    string.Format(
                        "Lost Packet >>> {0} >>> ID {1}", 
                        Encoding.UTF8.GetString(packet), 
                        EncodedID));
                LostPacketCounter++;
            }
        }
    }

    void Start()
    {
        int result = Connect(deviceName, donglePort);
        Debug.Log(string.Format("Connect result >>> {0}", result));
        ClearQueue();
        ToggleDebug(); // Uncomment this like to log activitiy to a file (BLEController.log)
    }

    private void OnDestroy()
    {
        disconnect();
    }

    //******************************************************
    // BLE commands
    //******************************************************

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    int tryCounter = 0;
    private int SendPacket(string message)
    {
        byte[] packet = Encoding.UTF8.GetBytes(message);

        //Debug.Log("PAKET " + BitConverter.ToString(packet));
        //Debug.Log("MSG " + message);

        IntPtr ptr = Marshal.AllocHGlobal(packet.Length);
        Marshal.Copy(packet, 0, ptr, packet.Length);
        
        int result = 0;
        bool mSent = false;
        do
        {
            result = SendPacket(ptr, (byte)packet.Length);
            if(result == 0) // OK
            {
                mSent = true;
                break;
            }
            else if (result == 3 ) // 3 = Internal error
            {
                Debug.Log("Error !!!!! " + result);
                Marshal.FreeHGlobal(ptr);
                return result;
            }
            tryCounter++;
        } while (result == 17 && tryCounter<20); // 17 = Busy, 20 = max limit for a data stream (common value ~ 3)

        Marshal.FreeHGlobal(ptr);

        if (!mSent) Debug.Log("Package discarded !!!!!");
        
        if (debugPackets) Debug.Log(string.Format("Message sent: {0} with length {1} result {2}", message, packet.Length, result));

        tryCounter = 0;
        return result;
    }
    
    public void recenter()
    {
        centeringQuaternion = Quaternion.Inverse(deviceQuaternion);
    }

    public void connect()
    {
        int result = Connect(deviceName, donglePort);
        Debug.Log(string.Format("Connect result >>> {0}", result));
    }

    public void disconnect()
    {
        ClearQueue();
        int result = Disconnect();
        Debug.Log(string.Format("Disconnect result >>> {0}", result));
    }

    public void requestIMUCalibration()
    {
        SendPacket("CO-REQ");
    }

    private void uploadIMUCalibration(String s)
    {
        int result = SendPacket(s);
        if (result != 0)
        {
            uploadIMUCalibration(s); // retry sending data until the package is complete // TODO: avoid this recursion
        }
    }

    public void uploadIMUCalibration()
    {
        CalibrationOffsets = CalibrationSettingsSaveLoad.Load().CalibrationOffsets;

        String s = "CO: ";
        for (byte i = 0; i < 11; i++) { s += CalibrationOffsets[i] + " "; }
        Debug.Log(s);

        uploadIMUCalibration("CO1" + CalibrationOffsets[0] + "," + CalibrationOffsets[1] + "," + CalibrationOffsets[2]);
        uploadIMUCalibration("CO2" + CalibrationOffsets[3] + "," + CalibrationOffsets[4] + "," + CalibrationOffsets[5]);
        uploadIMUCalibration("CO3" + CalibrationOffsets[6] + "," + CalibrationOffsets[7] + "," + CalibrationOffsets[8]);
        uploadIMUCalibration("CO4" + CalibrationOffsets[9] + "," + CalibrationOffsets[10] + ",0");
    }
    
    public void toggleIMU()
    {
        SendPacket("TOGIMU"); // Toggle IMU data transfer
    }

    public void ToggleLidarSensor()
    {
        SendPacket("DSLT");
    }

    public void toggleIRSensor()
    {
        SendPacket("DSIT");
    }

    public void triggerPiezo(bool enabled, String index, String piezoValues)
    {
        SendPacket("AP" + (enabled ? ("E" + index + "," + piezoValues) : ("D" + piezoIndex)));
    }

    public void triggerPiezo(bool enabled, String piezoValues)
    {
        SendPacket("AP" + (enabled ? ("E" + piezoIndex + "," + piezoValues) : ("D" + piezoIndex)));
    }

    public void triggerPiezo(bool enabled)
    {
        SendPacket("AP" + (enabled ? ("E" + piezoIndex + "," + amplitud + "," + (byte)(frequency / DRV2667_MIN_FREQ_BASE) + "," + cycles + "," + envelope) : ("D" + piezoIndex)));
    }

    public void triggerPiezo(String index)
    {
        SendPacket("AP" + "E" + index + "," + amplitud + "," + (byte)(frequency / DRV2667_MIN_FREQ_BASE) + "," + cycles + "," + envelope);
    }

    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    public String buildSetParametersCommand()
    {
        return ("SPP" + (byte)(frequency / DRV2667_MIN_FREQ_BASE) + "," + cycles + "," + envelope);
    }

    public String buildPlayAllCommand(byte amplitud1, byte amplitud2, byte amplitud3, byte amplitud4, byte amplitud5)
    {
        return ("Z" + amplitud1 + "," + amplitud2 + "," + amplitud3 + "," + amplitud4 + "," + amplitud5);
    }

    public String buildStopAllCommand()
    {
        return ("PPS");
    }

    public int triggerCommand(String command)
    {
        return SendPacket(command);
    }

    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    ///Debug.Log(string.Format(sw.ElapsedMilliseconds + "Fingertip feedback >>> {0}", enabled));
}
