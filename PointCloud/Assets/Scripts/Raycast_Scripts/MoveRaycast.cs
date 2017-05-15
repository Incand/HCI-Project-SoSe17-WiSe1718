using UnityEngine;
using TrapezeGrid;

[RequireComponent(typeof(GridData))]
public class MoveRaycast : MonoBehaviour
{
    [SerializeField]
    private float frequencyVertical,frequencyHorizontal;
    private GridData gd;
    RaycastHit hit;
    FeedBackAudio fb;

    void Awake()
    {
        gd = GetComponent<GridData>();
    }
    // Use this for initialization
    void Start()
    {
        fb = GetComponent<FeedBackAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        bool treffer = Physics.Raycast(transform.position, getDirection(), out hit, gd.Depth);
        Debug.Log(hit.distance);
        if (treffer) hitting();
        else nohit();
        Debug.DrawRay(transform.position, getDirection(), Color.black, 0.01f);
    }

    float getSineValue(float amplitude, float frequency)
    {
        //sin x;
        return 0.5f * amplitude * Mathf.Sin(frequency * (Time.time * (2 * Mathf.PI)));
    }
    void hitting()
    {
        fb.play(gd.Depth - hit.distance);
    }
    void nohit()
    {
        fb.Stop();
    }
    private Vector3 getDirection()
    {
        float x = getSineValue(gd.WidthAngleRadian,frequencyHorizontal );
        float y = getSineValue(gd.HeightAngleRadian,frequencyVertical);

        return GridWorldConverter.PolarToCartesian(new Vector3(x, y, gd.Depth));
    }
}
