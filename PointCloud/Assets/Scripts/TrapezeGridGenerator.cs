using System.Collections.Generic;
using UnityEngine;

public class TrapezeGridGenerator : MonoBehaviour {

	[HeaderAttribute("Grid Size")]

	[SerializeField]
	[Range(20.0f, 180.0f)]
    private float _horizontalDegree = 45.0f;

	private float HorizontalRadian {
		get { return _horizontalDegree * Mathf.Deg2Rad; }
	}

	[SerializeField]
	[Range(20.0f, 180.0f)]
    private float _verticalDegree = 45.0f;

	private float VerticalRadian {
		get { return _verticalDegree * Mathf.Deg2Rad; }
	}

    [SerializeField]
    [Range(1.0f, 100.0f)]
 	private float _depth = 10.0f;

	[HeaderAttribute("Depth Dynamics")]

	[SerializeField]
    [Range(0.1f, 10.0f)]
    private float _depthOffset = 1.0f;

    [SerializeField]
    private AnimationCurve _depthCurve;

	[HeaderAttribute("Grid Division")]

    [SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _horizontalSteps = 5;

	[SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _verticalSteps = 5;

	[SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _lengthSteps = 5;

	public uint CellNumber {
		get { return _horizontalSteps * _verticalSteps * _lengthSteps; }
    }

	public float HorizontalStepSize {
		get { return _horizontalDegree / _horizontalSteps; }
	}

	public float VerticalStepSize {
		get { return _verticalDegree / _verticalSteps; }
    }

	public float LengthStepDynamic(uint depth) {
		return _depthCurve.Evaluate((float)depth / _depth);
    }

    private float _squaredCoeff = 1.0f;
	public float LengthStepSquared(uint depth) {
		return _squaredCoeff * depth * depth;
    }

	void Awake () {

    }

    private Vector3 getDirection(uint x, uint y, uint z)
    {
		return new Vector3(
            Mathf.Sin(-HorizontalRadian + 2.0f * x * HorizontalRadian / _horizontalSteps),
            Mathf.Sin(-VerticalRadian   + 2.0f * y * VerticalRadian   / _verticalSteps  ),
            1.0f
        ).normalized;
    }

    private Mesh generateMesh(uint x, uint y, uint z)
	{
		return null;
	}

    private List<Mesh> generateMeshes()
	{
        List<Mesh> meshes = new List<Mesh>();

        for (uint z = 0; z < _lengthSteps; z++) {
            for (uint y = 0; y < _verticalSteps; y++) {
                for (uint x = 0; x < _horizontalSteps; x++) {
					meshes.Add(generateMesh(x, y, z));
				}
            }
        }

		return meshes;
	}
}
