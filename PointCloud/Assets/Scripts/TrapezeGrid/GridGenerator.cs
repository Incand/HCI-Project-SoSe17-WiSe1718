using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrapezeGrid
{
	[ExecuteInEditMode]
    [RequireComponent(typeof(GridData))]
    public class GridGenerator : MonoBehaviour {

		#region PRIVATE_FIELDS

		private GridData _gridData;

		private AMeshGenerator _meshGenerator;

		#endregion

		#region EDITOR_INTERFACE

		[HeaderAttribute("Rendering")]

		[SerializeField]
		private MeshType _meshType;

        [SerializeField]
        private Material _cellMaterial;

    	#endregion

    	#region UNITY_EXECUTION_CHAIN_METHODS

    	void Awake () {
            _gridData = GetComponent<GridData>();
        }

		void Update () {
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
			cell.transform.parent = transform;
		}

        private void instantiateCells()
        {
            for (uint z = 0; z < _gridData.DepthSteps; z++) {
                for (uint y = 0; y < _gridData.VerticalSteps; y++) {
                    for (uint x = 0; x < _gridData.HorizontalSteps; x++) {
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
					_meshGenerator = new SimpleMeshGenerator(_gridData); 
					break;
				case MeshType.COMPLEX: 
					_meshGenerator = new ComplexMeshGenerator(_gridData); 
					break;
			}
		}

    	#endregion
    }

	public enum MeshType { SIMPLE, COMPLEX };
}
