using UnityEngine;

[CreateAssetMenu(fileName = "DataContainer")]
public class DataContainer : ScriptableObject {

	public float frequencyVertical,frequencyHorizontal;
	public float verticalDegree;
	public float horizontalDegree;
	public float distance;
	public float VerticalRadian {
		get { return verticalDegree * Mathf.Deg2Rad; }
	}
	public float HorizontalRadian {
		get { return horizontalDegree * Mathf.Deg2Rad; }
	}
}
