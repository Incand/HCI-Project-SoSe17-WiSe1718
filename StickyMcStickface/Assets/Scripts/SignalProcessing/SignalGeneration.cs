using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SignalGeneration : MonoBehaviour {

	[Range(10, 1000)]
	[SerializeField]
	private uint _graphLength = 100;

	[SerializeField]
	private AnimationCurve _graph = new AnimationCurve ();

	private const float ONE_SIXTIETH = 1.0f / 60.0f;

	private float _currentValue = 0.0f;
	public float CurrentValue {
		get { return _currentValue; }
	}

        private class NewSignalEvent : UnityEvent<float>{}
	//[SerializeField]
	//private NewSignalEvent _onNewSignalValue = new NewSignalEvent();

	void FixedUpdate() {
		float lastTime = 0.0f;
		while(true) {
			float time = Time.time;
			_currentValue = Mathf.PerlinNoise(time, 0.0f);

			if (_graph.length < _graphLength) {
				_graph.AddKey (time, _currentValue);
			} else {
				for(int i = 0; i < _graph.length-1; i++){
					_graph.MoveKey (i, new Keyframe (_graph.keys[i].time, _graph.keys [i+1].value));
				}

				int last = _graph.length - 1;
				_graph.MoveKey (
					last, 
					new Keyframe (_graph.keys[last].time, _currentValue)
				);
		        }
		}
	}

	private float GeneratePerlinValue(){
		return 0.0f;
	}
}
