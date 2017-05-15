using UnityEngine;

namespace TrapezeGrid
{
	public class SimpleMeshGenerator : AMeshGenerator
	{
		public SimpleMeshGenerator(GridData gridData) 
			: base(gridData) { }


		public override Mesh GenerateMesh(uint x, uint y, uint z)
		{
			Mesh result = new Mesh();

			Vector3[] vertices = new Vector3[8] {
    			_gridWorldConverter.GridToWorld(x    , y    , z    ), // 0 left -lower-front
    	        _gridWorldConverter.GridToWorld(x + 1, y    , z    ), // 1 right-lower-front
    	        _gridWorldConverter.GridToWorld(x    , y + 1, z    ), // 2 left -upper-front
    	        _gridWorldConverter.GridToWorld(x + 1, y + 1, z    ), // 3 right-upper-front
    	        _gridWorldConverter.GridToWorld(x    , y    , z + 1), // 4 left -lower-back
    	        _gridWorldConverter.GridToWorld(x + 1, y    , z + 1), // 5 right-lower-back
    	        _gridWorldConverter.GridToWorld(x    , y + 1, z + 1), // 6 left -upper-back
    	        _gridWorldConverter.GridToWorld(x + 1, y + 1, z + 1)  // 7 right-upper-back
            };

			int[] triangles = new int[36] {
                4, 2, 6,  4, 0, 2,  // Left
                1, 7, 3,  1, 5, 7,  // Right
                4, 1, 0,  4, 5, 1,  // Bottom
                2, 7, 6,  2, 3, 7,  // Upper
                0, 3, 2,  0, 1, 3,  // Front
                5, 6, 7,  5, 4, 6  // Back
            };

			result.vertices = vertices;
			result.triangles = triangles;
			result.RecalculateNormals();

			return result;
		}
	}
}
