using UnityEngine;
using UnityEngine.Events;
using System;
using TrapezeGrid;

[RequireComponent(typeof(GridData))]
public class MoveRaycast : MonoBehaviour
{
	#region FIELDS

	[Serializable]
	public class HitEvent : UnityEvent<Vector3> { }

	private GridData _gridData;
   

    private bool lastHit = false;

	#endregion

	#region EDITOR_INTERFACE

	[HeaderAttribute("Raycast Movement Speed")]

	[SerializeField]
	[Range(0.0f, 15.0f)]
	private float frequencyVertical = 2.5f;

	[SerializeField]
	[Range(0.0f, 2.0f)]
	private float frequencyHorizontal = 0.25104398561f;


	[HeaderAttribute("Events")]

	[SerializeField]
	private HitEvent OnHitEnter;

	[SerializeField]
	private HitEvent OnHitStay;

	[SerializeField]
	private HitEvent OnHitExit;

	[SerializeField]
	private HitEvent NoHitStay;

    //distance in cm
    private SerialComm serialComm;
	#endregion

	#region UNITY_EXECUTION_CHAIN_METHODS

	void Awake()
	{
		_gridData = GetComponent<GridData>();
        serialComm = GetComponent<SerialComm>();
	}

	// Update is called once per frame
	void Update()
	{
        float x = getSineValue(_gridData.WidthAngleRadian, frequencyHorizontal);
        float y = getSineValue(_gridData.HeightAngleRadian, frequencyVertical);

        bool hitSomething = serialComm.distance < _gridData.Depth*100;
        Vector3 polar = transform.localToWorldMatrix * GridWorldConverter.PolarToCartesian( new Vector3(x, y, serialComm.distance/100.0f));
        Debug.Log(polar);

        if (hitSomething && !lastHit)
		{
			OnHitEnter.Invoke(polar);
		}
		else if (hitSomething && lastHit)
		{
			OnHitStay.Invoke(polar);
		}
		else if (!hitSomething && lastHit)
		{
			OnHitExit.Invoke(new Vector3(0.0f, 0.0f, _gridData.Depth));
		}

		lastHit = hitSomething;
	}

	#endregion

	#region PRIVATE_METHODS

	private float getSineValue(float amplitude, float frequency)
	{
		return 0.5f * amplitude * Mathf.Sin(frequency * (Time.time * (2 * Mathf.PI)));
	}


	private Vector3 getDirection()
	{
		float x = getSineValue(_gridData.WidthAngleRadian, frequencyHorizontal);
		float y = getSineValue(_gridData.HeightAngleRadian, frequencyVertical);

		return transform.localToWorldMatrix * GridWorldConverter.PolarToCartesian(new Vector3(x, y, _gridData.Depth));
	}

	#endregion
}
