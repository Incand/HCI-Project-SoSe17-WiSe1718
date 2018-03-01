using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Sensor
{
    LASER,
    SONIC
}

public class SignalToFeedback : MonoBehaviour
{
    [SerializeField]
    private HapStickController _hapcon;

    [SerializeField]
    private Sensor feedbackSensor = Sensor.SONIC;

    [Header("Signal Processing")]
    [SerializeField]
    private SignalFeedbackConfiguration _sonicConfig = new SignalFeedbackConfiguration();

    [SerializeField]
    private SignalFeedbackConfiguration _infraConfig = new SignalFeedbackConfiguration();
    
    
    [Header("After Image Configuration")]
    [SerializeField]
    private float _angleGaussStdDev = 10.0f;

    [SerializeField]
    private float _afterimagesLifetime = 0.5f;

    [SerializeField]
    private AnimationCurve _amplitudeFalloff = new AnimationCurve();

    private float _lastTime = 0.0f;
    private int count =0;
    private float _timer = 0.0f;
    private float _maxTime = 10.0f;

    private Queue<Afterimage> _afterimages = new Queue<Afterimage>();

    [Header("Signal Smoothing")]
    [SerializeField]
    private uint _nLastDistances = 2;

    private Queue<float> _lastDistances = new Queue<float>();

	void Start () {
        Afterimage.GAUSS_STD_DEV = _angleGaussStdDev;
        Afterimage.MAX_LIFETIME = _afterimagesLifetime;
        IEnumerator corout = _feedbackCoroutine();
        //StartCoroutine(corout);
	}

    private float GetSummedGaussian(float angle)
    {
        float result = 0.0f;
        foreach(Afterimage ai in _afterimages)
        {
            result += ai.GaussLike(angle);
        }
        return result;
    }

    private float GetRevExpFeedback(float x)
    {
        return _sonicConfig.MaxFrequency * Mathf.Pow(_sonicConfig.MinFrequency / _sonicConfig.MaxFrequency, (_sonicConfig.MinSignal - x) / (_sonicConfig.MinSignal - _sonicConfig.MaxSignal));
    } 

  

    float _feedbackTimer = 0.0f;

    void Update()
    {
        _feedbackTimer += Time.fixedDeltaTime;
        float metaPeriod = 1 / (feedbackSensor == Sensor.SONIC ? SonicMetaFrequency : LaserMetaFrequency);

        if(_feedbackTimer >= metaPeriod)
        {
            _feedbackTimer -= metaPeriod;
            _hapcon.durationMS = 50;
            _hapcon.amplitud = 255;
            _hapcon.frequency = 255;
            _hapcon.cycles = (byte)(_hapcon.frequency * _hapcon.durationMS / 1000);

            foreach (Afterimage ai in _afterimages)
            ai.Update(Time.deltaTime);

            if (_afterimages.Count>0 && _afterimages.Peek().Done)
            _afterimages.Dequeue();
            //now calculate the hapstick position and determine the piezo index.
            float angle = MathUtil.SignedAngle(Vector3.forward,
            _hapcon.getIMUOrientation() * Vector3.forward);
            //use a threshold for not spammin actuators with senseless feedback
             if (_hapcon.LaserSensorDistance < _sonicConfig.MaxSignal)
        { 
            _afterimages.Enqueue(new Afterimage(angle));//angle
                count++;
                if (count == 20)
                {
                    Debug.Log("angle: "+angle );
                    count = 0;
                }
                
        }
            //for it's just wasting battery
            float[] angles = new float[]{ -90.0f, -45.0f, 0.0f, 45.0f, 90.0f };
        foreach(float a in angles)
        {
            float value = GetSummedGaussian(angle + a);
                //determine piezo value according to position on gaussian curve circle
                //Debug.Log("Gauss value: " +a +" -> "+ value);
                //calculates the gaussian value for a certain peizo 0.0 is the front one whereas#
                //the other ones have index 1,2 (west) or 4,5 (east) are looking at
                // Signal an actuator when value bigger than threshold 
                if(count ==0)
                  Debug.Log("value "+Array.IndexOf(angles,a)+": angle: "+a+": "+value);
                _hapcon.triggerPiezo(true, Array.IndexOf(angles,a)+1);
            
        }
            //Debug.Log(Time.fixedTime - _lastTime + "\t distance: " + _hapcon.UltrasonicSensorDistance);
            _lastTime = Time.fixedTime;
        }
    }
	

    //unused
    private IEnumerator _feedbackCoroutine()
    {
        while (true)
        {
            float metaFreq = feedbackSensor == Sensor.SONIC ? SonicMetaFrequency : LaserMetaFrequency;
			yield return new WaitForSeconds(1.0f / metaFreq);
            _hapcon.durationMS = 50;
            _hapcon.amplitud = 255;
            _hapcon.frequency = 255;
            _hapcon.cycles = (byte)(_hapcon.frequency * _hapcon.durationMS / 1000);

            _hapcon.triggerPiezo(true,3);
            //Debug.Log(Time.time - _lastTime + "\t distance: " + _hapcon.UltrasonicSensorDistance);
            _lastTime = Time.time;
        }
    }
    
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

    private float SonicMetaFrequency
    {
        get
        {
            float clampedSignal = Mathf.Clamp(_hapcon.UltrasonicSensorDistance, _sonicConfig.MinSignal, _sonicConfig.MaxSignal);
            return GetRevExpFeedback(SmoothSignal(clampedSignal));
        }
    }
    private float LaserMetaFrequency
    {
        get
        {
            Debug.Log(_hapcon);
            float clampedSignal = Mathf.Clamp(_hapcon.LaserSensorDistance, _sonicConfig.MinSignal, _sonicConfig.MaxSignal);
            return GetRevExpFeedback(clampedSignal);
        }
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
}
