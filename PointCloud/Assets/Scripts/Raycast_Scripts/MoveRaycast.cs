﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRaycast : MonoBehaviour {

    [SerializeField]
    private DataContainer dc;
    private Vector3 origin, direction;
    GameObject sphere, sphereHeight, sphereWidth;
    private float amplitudeVertical, amplitudeHorizontal;
    private float frequencyVertical, frequencyHorizontal, distance;
    RaycastHit hit;
    FeedBackAudio fb;

    // Use this for initialization
    void Start () {
        origin = gameObject.transform.position;
        sphere = GameObject.Find("Sphere");
        sphereHeight = GameObject.Find("SphereLimitHeight");
        sphereWidth = GameObject.Find("SphereLimitWidth");
        fb = GetComponent<FeedBackAudio>();
        distance = dc.distance;
        amplitudeVertical = dc.width / 2;
        amplitudeHorizontal = dc.height / 2;
        frequencyVertical =dc.frequencyVertical;
        frequencyHorizontal = dc.frequencyHorizontal;
        direction = new Vector3(0, 0, distance);
        sphereHeight.transform.position = new Vector3(direction.x,amplitudeVertical, direction.z);
        sphereWidth.transform.position = new Vector3(amplitudeHorizontal, direction.y, direction.z);

    }

    // Update is called once per frame
    void Update () {
        direction = getDirection();
        bool treffer=Physics.Raycast(origin, direction, out hit,distance);
        Debug.Log(hit.distance);
        if (treffer) hitting();
        else nohit();
        Debug.DrawRay(origin, direction,  Color.black, 0.01f);
}
    float getSineValue(float frequency,float amplitude)
    {
        //sin x;
        return amplitude*Mathf.Sin(frequency* (Time.time / (2 * Mathf.PI)));
    }
    void hitting()
    {
        sphere.transform.position = hit.point;
        fb.play(distance-hit.distance);
    }
    void nohit()
    {
        sphere.transform.position = direction;
        fb.Stop();

    }
    private Vector3 getDirection()
    {
        direction.y = getSineValue(frequencyVertical,amplitudeVertical);
        direction.x = getSineValue(frequencyHorizontal,amplitudeHorizontal);
        return direction;
    }
}