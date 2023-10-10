using UnityEngine;
#if (UNITY_WEBGL && !UNITY_EDITOR)
using System.Runtime.InteropServices;
#endif

public class Yandex : MonoBehaviour
{
    public static Yandex Instance { get; private set; }

#if (UNITY_WEBGL && !UNITY_EDITOR)
     [DllImport("__Internal")]
     private static extern void RateGameExtern();
#endif

    private bool _gameRated;
    public bool CannotRateGame { get; private set; }
    public bool ButtonRateGameEnabled { get; private set; }
    public bool GameRated => _gameRated;

    public void Initialize()
    {
        Instance = this;

        GameManager.Instance.OnSaveLoaded += data =>
        {
            _gameRated = data.gameRated;
        };

        WorldManager.Instance.OnGameEnded += HandleOnGameEnded;
    }

    private void HandleOnGameEnded()
    {
        if (CannotRateGame) return;

        ButtonRateGameEnabled = true;
    }

    public void RateGame()
    {
        if (CannotRateGame || _gameRated) return;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
#if (UNITY_WEBGL && !UNITY_EDITOR)
            RateGameExtern();
#endif
        }
        else
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
        }
        
        CannotRateGame = true;
        _gameRated = true;
        HUDManager.Instance.MenuUI.ToggleRateGameButton(false);
        GameManager.Instance.SaveGame();
    }
}
