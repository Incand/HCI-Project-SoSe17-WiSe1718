using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(TrapezeGridData))]
public class TrapezeGridGenerator : MonoBehaviour {

    private TrapezeGridData _gridData;

	#region EDITOR_INTERFACE

	[HeaderAttribute("Grid Division")]

    [SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _horizontalSteps = 5;

	[SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _verticalSteps = 5;

	[SerializeField]
    [Range(1.0f, 30.0f)]
    private uint _depthSteps = 5;

	#endregion

	#region PROPERTIES

	private uint CellNumber {
		get { return _horizontalSteps * _verticalSteps * _depthSteps; }
    }


	private float HorizontalStepSizeDegree {
		get { return _gridData.WidthAngleDegree / _horizontalSteps; }
    }

	private float HorizontalStepSizeRadian {
		get { return _gridData.HeightAngleDegree / _horizontalSteps; }
	}


	private float VerticalStepSizeDegree {
		get { return _gridData.WidthAngleDegree / _verticalSteps; }
    }

	private float VerticalStepSizeRadian {
		get { return _gridData.HeightAngleRadian / _verticalSteps; }
    }


	private float DepthStepSizeLinear {
		get { return (_gridData.Depth - _gridData.DepthOffset) / _depthSteps; }
	}

	#endregion

	#region UNITY_EXECUTION_CHAIN_METHODS

	void Awake () {
		_gridData = GetComponent<TrapezeGridData>();
    }

	#endregion

	#region PRIVATE_METHODS

    /**
	 * Returns the lower left corner's world position of the specified cell
	 */
    private Vector3 Grid2World(uint x, uint y, uint z)
	{
        if ( x > _horizontalSteps ||
             y > _verticalSteps   ||
             z > _depthSteps        )
            throw new IndexOutOfRangeException();

		// (azimuth, polar, radial)
		Vector3 polarCoords = new Vector3(
			-0.5f * _gridData.WidthAngleRadian  + x * HorizontalStepSizeRadian,
			-0.5f * _gridData.HeightAngleRadian + y * VerticalStepSizeRadian,
			_gridData.DepthOffset + z * DepthStepSizeLinear
		);

		return TrapezeGridUtil.PolarToCartesian(polarCoords);
    }


    private Vector3 World2Grid(Vector3 position)
    {
		throw new NotImplementedException();
    }


    private Mesh generateMesh(uint x, uint y, uint z)
    {
        Mesh result = new Mesh();

        Vector3[] vertices = new Vector3[8] {
			Grid2World(x    , y    , z    ), // 0 left -lower-front
	        Grid2World(x + 1, y    , z    ), // 1 right-lower-front
	        Grid2World(x    , y + 1, z    ), // 2 left -upper-front
	        Grid2World(x + 1, y + 1, z    ), // 3 right-upper-front
	        Grid2World(x    , y    , z + 1), // 4 left -lower-back
	        Grid2World(x + 1, y    , z + 1), // 5 right-lower-back
	        Grid2World(x    , y + 1, z + 1), // 6 left -upper-back
	        Grid2World(x + 1, y + 1, z + 1)  // 7 right-upper-back
        };

        int[] triangles = new int[36] {
            4, 6, 2,  4, 2, 0, // Left
            1, 3, 7,  1, 7, 5, // Right
            4, 0, 1,  4, 1, 5, // Bottom
            2, 6, 7,  2, 7, 3, // Upper
            0, 2, 3,  0, 3, 1, // Front
            5, 7, 6,  5, 6, 4  // Back
        };

        result.vertices  = vertices;
        result.triangles = triangles;
        result.RecalculateNormals();

        return result;
    }

    private List<Mesh> generateMeshes()
	{
        List<Mesh> meshes = new List<Mesh>();

        for (uint z = 0; z < _depthSteps; z++) {
            for (uint y = 0; y < _verticalSteps; y++) {
                for (uint x = 0; x < _horizontalSteps; x++) {
					meshes.Add(generateMesh(x, y, z));
				}
            }
        }

		return meshes;
	}

	#endregion
}
