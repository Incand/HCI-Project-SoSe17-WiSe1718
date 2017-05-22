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

	#endregion

	#region UNITY_EXECUTION_CHAIN_METHODS

	void Awake()
	{
		_gridData = GetComponent<GridData>();
	}

	// Update is called once per frame
	void Update()
	{
		RaycastHit hit;
		bool hitSomething = Physics.Raycast(transform.position, getDirection(), out hit, _gridData.Depth);

		if (hitSomething && !lastHit)
		{
			OnHitEnter.Invoke(hit.point);
		}
		else if (hitSomething && lastHit)
		{
			OnHitStay.Invoke(hit.point);
		}
		else if (!hitSomething && lastHit)
		{
			OnHitExit.Invoke(new Vector3(0.0f, 0.0f, _gridData.Depth));
		}

		Debug.DrawRay(transform.position, getDirection(), Color.black, 0.01f);
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

		return GridWorldConverter.PolarToCartesian(new Vector3(x, y, _gridData.Depth));
	}

	#endregion
}
