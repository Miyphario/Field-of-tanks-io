using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class SaveData
{
	public bool gameTutorial = true;
	public float masterVolume = 1f;
	public bool batterySaving = false;

    public byte[] Serialize()
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        writer.Write(gameTutorial);
        writer.Write(masterVolume);
        writer.Write(batterySaving);
        return stream.ToArray();
    }

    public static SaveData Deserealize(byte[] data)
    {
        if (data.Length <= 0) return default;

        SaveData result = new();
        using BinaryReader reader = new(new MemoryStream(data));
        result.gameTutorial = reader.ReadBoolean();
        result.masterVolume = reader.ReadSingle();
        result.batterySaving = reader.ReadBoolean();
        return result;
    }
}

public static class SaveSystem
{
	[DllImport("__Internal")]
    private static extern void SaveGameExtern(string data, bool flush);

    [DllImport("__Internal")]
    private static extern void LoadGameExtern();
	
	private static readonly string _savesPath;
	private const string SAVE_NAME = "Save.sav";

	static SaveSystem()
	{
		if (Application.isMobilePlatform)
        {
            _savesPath = Application.persistentDataPath;
        }
        else
        {
            _savesPath = Application.platform switch
            {
                _ => Application.dataPath,
            };
        }

		_savesPath = Path.Combine(_savesPath, "Saves");
	}

	public static void Save(SaveData data, bool flush = false)
	{
		if (data == null) return;

        switch (Application.platform)
        {
            case RuntimePlatform.WebGLPlayer:
                {
                    string json = JsonUtility.ToJson(data, true);
                    SaveGameExtern(json, flush);
                }
                break;

            default:
                {
                    if (!Directory.Exists(_savesPath))
                        Directory.CreateDirectory(_savesPath);
                    string savePath = Path.Combine(_savesPath, SAVE_NAME);
                    if (File.Exists(savePath)) File.WriteAllText(savePath, string.Empty);

                    byte[] bytes = data.Serialize();
                    using FileStream stream = new(savePath, FileMode.OpenOrCreate);
                    try
                    {
                        using BinaryWriter writer = new(stream);
                        writer.Write(bytes);
                    }
                    catch
                    {
                        Debug.LogError("Cannot write save to file!");
                    }
                }
                break;
        }
	}

	public static SaveData LoadData()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WebGLPlayer:
                {
                    LoadGameExtern();
                    return default;
                }

            default:
                {
                    if (!Directory.Exists(_savesPath)) return default;
                    string savePath = Path.Combine(_savesPath, SAVE_NAME);
                    if (!File.Exists(savePath)) return default;

                    using FileStream stream = new(savePath, FileMode.Open);
                    using BinaryReader reader = new(stream);
                    byte[] bytes = reader.ReadAllBytes();
                    SaveData data = SaveData.Deserealize(bytes);
                    return data;
                }
        }
    }

	public static SaveData LoadData(string json)
	{
		if (json == null || json.Length < 2) return default;

		try
		{
			return JsonUtility.FromJson<SaveData>(json);
		}
		catch
		{
			return default;
		}
	}
}

public static class BinaryReaderExt
{
    public static byte[] ReadAllBytes(this BinaryReader reader)
    {
        const int bufferSize = 4096;
        using MemoryStream stream = new();
        byte[] buffer = new byte[bufferSize];
        int count;
        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            stream.Write(buffer, 0, count);
        return stream.ToArray();
    }
}