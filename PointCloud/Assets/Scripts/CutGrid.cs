using System.Collections;
using System.Collections.Generic;
using TrapezeGrid;
using UnityEngine;

public class CutGrid : MonoBehaviour {

    //Add script to the Feedback Loop, if cut yields true, then there is no Feedback neccessary.
    //Potential problem : Sensor might be angled already 
    public GridData griddata;
	// Use this for initialization
	void Start () {
        //getAngle from IMU
        //getDistance from Sensor
	}
	public bool cut()
    {
        return (calculateGradient() * distance >2.2f);  
    }
    public float calculateGradient()
    {
        return (angle<90.0f) ? Mathf.Tan(angle) : -1.0f* Mathf.Tan(180.0f-angle);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
