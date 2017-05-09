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

    [HeaderAttribute("Rendering")]

    [SerializeField]
    private Material _cellMaterial;

	#endregion

	#region PROPERTIES

	private uint CellNumber {
		get { return _horizontalSteps * _verticalSteps * _depthSteps; }
    }


	private float HorizontalStepSizeDegree {
		get { return _gridData.WidthAngleDegree / _horizontalSteps; }
    }

	private float HorizontalStepSizeRadian {
		get { return _gridData.WidthAngleRadian / _horizontalSteps; }
	}


	private float VerticalStepSizeDegree {
		get { return _gridData.HeightAngleDegree / _verticalSteps; }
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

        instantiateCells();
    }

	#endregion

	#region PRIVATE_METHODS

    /**
	 * Returns the lower left corner's world position of the specified cell
	 */
    private Vector3 Grid2World(uint x, uint y, uint z)
	{
        if ( x > _horizontalSteps + 1 ||
             y > _verticalSteps   + 1 ||
             z > _depthSteps      + 1   )
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

        Vector3[] vertices = new Vector3[24] {
            // Left
            Grid2World(x, y    , z + 1),
            Grid2World(x, y    , z    ),
            Grid2World(x, y + 1, z + 1),
            Grid2World(x, y + 1, z    ),

            // Right
            Grid2World(x + 1, y    , z    ),
            Grid2World(x + 1, y    , z + 1),
            Grid2World(x + 1, y + 1, z    ),
            Grid2World(x + 1, y + 1, z + 1),

            // Bottom
            Grid2World(x    , y, z + 1),
            Grid2World(x + 1, y, z + 1),
            Grid2World(x    , y, z    ),
            Grid2World(x + 1, y, z    ),

            // Top
            Grid2World(x    , y + 1, z    ),
            Grid2World(x + 1, y + 1, z    ),
            Grid2World(x    , y + 1, z + 1),
            Grid2World(x + 1, y + 1, z + 1),

            // Front
            Grid2World(x    , y    , z),
            Grid2World(x + 1, y    , z),
            Grid2World(x    , y + 1, z),
            Grid2World(x + 1, y + 1, z),

            // Back
            Grid2World(x + 1, y    , z + 1),
            Grid2World(x    , y    , z + 1),
            Grid2World(x + 1, y + 1, z + 1),
            Grid2World(x    , y + 1, z + 1)

            /*
			Grid2World(x    , y    , z    ), // 0 left -lower-front
	        Grid2World(x + 1, y    , z    ), // 1 right-lower-front
	        Grid2World(x    , y + 1, z    ), // 2 left -upper-front
	        Grid2World(x + 1, y + 1, z    ), // 3 right-upper-front
	        Grid2World(x    , y    , z + 1), // 4 left -lower-back
	        Grid2World(x + 1, y    , z + 1), // 5 right-lower-back
	        Grid2World(x    , y + 1, z + 1), // 6 left -upper-back
	        Grid2World(x + 1, y + 1, z + 1)  // 7 right-upper-back
            */
        };


        int[] triangles = new int[36] {
             0,  2,  3,   0,  3,  1, // Left
             4,  6,  7,   4,  7,  5, // Right
             8, 10, 11,   8, 11,  9, // Bottom
            12, 14, 15,  12, 15, 13, // Top
            16, 18, 19,  16, 19, 17, // Front
            20, 22, 23,  20, 23, 21 // Back

            /*
            4, 2, 6,  4, 0, 2,  // Left
            1, 7, 3,  1, 5, 7,  // Right
            4, 1, 0,  4, 5, 1,  // Bottom
            2, 7, 6,  2, 3, 7,  // Upper
            0, 3, 2,  0, 1, 3,  // Front
            5, 6, 7,  5, 4, 6  // Back
            */
        };


        result.vertices  = vertices;
        result.triangles = triangles;
        result.RecalculateNormals();

        return result;
    }

    private void instantiateCell(uint x, uint y, uint z)
    {
        GameObject cell = new GameObject(string.Format("Cell_{0}_{1}_{2}", x, y, z));
        cell.AddComponent<MeshRenderer>().material = _cellMaterial;
        cell.AddComponent<MeshFilter>().mesh = generateMesh(x, y, z);
        cell.transform.parent = transform;
    }

    private void instantiateCells()
    {
        for (uint z = 0; z < _depthSteps; z++) {
            for (uint y = 0; y < _verticalSteps; y++) {
                for (uint x = 0; x < _horizontalSteps; x++) {
					instantiateCell(x, y, z);
				}
            }
        }
	}

	#endregion
}
