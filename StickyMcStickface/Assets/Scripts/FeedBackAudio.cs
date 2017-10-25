using UnityEngine;
using TrapezeGrid;

[RequireComponent(typeof(GridData))]
public class FeedBackAudio : MonoBehaviour {
    public int position = 0;
    public int samplerate = 44100;
    [Range(0,20)]
    private GridData gd;
    AudioSource aud;
    AudioClip.PCMReaderCallback callback;
    AudioClip myClip;

    // Use this for initialization
    void Start () {
        gd = GetComponent<GridData>();
        aud = GetComponent<AudioSource>();
        play();
    }

    // Update is called once per frame
    void Update() {

	}

    public void play()
    {
        callback = delegate (float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Mathf.Sin(i / 1);
            }
        };
        myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, callback, OnAudioSetPosition);
        aud.clip = myClip;
        aud.Play();
    }

    public void Stop()
    {
        aud.Stop();
    }

    public void updateSound(Vector3 hitpoint)
    {
        aud.pitch =  Mathf.Abs((hitpoint.magnitude - gd.Depth) /100);
        play();
    }
    

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }
}
