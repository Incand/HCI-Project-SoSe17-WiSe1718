using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class SensorFeedbackHandler {

    protected HapStickController _hapCon;

    protected float _feedbackTimer = 0.0f;
    protected float _timer = 0.0f;
    
    [SerializeField]
    protected byte _piezoIndex = 0;

    protected FeedbackCalculator _feedbackCalculator = new FeedbackCalculator();

    public SensorFeedbackHandler(HapStickController hapCon)
    {
        _hapCon = hapCon;
    }

    protected void _handlePiezoTrigger()
    {
        _hapCon.triggerPiezo(true, _piezoIndex);
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
            _handlePiezoTrigger();
        }
    }
}

public class LaserSensorFeedbackHandler : SensorFeedbackHandler
{
    public LaserSensorFeedbackHandler(HapStickController hapCon)
        : base(hapCon)
    { }

    public override float MetaFrequency
    {
        get { return _feedbackCalculator.GetMetaFrequency(_hapCon.LaserSensorDistance); }
    }
}

public class SonicSensorFeedbackHandler : SensorFeedbackHandler
{
    public SonicSensorFeedbackHandler(HapStickController hapCon)
        : base(hapCon)
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
    
    protected float _feedbackTimer = 0.0f;
    protected float _timer = 0.0f;

    [SerializeField]
    private byte _maxAmplitude = 255;
    
    [SerializeField]
    private List<byte> _piezoIndices = new List<byte>();
    [SerializeField]
    private List<float> _piezoAngles = new List<float>();

    private Dictionary<byte, float> _piezoIndexAngleDict;

    private Queue<Afterimage> _afterimages = new Queue<Afterimage>();

    public AfterimageFeedbackHandler(HapStickController hapCon, bool afterimagesFromSonic=true, bool afterimagesFromLaser=false)
    {
        _hapCon = hapCon;
        _piezoIndexAngleDict = new SerializableDictionary<byte, float>(_piezoIndices, _piezoAngles).Dict;
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
                _hapCon.triggerPiezo(true, pair.Key, amplitude);
            }
        }
    }
}