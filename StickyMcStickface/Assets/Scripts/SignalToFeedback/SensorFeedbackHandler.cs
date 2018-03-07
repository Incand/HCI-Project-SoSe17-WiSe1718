using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class SensorFeedbackHandler {

    protected HapStickController _hapCon;

    protected TriggerBuffer _triggerBuffer;

    protected float _feedbackTimer = 0.0f;
    protected float _timer = 0.0f;

    [SerializeField]
    protected byte _piezoIndex;

    protected FeedbackCalculator _feedbackCalculator = new FeedbackCalculator();

    public SensorFeedbackHandler(HapStickController hapCon, byte index)
    {
        _piezoIndex = index;
        _hapCon = hapCon;

        _triggerBuffer = _hapCon.GetComponent<TriggerBuffer>();
    }
    
    public virtual float MetaFrequency
    {
        get { return 0.0f; }
    }

    public virtual void Update()
    {
        _feedbackTimer += Time.deltaTime;
        float metaPeriod = 1.0f / MetaFrequency;

        if(_feedbackTimer >= metaPeriod)
        {
            _feedbackTimer -= metaPeriod;
            _triggerBuffer.AddCommand(_piezoIndex);
        }
    }
}

public class LaserSensorFeedbackHandler : SensorFeedbackHandler
{
    public LaserSensorFeedbackHandler(HapStickController hapCon, byte index)
        : base(hapCon, index)
    { }

    public override float MetaFrequency
    {
        get { return _feedbackCalculator.GetMetaFrequency(_hapCon.LaserSensorDistance); }
    }
}

public class SonicSensorFeedbackHandler : SensorFeedbackHandler
{
    public SonicSensorFeedbackHandler(HapStickController hapCon, byte index)
        : base(hapCon, index)
    { }

    public override float MetaFrequency
    {
        get { return _feedbackCalculator.GetMetaFrequency(_hapCon.UltrasonicSensorDistance); }
    }
}

public enum SensorType{
    LASER,
    SONIC
}



[Serializable]
public class AfterimageFeedbackHandler
{
    private HapStickController _hapCon;

    private TriggerBuffer _triggerBuffer;
    
    protected float _feedbackTimer = 0.0f;
    protected float _timer = 0.0f;

    [SerializeField]
    private byte _maxAmplitude = 255;
    
    private List<byte> _piezoIndices;

    private List<float> _piezoAngles;

    private Dictionary<byte, float> _piezoIndexAngleDict;

    private Queue<Afterimage> _afterimages = new Queue<Afterimage>();

    public AfterimageFeedbackHandler(HapStickController hapCon, bool afterimagesFromSonic, bool afterimagesFromLaser, List<byte> piezoIndices, List<float> piezoAngles)
    {
        _hapCon = hapCon;
        _piezoIndices = piezoIndices;
        _piezoAngles = piezoAngles;
        _piezoIndexAngleDict = new SerializableDictionary<byte, float>(_piezoIndices, _piezoAngles).Dict;

        _triggerBuffer = _hapCon.GetComponent<TriggerBuffer>();
    }

    public void AddAfterImage(float angle, float metaFrequency)
    {
        _afterimages.Enqueue(new Afterimage(angle, metaFrequency));
    }

    private Afterimage _getClosestAfterimage()
    {
        Afterimage result = _afterimages.Peek();
        foreach(Afterimage ai in _afterimages)
        {
            if (ai.MetaFrequency > result.MetaFrequency)
                result = ai;
        }
        return result;
    }

    public void Update(float stickAngle)
    {
        _feedbackTimer += Time.deltaTime;

        if (_afterimages.Peek().Done)
            _afterimages.Dequeue();
        //Debug.Log(_afterimages.Count);

        foreach (Afterimage afterimage in _afterimages)
            afterimage.Update(Time.deltaTime);

        Afterimage closestAfterimage = _getClosestAfterimage();

        float metaPeriod = 1.0f / closestAfterimage.MetaFrequency;
        if(_feedbackTimer >= metaPeriod)
        {
            _feedbackTimer -= metaPeriod;
            foreach(KeyValuePair<byte, float> pair in _piezoIndexAngleDict)
            {
                byte amplitude = (byte)(255 * closestAfterimage.GaussLike(stickAngle + pair.Value));
                _triggerBuffer.AddCommand(pair.Key, amplitude);
            }
        }
    }
}