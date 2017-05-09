using UnityEngine;

public class TrapezeGridData : MonoBehaviour {

	[HeaderAttribute("Grid Dimensions")]

    #region WIDTH_ANGLE

    [SerializeField]
	[Range(20.0f, 180.0f)]
    private float _widthAngleDegree = 90.0f;

	public float WidthAngleDegree {
		get { return _widthAngleDegree; }
    }

	public float WidthAngleRadian {
		get {return _widthAngleDegree * Mathf.Deg2Rad; }
    }

    #endregion

	#region HEIGHT_ANGLE

    [SerializeField]
	[Range(20.0f, 180.0f)]
	private float _heightAngleDegree = 90.0f;

	public float HeightAngleDegree {
		get { return _heightAngleDegree; }
	}

	public float HeightAngleRadian {
		get { return _heightAngleDegree * Mathf.Deg2Rad; }
    }

    #endregion

    #region DEPTH

	[SerializeField]
	[Range(2.0f, 20.0f)]
    private float _depth = 10.0f;

	public float Depth {
		get { return _depth; }
    }


    [SerializeField]
    [Range(0.1f, 2.0f)]
    private float _depthOffset = 1.0f;

	public float DepthOffset {
		get { return _depthOffset; }
	}

	#endregion

}
