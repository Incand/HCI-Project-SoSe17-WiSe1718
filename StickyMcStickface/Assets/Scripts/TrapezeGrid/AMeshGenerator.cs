using UnityEngine;

namespace TrapezeGrid
{
	public abstract class AMeshGenerator
	{
		protected GridWorldConverter _gridWorldConverter;


		protected AMeshGenerator(GridData gridData)
		{
			_gridWorldConverter = new GridWorldConverter(gridData);
		}

		protected AMeshGenerator(GridWorldConverter gridWorldConverter)
		{
			_gridWorldConverter = gridWorldConverter;
		}


		public abstract Mesh GenerateMesh(uint x, uint y, uint z);
	}
}
