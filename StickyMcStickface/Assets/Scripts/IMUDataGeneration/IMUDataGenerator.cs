using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUDataGenerator : MonoBehaviour {

	[SerializeField]
	private Transform _imu;

	[SerializeField]
	private uint _participantId = 0;

	[SerializeField]
	private uint _courseId = 0;

	[SerializeField]
	private string _dataPath;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
