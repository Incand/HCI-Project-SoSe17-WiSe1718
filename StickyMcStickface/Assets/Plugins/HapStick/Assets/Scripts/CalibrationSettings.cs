
/**
 *  Haptics Framework
 *
 *  Copyright 2017 by UHH HCI 
 *  Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

[Serializable()]
public class CalibrationSettings : ISerializable
{
    public ushort[] CalibrationOffsets = new ushort[11];
    
    public CalibrationSettings() { }

    public CalibrationSettings(SerializationInfo info, StreamingContext ctxt)
    {
        CalibrationOffsets = (ushort[])info.GetValue("CalibrationOffsets", typeof(ushort[]));
    }

    // Required by the ISerializable class to be properly serialized. This is called automatically
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
        // Repeat this for each var defined in the Values section
        info.AddValue("CalibrationOffsets", CalibrationOffsets);
    }
}

// === This is the class that will be accessed from scripts ===
public class CalibrationSettingsSaveLoad
{

    public static string currentFilePath = "Calibration.settings";    // Edit this for different save files

    public static void Save(ushort[] data)  // Overloaded
    {
        CalibrationSettings cs = new CalibrationSettings();
        cs.CalibrationOffsets = data;

        Save(currentFilePath, cs);
    }

    public static void Save(CalibrationSettings data)  // Overloaded
    {
        Save(currentFilePath, data);
    }
    public static void Save(string filePath, CalibrationSettings data)
    {
        Stream stream = File.Open(filePath, FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        bformatter.Serialize(stream, data);
        stream.Close();
    }

    // Call this to load from a file into "data"
    public static CalibrationSettings Load() { return Load(currentFilePath); }   // Overloaded
    public static CalibrationSettings Load(string filePath)
    {
        CalibrationSettings data = new CalibrationSettings();
        Stream stream = File.Open(filePath, FileMode.Open);
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        data = (CalibrationSettings)bformatter.Deserialize(stream);
        stream.Close();

        return data;
        // Now use "data" to access your Values
    }

}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
        {
            Type typeToDeserialize = null;

            assemblyName = Assembly.GetExecutingAssembly().FullName;

            // The following line of code returns the type. 
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }

        return null;
    }
}
