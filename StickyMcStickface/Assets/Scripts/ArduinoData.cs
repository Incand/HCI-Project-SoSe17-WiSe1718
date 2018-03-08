using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.IO;
public class ArduinoData : MonoBehaviour {
    System.IO.StreamReader reader;
    FileStream stream;
    string path = "D:/Benutzer/MCIProject_FSH/data.txt";
    // Use this for initialization
    void Start () {
        stream = new FileStream(path,FileMode.Open,FileAccess.Read);
        reader = new StreamReader(stream);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(reader.ReadLine().Last());
    }
    void OnApplicationQuit()
    {
        stream.Close();
        reader.Close();
    }
}
