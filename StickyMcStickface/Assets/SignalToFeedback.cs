using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalToFeedback : MonoBehaviour {
    public HapStickController hapcon;
    // Use this for initialization

    [Header("Sonic Signal Processing")]
    [SerializeField]
    [Range(10.0f, 100.0f)]
    private float _minSonicSignal = 20.0f;

    [SerializeField]
    [Range(100.0f, 600.0f)]
    private float _maxSonicSignal = 200.0f;

	[SerializeField]
	private AnimationCurve _sonicSignalToFrequency;

	[SerializeField]
	[Range(5.0f, 15.0f)]
	private float _sonicMaxFrequency = 5.0f;


    [Header("Infrared Signal Processing")]
    [Range(10.0f, 100.0f)]
    [SerializeField]
    private float _minSignal = 20.0f;

    [Range(100.0f, 600.0f)]
    [SerializeField]
    private float _maxSignal = 300.0f;

	[SerializeField]
	private AnimationCurve _infraSignalToFrequency;

    [SerializeField]
    private float mean = 0;

    private float _timer = 0.0f;
    private float _maxTime = 10.0f;

	void Start () {
        //IEnumerator corout = _sonicFeedbackCoroutine();
        //StartCoroutine(corout);
	}

    void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        float smf = SonicMetaFrequency;
        _maxTime = 1.0f / smf;
        if(_timer >= _maxTime)
        {
            _timer -= _maxTime;
            hapcon.triggerPiezo(true);
            Debug.Log("Feedback!");
        }
    }
	
    /*
    private IEnumerator _sonicFeedbackCoroutine()
    {
        while (true)
        {
			yield return new WaitForSeconds(1.0f / SonicMetaFrequency);
            hapcon.durationMS = 125;
            hapcon.amplitud = 255;
            hapcon.frequency = 255;
            hapcon.cycles = (byte)(hapcon.frequency * hapcon.durationMS / 1000);

            hapcon.triggerPiezo(true);
            Debug.Log("Feedback!");
        }
    }*/
    
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
			float cSNorm = (clampedSignal - _minSonicSignal) / (_maxSonicSignal - _minSonicSignal);
            return Mathf.Clamp(_sonicMaxFrequency * _sonicSignalToFrequency.Evaluate(cSNorm), 1.0f, _sonicMaxFrequency);
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
