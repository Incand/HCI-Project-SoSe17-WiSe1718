using UnityEngine;

public class TrapezeGridData : MonoBehaviour {

	[SerializeField]
	[Range(20.0f, 180.0f)]
    private float _widthAngleDegree = 90.0f;

	public float WidthAngleDegree {
		get { return _widthAngleDegree; }
    }

	public float WidthAngleRadian {
		get {return _widthAngleDegree * Mathf.Deg2Rad; }
	}

    [SerializeField]
	[Range(20.0f, 180.0f)]
	private float _heightAngleDegree = 90.0f;

	public float HeightAngleDegree {
		get { return _heightAngleDegree; }
	}

	public float HeightAngleRadian {
		get { return _heightAngleDegree * Mathf.Deg2Rad; }
	}
}
