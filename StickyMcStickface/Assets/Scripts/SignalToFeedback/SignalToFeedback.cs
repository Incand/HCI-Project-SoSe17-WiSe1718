using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MathUtil
{
    public static float SignedAngle(Vector3 from, Vector3 to)
    {
        Vector3 xzFrom = new Vector3(from.x, 0.0f, from.z);
        Vector3 xzTo = new Vector3(to.x, 0.0f, to.z);
        float angle = Vector3.Angle(xzFrom, xzTo);
        if (Vector3.Cross(xzFrom, xzTo).y < 0)
            return -angle;
        return angle;
    }
}

public enum Sensor
{
    LASER,
    SONIC
}

public class SignalToFeedback : MonoBehaviour {
    public HapStickController hapcon;
    // Use this for initialization

    [SerializeField]
    private Sensor feedbackSensor = Sensor.SONIC;

    [Header("Sonic Signal Processing")]
    [SerializeField]
    [Range(10.0f, 100.0f)]
    private float _minSonicSignal = 20.0f;

    [SerializeField]
    [Range(100.0f, 600.0f)]
    private float _maxSonicSignal = 200.0f;

    [SerializeField]
    private bool _useSonicAnimationCurve = false;

	[SerializeField]
	private AnimationCurve _sonicSignalToFrequency;

	[SerializeField]
	[Range(5.0f, 15.0f)]
	private float _sonicMaxFrequency = 5.0f;

    [SerializeField]
	[Range(.0f,1.0f)]
	private float _sonicMinFrequency = 1.0f;


    [Header("Infrared Signal Processing")]
    [Range(10.0f, 100.0f)]
    [SerializeField]
    private float _minSignal = 20.0f;

    [Range(100.0f, 600.0f)]
    [SerializeField]
    private float _maxSignal = 300.0f;

    [SerializeField]
    private bool _useInfraAnimationCurve = false;

	[SerializeField]
	private AnimationCurve _infraSignalToFrequency;

    [SerializeField]
    private float mean = 0;

    [Header("After Image Configuration")]

    private float _timer = 0.0f;
    private float _maxTime = 10.0f;
    
    private Queue<Afterimage> _afterimages = new Queue<Afterimage>();

    [SerializeField]
    private float _angleGaussStdDev = 10.0f;

    [SerializeField]
    private float _afterimagesLifetime = 0.5f;

    [SerializeField]
    private AnimationCurve _amplitudeFalloff = new AnimationCurve();


	void Start () {
        Afterimage.MAX_LIFETIME = _afterimagesLifetime;
        IEnumerator corout = _feedbackCoroutine();
        StartCoroutine(corout);
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
        return _sonicMaxFrequency * Mathf.Pow(_sonicMinFrequency / _sonicMaxFrequency, (_minSonicSignal - x) / (_minSonicSignal - _maxSonicSignal));
    } 

    void Update()
    {
        foreach(Afterimage ai in _afterimages)
            ai.Update(Time.deltaTime);

        if (_afterimages.Count>0 && _afterimages.Peek().Done)
            _afterimages.Dequeue();

        float angle = MathUtil.SignedAngle(Vector3.forward,
            hapcon.getIMUOrientation() * Vector3.forward);

        if (hapcon.LaserSensorDistance < _maxSonicSignal)
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
    }
	
    
    private IEnumerator _feedbackCoroutine()
    {
        while (true)
        {
            float metaFreq = feedbackSensor == Sensor.SONIC ? SonicMetaFrequency : LaserMetaFrequency;
			yield return new WaitForSeconds(1.0f / metaFreq);
            hapcon.durationMS = 125;
            hapcon.amplitud = 255;
            hapcon.frequency = 255;
            hapcon.cycles = (byte)(hapcon.frequency * hapcon.durationMS / 1000);

            hapcon.triggerPiezo(true);
            Debug.Log("Feedback!");
        }
    }
    
    #region SIGNAL_PROCESSING
    private float CombineMean(float laserDistance, float sonicDistance)
    {
        return (laserDistance + sonicDistance) * 0.5f;
    }

    private float SonicMetaFrequency
    {
        get
        {
            float clampedSignal = Mathf.Clamp(hapcon.UltrasonicSensorDistance, _minSonicSignal, _maxSonicSignal);
            return GetRevExpFeedback(clampedSignal);
            /*
			float cSNorm = (clampedSignal - _minSonicSignal) / (_maxSonicSignal - _minSonicSignal);
            return Mathf.Clamp(_sonicMaxFrequency * _sonicSignalToFrequency.Evaluate(cSNorm), 1.0f, _sonicMaxFrequency);
            */
        }
    }
    private float LaserMetaFrequency
    {
        get
        {
            float clampedSignal = Mathf.Clamp(hapcon.LaserSensorDistance, _minSonicSignal, _maxSonicSignal);
            return GetRevExpFeedback(clampedSignal);
            /*
			float cSNorm = (clampedSignal - _minSonicSignal) / (_maxSonicSignal - _minSonicSignal);
            return Mathf.Clamp(_sonicMaxFrequency * _sonicSignalToFrequency.Evaluate(cSNorm), 1.0f, _sonicMaxFrequency);
            */
        }
    }
    #endregion

    #region FEEDBACK_CALCULATION
    private byte calculateAmplitude(float signal)
    {
        float clampedSignal = Mathf.Clamp(signal, _minSignal, _maxSignal);
        float cSNorm = 1.0f - (clampedSignal - _minSignal) / (_maxSignal - _minSignal);
        return (byte)(255 * cSNorm);
    }
    #endregion
}
