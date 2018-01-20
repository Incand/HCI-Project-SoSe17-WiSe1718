using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        /*
        foreach(Afterimage ai in _afterimages)
            ai.Update(Time.deltaTime);

        if (_afterimages.Count>0 && _afterimages.Peek().Done)
            _afterimages.Dequeue();

        float angle = MathUtil.SignedAngle(Vector3.forward,
            hapcon.getIMUOrientation() * Vector3.forward);

        if (hapcon.LaserSensorDistance < _sonicConfig.MaxSignal)
        { 
            _afterimages.Enqueue(new Afterimage(0));//angle
             //hapcon.triggerPiezo(true);
        }

        float[] angles = { -90.0f, -45.0f, 0.0f, 45.0f, 90.0f };
        foreach(float a in angles)
        {
            float value = GetSummedGaussian(angle + a);
            // Signal an actuator
        }
       // hapcon.triggerPiezo(true, "255,18,7,9");
       */
    }

    float _feedbackTimer = 0.0f;

    void FixedUpdate()
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

            _hapcon.triggerPiezo(true);
            Debug.Log(Time.fixedTime - _lastTime + "\t distance: " + _hapcon.UltrasonicSensorDistance);
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

            _hapcon.triggerPiezo(true);
            Debug.Log(Time.time - _lastTime + "\t distance: " + _hapcon.UltrasonicSensorDistance);
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
