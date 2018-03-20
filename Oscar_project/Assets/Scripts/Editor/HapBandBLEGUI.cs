
/**
 *  Haptics Framework
 *
 *  UHH HCI 
 *  Author: Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HapStickController))]
public class HapBandBLEGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HapStickController hpbc = (HapStickController)target;
 
        GUILayout.Space(15);

        if (GUILayout.Button("Trigger Feedback"))
        {
            hpbc.triggerPiezo(true);
        }
        if (GUILayout.Button("Stop Feedback"))
        {
            hpbc.triggerPiezo(false);
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Toggle Maxbotix Sensor"))
        {
            hpbc.toggleIRSensor();
        }

        if (GUILayout.Button("Toggle LIDAR Sensor"))
        {
            hpbc.ToggleLidarSensor();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Toggle IMU"))
        {
            hpbc.toggleIMU();
        }
        if (GUILayout.Button("Recenter IMU"))
        {
            hpbc.recenter();
        }
        if (GUILayout.Button("Request IMU Calibration"))
        {
            hpbc.requestIMUCalibration();
        }
        if (GUILayout.Button("Upload IMU Calibration"))
        {
            hpbc.uploadIMUCalibration();
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Connect"))
        {
            hpbc.connect();
        }
        if (GUILayout.Button("Disconnect"))
        {
            hpbc.disconnect();
        }
    }
}
