using System.Collections;
using UnityEngine;

namespace TrapezeGrid
{
	public class CellColorizer : MonoBehaviour
	{
		#region FIELDS

		private MeshRenderer _meshRenderer;

		#endregion

		#region EDITOR_INTERFACE

		[SerializeField]
		private Color _inactiveColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

		[SerializeField]
		private Color _activeColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

		[SerializeField]
		private float _fadeTime = 1.0f;

		#endregion

		#region UNITY_EXECUTION_CHAIN_METHODS

		void Awake()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		#endregion

		#region PRIVATE_METHODS

		private IEnumerator Fade(bool isMeshVisible)
		{
			float timer = 0.0f;
			while (timer < _fadeTime)
			{
                setMeshVisible(true);
				_meshRenderer.material.color = Color.Lerp(_activeColor, _inactiveColor, timer / _fadeTime);
				timer += Time.deltaTime;
				yield return null;
			}
            setMeshVisible(isMeshVisible);
		}

		#endregion

		#region PUBLIC_METHODS

		public void Colorize(bool isMeshVisible)
		{
			StartCoroutine(Fade(isMeshVisible));
		}
        public void setMeshVisible(bool visible)
        {
            _meshRenderer.enabled = visible;
        }
		#endregion
	}
}
