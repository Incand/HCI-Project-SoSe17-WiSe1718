using UnityEngine;

public class FeedBackAudio : MonoBehaviour {
    public int position = 0;
    public int samplerate = 44100;
    int globalDistance;
    AudioSource aud;
    AudioClip myClip;
    // Use this for initialization
    void Start () {
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

	}

    public void play(float distance)
    {
        globalDistance = (int)distance;
        myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        aud.clip = myClip;
        aud.Play();
    }

    public void Stop()
    {
        aud.Stop();
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * globalDistance * position / samplerate));
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }
}
