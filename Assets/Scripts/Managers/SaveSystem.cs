using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

[Serializable]
public class SaveData
{
	public bool gameTutorial = true;
	public float masterVolume = 1f;
    public bool batterySaving;
    public bool gameRated;
    public bool showFps;
    public List<PlayerScore> scores = new();

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public static SaveData Deserealize(string data)
    {
        if (data.Length <= 0) return default;

        SaveData result = new();
        try
        {
            result = JsonUtility.FromJson<SaveData>(data);
        }
        catch { }

        return result;
    }
}

public static class SaveSystem
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SaveGameExtern(string data, bool flush);

    [DllImport("__Internal")]
    private static extern void LoadGameExtern();
#endif

    private static readonly string _savesPath;
	private const string SAVE_NAME = "Save.sav";
#if !UNITY_WEBGL || UNITY_EDITOR
    private readonly static byte[] _savedKey = { 0x01, 0x25, 0x05, 0x12, 0x13, 0x20, 0x02, 0x01, 0x25, 0x05, 0x12, 0x13, 0x20, 0x02, 0x15, 0x13 };
#endif

    public static bool SaveExists
    {
        get
        {
            if (!Directory.Exists(_savesPath)) return false;
            string savePath = Path.Combine(_savesPath, SAVE_NAME);
            if (!File.Exists(savePath)) return false;
            return true;
        }
    }

    static SaveSystem()
	{
        if ((Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer) && !Application.isEditor)
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
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(data, true);
        SaveGameExtern(json, flush);
#else
        if (!Directory.Exists(_savesPath))
            Directory.CreateDirectory(_savesPath);
        string savePath = Path.Combine(_savesPath, SAVE_NAME);
        if (File.Exists(savePath)) File.WriteAllText(savePath, string.Empty);

        Aes iAes = Aes.Create();

        using FileStream stream = new(savePath, FileMode.OpenOrCreate);
        byte[] inputIV = iAes.IV;
        stream.Write(inputIV, 0, inputIV.Length);
        CryptoStream iStream = new(
            stream,
            iAes.CreateEncryptor(_savedKey, iAes.IV),
            CryptoStreamMode.Write);

        using StreamWriter writer = new(iStream);
        writer.Write(data.Serialize());
#endif
    }

    public static SaveData LoadData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        LoadGameExtern();
        return default;
#else
        if (!Directory.Exists(_savesPath)) return default;
        string savePath = Path.Combine(_savesPath, SAVE_NAME);
        if (!File.Exists(savePath)) return default;

        Aes oAes = Aes.Create();
        byte[] outputIV = new byte[oAes.IV.Length];

        using FileStream stream = new(savePath, FileMode.Open);
        stream.Read(outputIV, 0, outputIV.Length);
        try
        {
            CryptoStream oStream = new(
                stream,
                oAes.CreateDecryptor(_savedKey, outputIV),
                CryptoStreamMode.Read);
            using StreamReader reader = new(oStream);
            string text = reader.ReadToEnd();
            SaveData data = SaveData.Deserealize(text);
            return data;
        }
        catch { }
        return default;
#endif
    }

    public static SaveData LoadData(string json)
	{
		if (json == null || json.Length <= 2) return default;

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