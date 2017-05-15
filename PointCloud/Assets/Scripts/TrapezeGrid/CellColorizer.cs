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

		void Update()
		{
			if (Random.value < 0.001f)
				StartFade();
		}

		#endregion

		#region PRIVATE_METHODS

		private IEnumerator Fade()
		{
			float timer = 0.0f;
			while (timer < _fadeTime)
			{
				_meshRenderer.material.color = Color.Lerp(_activeColor, _inactiveColor, timer / _fadeTime);
				timer += Time.deltaTime;
				yield return null;
			}
		}

		#endregion

		#region PUBLIC_METHODS

		public void StartFade()
		{
			Debug.Log("Blub");
			StartCoroutine(Fade());
		}

		#endregion
	}
}
