using System;
using UnityEngine;

[Serializable]
public class SignalFeedbackConfiguration
{
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float _minSignal = 20.0f;
    public float MinSignal { get { return _minSignal; } }

    [SerializeField]
    [Range(150.0f, 1000.0f)]
    private float _maxSignal = 300.0f;
    public float MaxSignal { get { return _maxSignal; } }

    [SerializeField]
    [Range(0.1f, 5.0f)]
    private float _minFrequency = 1.0f;
    public float MinFrequency { get { return _minFrequency; } }

    [SerializeField]
    [Range(6.0f, 20.0f)]
    private float _maxFrequency = 10.0f;
    public float MaxFrequency { get { return _maxFrequency; } }
}
