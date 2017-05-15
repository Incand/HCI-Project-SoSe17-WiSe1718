using UnityEngine;

namespace TrapezeGrid
{
	public class ComplexMeshGenerator : AMeshGenerator
	{
		public ComplexMeshGenerator(GridData gridData)
			: base(gridData) { }


		public override Mesh GenerateMesh(uint x, uint y, uint z)
		{
			Mesh result = new Mesh();

			Vector3[] vertices = new Vector3[24] {
                // Left
                _gridWorldConverter.GridToWorld(x, y    , z + 1),
				_gridWorldConverter.GridToWorld(x, y    , z    ),
				_gridWorldConverter.GridToWorld(x, y + 1, z + 1),
				_gridWorldConverter.GridToWorld(x, y + 1, z    ),

                // Right
                _gridWorldConverter.GridToWorld(x + 1, y    , z    ),
				_gridWorldConverter.GridToWorld(x + 1, y    , z + 1),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z    ),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z + 1),

                // Bottom
                _gridWorldConverter.GridToWorld(x    , y, z + 1),
				_gridWorldConverter.GridToWorld(x + 1, y, z + 1),
				_gridWorldConverter.GridToWorld(x    , y, z    ),
				_gridWorldConverter.GridToWorld(x + 1, y, z    ),

                // Top
                _gridWorldConverter.GridToWorld(x    , y + 1, z    ),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z    ),
				_gridWorldConverter.GridToWorld(x    , y + 1, z + 1),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z + 1),

                // Front
                _gridWorldConverter.GridToWorld(x    , y    , z),
				_gridWorldConverter.GridToWorld(x + 1, y    , z),
				_gridWorldConverter.GridToWorld(x    , y + 1, z),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z),

                // Back
                _gridWorldConverter.GridToWorld(x + 1, y    , z + 1),
				_gridWorldConverter.GridToWorld(x    , y    , z + 1),
				_gridWorldConverter.GridToWorld(x + 1, y + 1, z + 1),
				_gridWorldConverter.GridToWorld(x    , y + 1, z + 1)
            };


			int[] triangles = new int[36] {
				 0,  2,  3,   0,  3,  1, // Left
                 4,  6,  7,   4,  7,  5, // Right
                 8, 10, 11,   8, 11,  9, // Bottom
                12, 14, 15,  12, 15, 13, // Top
                16, 18, 19,  16, 19, 17, // Front
                20, 22, 23,  20, 23, 21  // Back
            };

			result.vertices = vertices;
			result.triangles = triangles;
			result.RecalculateNormals();

			return result;
		}
	}
}