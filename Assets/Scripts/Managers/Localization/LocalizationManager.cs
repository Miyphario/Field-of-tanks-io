using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

static class LocalizationManager
{
    [DllImport("__Internal")]
    private static extern string GetLang();

    private static string _currentLanguage;
    public static string CurrentLanguage => _currentLanguage;
    public static string LanguageName { get; private set; }

    private static List<LocalizationLanguage> _installedLanguages = new();
    public static IReadOnlyList<LocalizationLanguage> InstalledLanguages => _installedLanguages;

    private static readonly Dictionary<string, string> _localizedText = new();

    private static bool _isReady;
    private static bool _isLoading;
    public static event Action OnLanguageChanged;

    public static void Initialize()
    {
        if (!Application.isPlaying || Bootstrap.Instance == null) return;

        Debug.Log("Loading languages...");
        Bootstrap.Instance.StartCoroutine(GetInstalledLanguagesIE());
    }

    private static IEnumerator GetInstalledLanguagesIE()
    {
        if (_isLoading) yield break;

        _isLoading = true;
        string path = Path.Combine(Application.streamingAssetsPath, "Languages");

        if (Directory.Exists(path) || Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android)
        {
            IEnumerator addLanguages(IEnumerable<string> files)
            {
                string data;
                foreach (var file in files)
                {
                    if (!file.EndsWith(".json")) continue;

                    if ((Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android) && !Application.isEditor)
                    {
                        using UnityWebRequest unityWebRequest = UnityWebRequest.Get(file);
                        yield return unityWebRequest.SendWebRequest();
                        data = System.Text.Encoding.UTF8.GetString(unityWebRequest.downloadHandler.data, 3, unityWebRequest.downloadHandler.data.Length - 3);
                    }
                    else
                    {
                        data = File.ReadAllText(file);
                    }

                    if (data == null || data == "")
                    {
                        Debug.LogError($"{path} data is empty!");
                        continue;
                    }

                    LocalizationData loadedData;

                    try
                    {
                        loadedData = JsonUtility.FromJson<LocalizationData>(data);
                    }
                    catch
                    {
                        Debug.LogError($"{path} cannot be loaded!");
                        continue;
                    }

                    string langCode = Path.GetFileNameWithoutExtension(file);

                    LocalizationLanguage lang = new() { name = loadedData.languageName, code = langCode };
                    _installedLanguages.Add(lang);
                    Debug.Log($"Language {lang.name} is installed!");
                }
            }

            List<string> files = new();

            if ((Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android) && !Application.isEditor)
            {
                string pathToList = Path.Combine(Application.streamingAssetsPath, "list.txt");

                using UnityWebRequest unityWebRequest = UnityWebRequest.Get(pathToList);
                yield return unityWebRequest.SendWebRequest();
                string data = System.Text.Encoding.UTF8.GetString(unityWebRequest.downloadHandler.data, 3, unityWebRequest.downloadHandler.data.Length - 3);

                string[] split = data.Split('\n');
                foreach (var pt in split)
                {
                    if (pt.StartsWith("Languages"))
                    {
                        files.Add(Path.Combine(Application.streamingAssetsPath, pt));
                    }
                }
            }
            else
            {
                files = Directory.GetFiles(path).ToList();
            }

            yield return addLanguages(files);

            string lang;

            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
                lang = GetLang();
            else
            {
                lang = Application.systemLanguage switch
                {
                    SystemLanguage.Russian => "ru_RU",
                    SystemLanguage.Turkish => "tr_TR",
                    _ => "en_US",
                };
            }

            ChangeLanguage(lang);
        }
        else
        {
            Debug.LogError($"There are not files in current path: {path}");
            _isLoading = false;
            yield break;
        }

        _isLoading = false;
    }

    public static int GetLanguageIndex(string langName)
    {
        return _installedLanguages.FindIndex(x => x.name == langName);
    }

    private static string GetLangFromText(string text)
    {
        var split = text.Split('_');
        if (split.Length == 2)
        {
            return split[0].ToLower() + "_" + split[1].ToUpper();
        }
        else
        {
            // For browser games and GetLang method when return only 2 first symbols
            foreach (var installLang in _installedLanguages)
            {
                if (text.StartsWith(installLang.code[..2]))
                    return installLang.code;
            }

            return "en_US";
        }
    }

    public static void ChangeLanguage(string lang)
    {
        Bootstrap.Instance.StartCoroutine(LoadLocalizedTextIE(GetLangFromText(lang)));
    }

    public static void ChangeLanguageByName(string langName)
    {
        foreach (var lang in _installedLanguages)
        {
            if (lang.name == langName)
            {
                ChangeLanguage(lang.code);
                return;
            }
        }
    }

    public static IEnumerator LoadLocalizedTextIE(string langName)
    {
        if (_currentLanguage == langName) yield break;

        _isReady = false;
        string path = Path.Combine(Application.streamingAssetsPath, "Languages");
        path = Path.Combine(path, $"{langName}.json");

        if (File.Exists(path) || Application.platform == RuntimePlatform.WebGLPlayer || Application.platform == RuntimePlatform.Android)
        {
            string data;

            if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer) && !Application.isEditor)
            {
                using UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);
                yield return unityWebRequest.SendWebRequest();
                data = System.Text.Encoding.UTF8.GetString(unityWebRequest.downloadHandler.data, 3, unityWebRequest.downloadHandler.data.Length - 3);
            }
            else
            {
                data = File.ReadAllText(path);
            }

            if (data == null || data == "")
            {
                Debug.LogError($"{langName}.json is empty");
                yield break;
            }

            LocalizationData loadedData;

            try
            {
                loadedData = JsonUtility.FromJson<LocalizationData>(data);
            }
            catch
            {
                Debug.LogError($"{langName}.json cannot be loaded");
                yield break;
            }

            LanguageName = loadedData.languageName;

            _localizedText?.Clear();
            for (int i = 0; i < loadedData.items.Length; i++)
            {
                _localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            _currentLanguage = langName;
            _isReady = true;

            OnLanguageChanged?.Invoke();
        }
        else
        {
            throw new Exception($"{langName}.json not found!");
        }
    }

    public static string GetLocalizedText(string key)
    {
        if (_isReady)
        {
            if (_localizedText.ContainsKey(key))
            {
                if (_localizedText[key] == null || _localizedText[key] == "")
                    return key;

                return _localizedText[key];
            }
            else
            {
                Debug.LogError($"Key \"{key}\" is not found in {_currentLanguage}.json file!");
                return key;
            }
        }
        else
        {
            if (!_isLoading)
                Initialize();
            return key;
        }
    }
}
