using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBuffer : MonoBehaviour {
   
    private HapStickController _hapCon;

    private Queue<string> _buffer = new Queue<string>();
    private byte count = 0;
    [SerializeField]
    private float _triggerDelay = 0.005f;

	// Use this for initialization
	void Start () {
        _hapCon = GetComponent<HapStickController>();
        StartCoroutine(workBuffer());
	}

    private IEnumerator workBuffer()
    {
        while (true)
        {
            if (_buffer.Count > 0)
                _hapCon.triggerPiezo(_buffer.Dequeue());
            yield return new WaitForSeconds(_triggerDelay);       
        }
    }

    public void AddCommand(int index)
    {
        _buffer.Enqueue(_hapCon.buildCommand(index));
    }

    public void AddCommand(int index, byte amplitud)
    {
        _buffer.Enqueue(_hapCon.buildCommand(index, amplitud));
    }
}
