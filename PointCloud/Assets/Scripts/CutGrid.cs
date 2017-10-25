using System.Collections;
using System.Collections.Generic;
using TrapezeGrid;
using UnityEngine;

public class CutGrid : MonoBehaviour {

    //Add script to the Feedback Loop, if cut yields true, then there is no Feedback neccessary.
    // Use Case: Ground and rooms/objects above 2.2m
    //Potential problem : Sensor might be angled already 
    float distance = 2.0f;
    float angle = 90.0f;
        // Use this for initialization
	void Start () {
        //getAngle from IMU
        //getDistance from Sensor

        //cut the ground, too
        //height of Sensor and angle
	}
	public bool cut()
    {
        //TODO: substract height from calculateIMU rotation to manage different cone rotations.
        // check whether sensor data exceeds relevant object detection heights. 
        if (angle > 0)
            return (calculateGradient() * distance > 2.2f); // over-head
        else
            return (calculateGradient() * distance < 0.1f); // ground
    }
    public float calculateGradient()
    {
        return (angle<90.0f) ? Mathf.Tan(angle) : -1.0f* Mathf.Tan(180.0f-angle);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
