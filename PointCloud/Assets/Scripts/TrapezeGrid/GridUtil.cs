using UnityEngine;

namespace TrapezeGrid
{

	public static class GridUtil {

	    /**
		 * Translates a cartesian Vector to polar coordinates.
		 * Polar coords are represented as a Vector3 (azimuth, polar, radial)
		 */
		public static Vector3 CartesianToPolar (Vector3 point) {

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
		public static Vector3 PolarToCartesian (Vector3 polar) {

			float xzLength = polar.z * Mathf.Cos(polar.y);

	        return new Vector3(
	            xzLength * Mathf.Sin(polar.x),
	            polar.z  * Mathf.Sin(polar.y),
				xzLength * Mathf.Cos(polar.x)
			);
		}
	}

}
