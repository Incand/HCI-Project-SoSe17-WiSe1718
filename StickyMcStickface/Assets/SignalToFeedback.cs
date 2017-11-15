﻿using System.Collections;
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

    [Header("Infrared Signal Processing")]
    [Range(10.0f, 100.0f)]
    [SerializeField]
    private float _minSignal = 20.0f;

    [Range(100.0f, 600.0f)]
    [SerializeField]
    private float _maxSignal = 300.0f;

    [SerializeField]
    private float mean = 0;

	void Start () {
        IEnumerator corout = _sonicFeedbackCoroutine();
        StartCoroutine(corout);
	}
	
    private IEnumerator _sonicFeedbackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.125f);
            hapcon.durationMS = 125;
            hapcon.amplitud = 255;
            hapcon.frequency = SonicSignalFeedback;
            hapcon.cycles = (byte)(hapcon.frequency * hapcon.durationMS / 1000);

            hapcon.triggerPiezo(true);
            Debug.Log("Feedback!");
        }
    }
    

	// Update is called once per frame
	void FixedUpdate () {

	}

    #region SIGNAL_PROCESSING
    private float CombineMean(float laserDistance, float sonicDistance)
    {
        return (laserDistance + sonicDistance) * 0.5f;
    }

    private byte SonicSignalFeedback
    {
        get
        {
            float clampedSignal = Mathf.Clamp(hapcon.UltrasonicSensorDistance, _minSonicSignal, _maxSonicSignal);
            float cSNorm = 1.0f - (clampedSignal - _minSignal) / (_maxSignal - _minSignal);
            return (byte)(255 * cSNorm);
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
