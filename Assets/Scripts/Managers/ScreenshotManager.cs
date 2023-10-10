using System;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private Camera _camera;

    public void Initialize()
    {
        _camera = GetComponent<Camera>();
        InputManager.Instance.OnScreenshot += TakeScreenshot;
    }

    public void TakeScreenshot()
    {
        RenderTexture screenTexture = new(Screen.width, Screen.height, 16);
        _camera.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        _camera.Render();
        Texture2D renderedTexture = new(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        byte[] byteArray = renderedTexture.EncodeToPNG();

        string path;
        if (Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            path = Application.persistentDataPath;
        }
        else
        {
            path = Application.dataPath;
        }

        path = Path.Combine(path, "Screenshots");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        int fileCount = Directory.GetFiles(path).Length;
        path = Path.Combine(path, DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + $"_{fileCount}.png");
        File.WriteAllBytes(path, byteArray);
    }
}
