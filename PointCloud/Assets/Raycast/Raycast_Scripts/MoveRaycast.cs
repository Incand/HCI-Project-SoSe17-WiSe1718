using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRaycast : MonoBehaviour {
    Vector3 origin, direction;
    GameObject sphere;
    public float amplitudeVertical,amplitudeHorizontal;
    public float frequencyVertical,frequencyHorizontal, distance;
    RaycastHit hit;
	// Use this for initialization
	void Start () {
        origin = gameObject.transform.position;
        direction = new Vector3(0,0,distance);
        sphere = GameObject.Find("Sphere");
	}
	
	// Update is called once per frame
	void Update () {
        direction = getDirection();
        bool treffer=Physics.Raycast(origin, direction, out hit);
        Debug.Log(hit.distance);
        sphere.transform.position = treffer ? hit.point:direction;

        Debug.DrawRay(origin, direction,  Color.black, 0.01f);
}
    float getSineValue(float frequency,float amplitude)
    {
        //sin x;
        return amplitude*Mathf.Sin(frequency* (Time.time / (2 * Mathf.PI)));
    }
    Vector3 getDirection()
    {
        direction.y = getSineValue(frequencyVertical,amplitudeVertical);
        direction.x = getSineValue(frequencyHorizontal,amplitudeHorizontal);
        return direction;
    }
}
