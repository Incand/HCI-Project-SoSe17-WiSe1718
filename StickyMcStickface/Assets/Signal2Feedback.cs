using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal2Feedback : MonoBehaviour {
    public HapStickController hapcon;
    // Use this for initialization

    [Range(10.0f, 100.0f)]
    [SerializeField]
    private float _minSignal = 20.0f;

    [Range(100.0f, 600.0f)]
    [SerializeField]
    private float _maxSignal = 300.0f;

    [SerializeField]
    private float mean = 0;

	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float signal = CombineMean(hapcon.LaserSensorDistance, hapcon.UltrasonicSensorDistance);
        mean = signal;
        hapcon.amplitud = calculateAmplitude(signal);


	}

    #region SIGNAL_PROCESSING
    private float CombineMean(float laserDistance, float sonicDistance)
    {
        return (laserDistance + sonicDistance) * 0.5f;
    }
    #endregion

    #region FEEDBACK_CALCULATION
    private byte calculateAmplitude(float signal)
    {
        float clampedSignal = Mathf.Clamp(signal, _minSignal, _maxSignal);
        float cSNorm = 1.0f - (signal - _minSignal) / (_maxSignal - _minSignal);
        return (byte)(255 * cSNorm);
    }
    #endregion
}
