using System;
using UnityEngine;

namespace TrapezeGrid
{
	public class GridWorldConverter
	{
		private GridData _gridData;


		public GridWorldConverter(GridData gridData)
		{
			_gridData = gridData;
		}


		#region PUBLIC_METHODS

		/**
    	 * Returns the lower left corner's world position of the specified cell
    	 */
		public Vector3 GridToWorld(uint x, uint y, uint z)
		{
			if (x > _gridData.HorizontalSteps ||
				y > _gridData.VerticalSteps   ||
				z > _gridData.DepthSteps        )
				throw new IndexOutOfRangeException();

			// (azimuth, polar, radial)
			Vector3 polarCoords = new Vector3(
				-0.5f * _gridData.WidthAngleRadian  + x * _gridData.HorizontalStepSizeRadian,
				-0.5f * _gridData.HeightAngleRadian + y * _gridData.VerticalStepSizeRadian,
						_gridData.DepthOffset       + z * _gridData.DepthStepSizeLinear
			);

			return _gridData.transform.localToWorldMatrix * PolarToCartesian(polarCoords);
		}


		/**
		 * Returns the indices of the 3D-grid, the given position were in
		 */
		public int[] WorldToGrid(Vector3 position)
		{
			Vector3 polar = CartesianToPolar(_gridData.transform.InverseTransformPoint(position));

			int[] result = new int[3] {
				(int)((polar.z -        _gridData.DepthOffset      ) / _gridData.DepthStepSizeLinear     ),
				(int)((polar.y + 0.5f * _gridData.HeightAngleRadian) / _gridData.VerticalStepSizeRadian  ),
				(int)((polar.x + 0.5f * _gridData.WidthAngleRadian ) / _gridData.HorizontalStepSizeRadian)
			};

			return result;
		}

		#endregion

		#region STATIC_FUNCTIONS

		/**
		 * Translates a cartesian Vector to polar coordinates.
		 * Polar coords are represented as a Vector3 (azimuth, polar, radial)
		 */
		public static Vector3 CartesianToPolar(Vector3 point)
		{
			float xzLength = new Vector2(point.x, point.z).magnitude;

			return new Vector3(
				Mathf.Atan2(point.x, point.z),
				Mathf.Atan2(point.y, xzLength),
				point.magnitude
			);
		}

		/**
		 * Translates a polar Vector (azimuth, polar, radial) to cartesian
		 * coordinates.
		 */
		public static Vector3 PolarToCartesian(Vector3 polar)
		{
			float xzLength = polar.z * Mathf.Cos(polar.y);

			return new Vector3(
				xzLength * Mathf.Sin(polar.x),
				polar.z  * Mathf.Sin(polar.y),
				xzLength * Mathf.Cos(polar.x)
			);
		}

		#endregion
	}
}
