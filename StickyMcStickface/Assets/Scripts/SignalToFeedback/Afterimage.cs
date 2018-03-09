using UnityEngine;

public class Afterimage
{
    public static float MAX_LIFETIME;
    public static float GAUSS_STD_DEV;

    private float _lifeTime = 0.0f;

    private bool _done;
    public bool Done { get { return _done; } }

    private float _angle;
    public float Angle
    {
        get { return _angle; }
    }

    private float _metaFrequency;
    public float MetaFrequency
    {
        get { return _metaFrequency; }
    }
        
    public Afterimage(float angle, float metaFrequency)
    {
        _angle = angle;
        _metaFrequency = metaFrequency;
    }

    private float _getFalloff()
    {
        return 1.0f - _lifeTime / MAX_LIFETIME;
    }

    public void Update(float time)
    {
        _lifeTime += time;
        if (_lifeTime > MAX_LIFETIME)
            _done = true;
    }

    // CAUTION: Potential error (_angle - x)
    public float GaussLike(float x)
    {
        float exp = 1.0f / GAUSS_STD_DEV * (_angle - x);
        return _getFalloff() * Mathf.Exp(-0.5f * (exp * exp));
    }
}