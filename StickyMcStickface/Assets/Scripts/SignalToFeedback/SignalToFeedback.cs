using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private bool _afterimagesFromSonic = true;
    [SerializeField]
    private bool _afterimagesFromLaser = false;

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
        _afterimageHandler = new AfterimageFeedbackHandler(_hapCon, _afterimagesFromSonic, _afterimagesFromLaser, _piezoAfterimageIndices, _piezoAfterimageAngles);
    }
    /*
    private float GetSummedGaussian(float angle)
    {
        float result = 0.0f;
        foreach (Afterimage ai in _afterimages)
            result += ai.GaussLike(angle);
        return result;
    }
    
    private void _handleAfterImages(float angle)
    {
        // Update all afterimages
        foreach (Afterimage ai in _afterimages)
                ai.Update(Time.deltaTime);

        // Remove finished afterimages
        if (_afterimages.Count > 0 && _afterimages.Peek().Done)
            _afterimages.Dequeue();
        
        // Add afterimages each frame
        //use a threshold for not spammin actuators with senseless feedback
        if (_hapCon.UltrasonicSensorDistance < _sonicConfig.MaxSignal)
        { 
            _afterimages.Enqueue(new Afterimage(angle));//angle
            count++;
            if (count == 20)
            {
                Debug.Log("angle: " + angle);
                count = 0;
            }
        }
    }

    private void _handlePiezoTriggering(float angle)
    {
        
        foreach(float a in _piezoAngles)
        {
            float value = GetSummedGaussian(angle + a);
            if (value < 0.1f)
                continue;
            // determine piezo value according to position on gaussian curve circle
            // Debug.Log("Gauss value: " +a +" -> "+ value);
            // calculates the gaussian value for a certain peizo 0.0 is the front one whereas#
            // the other ones have index 1,2 (west) or 4,5 (east) are looking at
            // Signal an actuator when value bigger than threshold 
            if(count == 0)
                Debug.Log("value " + Array.IndexOf(_piezoAngles, a) + ": angle: " + a + ": " + value);
            _hapCon.triggerPiezo(true, Array.IndexOf(_piezoAngles, a) + 1);
        }
    }
    

    void doIt(Sensor sensorType)
    {
        _feedbackTimer += Time.deltaTime;
        
        float metaPeriod = 1 / (sensorType == Sensor.SONIC ? SonicMetaFrequency : LaserMetaFrequency);

        if(_feedbackTimer >= metaPeriod)
        {
            _feedbackTimer -= metaPeriod;

            // now calculate the hapstick position and determine the piezo index.
            // For orientation reasons, the local axis pointing along the stick is "down"
            float angle = MathUtil.SignedAngle(
                Vector3.forward,
                Vector3.ProjectOnPlane(-_rotaryObj.up, Vector3.up)
            );

            _handleAfterImages(angle);
            _handlePiezoTriggering(angle);
        }
    }
    */

    void Update()
    {
        _laserHandler.Update();
        _sonicHandler.Update();
        
        /*float angle = MathUtil.SignedAngle(
                Vector3.forward,
                Vector3.ProjectOnPlane(-_rotaryObj.up, Vector3.up)
            );

        if (_afterimagesFromSonic)
            _afterimageHandler.AddAfterImage(angle, _sonicHandler.MetaFrequency);
        if (_afterimagesFromLaser)
            _afterimageHandler.AddAfterImage(angle, _laserHandler.MetaFrequency);

        // TODO: add stick angle
        _afterimageHandler.Update(angle);
        */
    }
	/*
    #region SIGNAL_PROCESSING
    private float CombineMean(float laserDistance, float sonicDistance)
    {
        return (laserDistance + sonicDistance) * 0.5f;
    }

    private float SmoothSignal(float newSignal)
    {
        _lastDistances.Enqueue(newSignal);
        while (_lastDistances.Count > _nLastDistances)
            _lastDistances.Dequeue();
        float sum = 0.0f;
        foreach (float f in _lastDistances)
            sum += f;
        return sum / _lastDistances.Count;
    }
    #endregion

    #region FEEDBACK_CALCULATION
    private byte calculateAmplitude(float signal)
    {
        SignalFeedbackConfiguration config = feedbackSensor == Sensor.SONIC ? _sonicConfig : _infraConfig;

        float clampedSignal = Mathf.Clamp(signal, config.MinSignal, config.MaxSignal);
        float cSNorm = 1.0f - (clampedSignal - config.MinSignal) / (config.MaxSignal - config.MinSignal);
        return (byte)(255 * cSNorm);
    }
    #endregion
    */
}
