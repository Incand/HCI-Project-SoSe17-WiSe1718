using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AfterimageSensorType{
    SONIC,
    LASER,
    BOTH
}

public class SignalToFeedback : MonoBehaviour
{
    [SerializeField]
    private Transform _rotaryObj;
    
    private HapStickController _hapCon;
    
    [SerializeField]
    private byte _laserIndex;

    [SerializeField]
    private byte _sonicIndex;

    [SerializeField]
    private List<byte> _piezoAfterimageIndices = new List<byte>();

    [SerializeField]
    private List<float> _piezoAfterimageAngles = new List<float>();

    [SerializeField]
    private SensorFeedbackHandler _laserHandler, _sonicHandler;

    [SerializeField]
    private AfterimageFeedbackHandler _afterimageHandler;
    
    [Header("After Image Configuration")]
    [SerializeField]
    private float _angleGaussStdDev = 10.0f;

    [SerializeField]
    private float _afterimagesLifetime = 0.5f;

    [SerializeField]
    private bool _afterimagesEnabled = true;

    [SerializeField]
    private AfterimageSensorType _afterimageSensor = AfterimageSensorType.BOTH;

    void Start() {
        _hapCon = GetComponent<HapStickController>();

        Afterimage.GAUSS_STD_DEV = _angleGaussStdDev;
        Afterimage.MAX_LIFETIME = _afterimagesLifetime;

        _hapCon.durationMS = 50;
        _hapCon.amplitud = 255;
        _hapCon.frequency = 255;
        _hapCon.cycles = (byte)(_hapCon.frequency * _hapCon.durationMS / 1000);
        
        _laserHandler = new LaserSensorFeedbackHandler(_hapCon,_laserIndex);
        _sonicHandler = new SonicSensorFeedbackHandler(_hapCon,_sonicIndex);
        _afterimageHandler = new AfterimageFeedbackHandler(_hapCon, _piezoAfterimageIndices, _piezoAfterimageAngles);
    }

    void Update()
    {
        _laserHandler.Update();
        _sonicHandler.Update();
        
        if (_afterimagesEnabled)
            _handleAfterimages();
    }

    private void _handleAfterimages()
    {
        float angle = MathUtil.SignedAngle(
                Vector3.forward,
                Vector3.ProjectOnPlane(-_rotaryObj.up, Vector3.up)
            );

        switch(_afterimageSensor)
        {
            case AfterimageSensorType.SONIC: 
                _afterimageHandler.AddAfterImage(angle, _sonicHandler.MetaFrequency);
                break;
            case AfterimageSensorType.LASER: 
                _afterimageHandler.AddAfterImage(angle, _laserHandler.MetaFrequency);
                break;
            case AfterimageSensorType.BOTH: 
                _afterimageHandler.AddAfterImage(angle, _sonicHandler.MetaFrequency);
                _afterimageHandler.AddAfterImage(angle, _laserHandler.MetaFrequency);
                break;
        }

        _afterimageHandler.Update(angle);
    }
}
