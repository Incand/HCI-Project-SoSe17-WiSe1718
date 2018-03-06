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
    private Dictionary<byte, float> _piezoIndexAngleMap = new Dictionary<byte, float>();

    private Queue<Afterimage> _afterimages = new Queue<Afterimage>();

    public AfterimageFeedbackHandler(HapStickController hapCon, bool afterimagesFromSonic=true, bool afterimagesFromLaser=false)
    {
        _hapCon = hapCon;
    }

    public void AddAfterImage(float angle, float metaFrequency)
    {
        _afterimages.Enqueue(new Afterimage(angle, metaFrequency));
    }

    private Afterimage _getClosestAfterimage()
    {
        IEnumerator<Afterimage> en = _afterimages.GetEnumerator();
        Afterimage result = en.Current;
        while (en.MoveNext())
        {
            if (en.Current.MetaFrequency > result.MetaFrequency)
                result = en.Current;
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

        foreach(KeyValuePair<byte, float> pair in _piezoIndexAngleMap)
        {
            byte amplitud = (byte)(255 * closestAfterimage.GaussLike(stickAngle));
            _hapCon.triggerPiezo(true, )
        }

        foreach (byte index in _piezoIndexMap)
        {
            _hapCon.amplitud = 
            _hapCon.triggerPiezo(true, index);
        }
            break;

        /*
        float metaPeriod = 1.0f / _getMetaFrequency();

        if(_feedbackTimer >= metaPeriod)
        {
            _feedbackTimer -= metaPeriod;
            _handlePiezoTrigger();
        }
        */
    }
}