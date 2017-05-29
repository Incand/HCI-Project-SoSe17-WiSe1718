using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrapezeGrid
{
	//[ExecuteInEditMode]
	[RequireComponent(typeof(GridData))]
	public class GridGenerator : MonoBehaviour
	{

		#region PRIVATE_FIELDS

		private GridData _gridData;

		private GridWorldConverter _gridWorldConverter;

		private AMeshGenerator _meshGenerator;
        [SerializeField]
		private CellColorizer[,,] _cellColorizers;

		#endregion

		#region EDITOR_INTERFACE

		[HeaderAttribute("Rendering")]

		[SerializeField]
		private MeshType _meshType = MeshType.COMPLEX;

		[SerializeField]
		private Material _cellMaterial;

		#endregion

		#region UNITY_EXECUTION_CHAIN_METHODS

		void Awake()
		{
			_gridData = GetComponent<GridData>();
			_gridWorldConverter = new GridWorldConverter(_gridData);

            removeCells();
            setMeshGenerator();
            instantiateCells();
        }

		void Update()
		{
			if (Application.isPlaying)
				return;

			removeCells();
			setMeshGenerator();
			instantiateCells();
		}

		#endregion

		#region PRIVATE_METHODS

		private void instantiateCell(uint x, uint y, uint z)
		{
			GameObject cell = new GameObject(string.Format("Cell_{0}_{1}_{2}", x, y, z));

			cell.AddComponent<MeshRenderer>().material = _cellMaterial;
			cell.AddComponent<MeshFilter>().mesh = _meshGenerator.GenerateMesh(x, y, z);
			_cellColorizers[z, y, x] = cell.AddComponent<CellColorizer>();

			cell.transform.position = transform.position;
			//cell.transform.rotation = transform.rotation;
			cell.transform.parent = transform;
		}

		private void instantiateCells()
		{
			_cellColorizers = new CellColorizer[_gridData.DepthSteps, _gridData.VerticalSteps, _gridData.HorizontalSteps];

			for (uint z = 0; z < _gridData.DepthSteps; z++)
			{
				for (uint y = 0; y < _gridData.VerticalSteps; y++)
				{
					for (uint x = 0; x < _gridData.HorizontalSteps; x++)
					{
						instantiateCell(x, y, z);
					}
				}
			}
		}


		private void removeCells()
		{
			int childCount = transform.childCount;
			for (int i = childCount - 1; i >= 0; i--)
				DestroyImmediate(transform.GetChild(i).gameObject);
		}


		private void setMeshGenerator()
		{
			switch (_meshType)
			{
				case MeshType.SIMPLE:
					_meshGenerator = new SimpleMeshGenerator(_gridWorldConverter);
					break;
				case MeshType.COMPLEX:
					_meshGenerator = new ComplexMeshGenerator(_gridWorldConverter);
					break;
			}
		}

		#endregion

		#region PUBLIC_METHODS

		public void ColorizeCell(Vector3 position)
		{
			int[] indices = _gridWorldConverter.WorldToGrid(position);
			Debug.DrawLine(transform.position, position, Color.blue, 2.0f, false);
			Debug.Log(String.Format("x:{0}, y:{1}, z:{2}", indices[2], indices[1], indices[0]));
			((CellColorizer)_cellColorizers.GetValue(indices)).Colorize();
		}

		#endregion

	}

	public enum MeshType { SIMPLE, COMPLEX };
}
