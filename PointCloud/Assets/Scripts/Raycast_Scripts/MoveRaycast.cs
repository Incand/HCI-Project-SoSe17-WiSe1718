using UnityEngine;
using UnityEngine.Events;
using System;
using TrapezeGrid;

[RequireComponent(typeof(GridData))]
public class MoveRaycast : MonoBehaviour
{
    [SerializeField]
    private float frequencyVertical,frequencyHorizontal;
    private GridData gd;
    [Serializable]
    public class HitEvent : UnityEvent<Vector3> { }
    public HitEvent OnHitEnter,OnHitStay,OnHitExit,NoHitStay;
    private bool lastHit = false;
    RaycastHit hit;
    FeedBackAudio fb;

    void Awake()
    {
        gd = GetComponent<GridData>();
    }
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool treffer = Physics.Raycast(transform.position, getDirection(), out hit, gd.Depth);
        Debug.Log(treffer);
        if (treffer&& !lastHit) {
            OnHitEnter.Invoke(hit.point);
                }
        else if(treffer && lastHit)
        {
            OnHitStay.Invoke(hit.point);
        }
        else if(!treffer && lastHit)
        {
            OnHitExit.Invoke(new Vector3(0,0,gd.Depth));
        }
        Debug.DrawRay(transform.position, getDirection(), Color.black, 0.01f);
        lastHit = treffer;
    }

    float getSineValue(float amplitude, float frequency)
    {   
        return 0.5f * amplitude * Mathf.Sin(frequency * (Time.time * (2 * Mathf.PI)));
    }
   
    private Vector3 getDirection()
    {
        float x = getSineValue(gd.WidthAngleRadian,frequencyHorizontal );
        float y = getSineValue(gd.HeightAngleRadian,frequencyVertical);

        return GridWorldConverter.PolarToCartesian(new Vector3(x, y, gd.Depth));
    }
}
