using UnityEngine;

namespace TrapezeGrid
{
	public class GridData : MonoBehaviour
	{
		#region EDITOR_INTERFACE

		[HeaderAttribute("Grid Dimensions")]

		[SerializeField]
		[Range(20.0f, 180.0f)]
		private float _widthAngleDegree = 90.0f;

		[SerializeField]
		[Range(20.0f, 180.0f)]
		private float _heightAngleDegree = 90.0f;

		[SerializeField]
		[Range(2.0f, 20.0f)]
		private float _depth = 10.0f;

		[SerializeField]
		[Range(0.1f, 2.0f)]
		private float _depthOffset = 1.0f;


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

		public float WidthAngleDegree
		{
			get { return _widthAngleDegree; }
		}

		public float WidthAngleRadian
		{
			get { return _widthAngleDegree * Mathf.Deg2Rad; }
		}


		public uint CellNumber
		{
			get { return _horizontalSteps * _verticalSteps * _depthSteps; }
		}


		public float HorizontalStepSizeDegree
		{
			get { return WidthAngleDegree / _horizontalSteps; }
		}

		public float HorizontalStepSizeRadian
		{
			get { return WidthAngleRadian / _horizontalSteps; }
		}


		public float HeightAngleDegree
		{
			get { return _heightAngleDegree; }
		}

		public float HeightAngleRadian
		{
			get { return _heightAngleDegree * Mathf.Deg2Rad; }
		}


		public float Depth
		{
			get { return _depth; }
		}


		public float DepthOffset
		{
			get { return _depthOffset; }
		}


		public uint HorizontalSteps
		{
			get { return _horizontalSteps; }
		}


		public uint VerticalSteps
		{
			get { return _verticalSteps; }
		}


		public uint DepthSteps
		{
			get { return _depthSteps; }
		}

		public float VerticalStepSizeDegree
		{
			get { return HeightAngleDegree / _verticalSteps; }
		}

		public float VerticalStepSizeRadian
		{
			get { return HeightAngleRadian / _verticalSteps; }
		}


		public float DepthStepSizeLinear
		{
			get { return (Depth - DepthOffset) / _depthSteps; }
		}

		#endregion
	}

}
