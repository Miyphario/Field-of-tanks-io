using System;
using System.IO;
using UnityEngine;
//#if (UNITY_WEBGL && !UNITY_EDITOR)
//using System.Runtime.InteropServices;
//#endif

[Serializable]
public class SaveData
{
	public bool gameTutorial = true;
	public float masterVolume = 1f;
	public bool batterySaving = false;
    public bool gameRated;

    public byte[] Serialize()
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        writer.Write(gameTutorial);
        writer.Write(masterVolume);
        writer.Write(batterySaving);
        writer.Write(gameRated);
        return stream.ToArray();
    }

    public static SaveData Deserealize(byte[] data)
    {
        if (data.Length <= 0) return default;

        SaveData result = new();
        try
        {
            using BinaryReader reader = new(new MemoryStream(data));
            result.gameTutorial = reader.ReadBoolean();
            result.masterVolume = reader.ReadSingle();
            result.batterySaving = reader.ReadBoolean();
            result.gameRated = reader.ReadBoolean();
        }
        catch {}
        
        return result;
    }
}

public static class SaveSystem
{
//#if (UNITY_WEBGL && !UNITY_EDITOR)
//    [DllImport("__Internal")]
//    private static extern void SaveGameExtern(string data, bool flush);

//    [DllImport("__Internal")]
//    private static extern void LoadGameExtern();
//#endif

    private static readonly string _savesPath;
	private const string SAVE_NAME = "Save.sav";

	static SaveSystem()
	{
		if (Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer)
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
//#if (UNITY_WEBGL && !UNITY_EDITOR)
//        string json = JsonUtility.ToJson(data, true);
//        SaveGameExtern(json, flush);
//#else
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
//#endif

	}

	public static SaveData LoadData()
    {
//#if (UNITY_WEBGL && !UNITY_EDITOR)
//        LoadGameExtern();
//        return default;
//#else
        if (!Directory.Exists(_savesPath)) return default;
        string savePath = Path.Combine(_savesPath, SAVE_NAME);
        if (!File.Exists(savePath)) return default;

        using FileStream stream = new(savePath, FileMode.Open);
        using BinaryReader reader = new(stream);
        byte[] bytes = reader.ReadAllBytes();
        SaveData data = SaveData.Deserealize(bytes);
        return data;
//#endif
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