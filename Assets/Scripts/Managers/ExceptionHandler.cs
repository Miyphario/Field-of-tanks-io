using System;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class ExceptionHandler : MonoBehaviour
{
    private static ExceptionHandler _instance;

    private void Awake()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        Destroy(gameObject);
#else
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.logMessageReceived += HandleException;
#endif
    }

    private void HandleException(string logString, string stackTrace, LogType type)
    {
        string path = null;
        string appPath;
        if (Application.isMobilePlatform)
        {
            appPath = Application.persistentDataPath;
        }
        else
        {
            appPath = Application.dataPath;
        }

        switch (type)
        {
            case LogType.Exception:
                path = Path.Combine(appPath, "Logs", "Exceptions.log");
                break;

            case LogType.Error:
                path = Path.Combine(appPath, "Logs", "Errors.log");
                break;

            case LogType.Warning:
                path = Path.Combine(appPath, "Logs", "Warnings.log");
                break;

            //case LogType.Log:
            //    path = Path.Combine(appPath, "Logs", "Logs.log");
            //    break;

            case LogType.Assert:
                path = Path.Combine(appPath, "Logs", "Asserts.log");
                break;
        }

        if (path != null)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using StreamWriter writer = new(path, true);
            writer.WriteLine(DateTime.Now.ToString("g") + " | Version: " + Application.version);
            writer.WriteLine("-----------------------------------------");
            writer.WriteLine(logString);
            writer.WriteLine(stackTrace);
            writer.WriteLine();
        }
    }
}
