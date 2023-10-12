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
     [DllImport("__Internal")]
    private static extern void CanRateGameExtern();
#endif

    private bool _gameRated;
    public bool CannotRateGame { get; private set; }
    public bool ButtonRateGameEnabled { get; private set; }
    public bool GameRated => _gameRated;

    public void Initialize()
    {
        Instance = this;
#if UNITY_WEBGL && !UNITY_EDITOR
        CanRateGameExtern();
#else
        GameManager.Instance.OnSaveLoaded += data =>
        {
            _gameRated = data.gameRated;
        };
#endif
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

#if UNITY_WEBGL && !UNITY_EDITOR
        RateGameExtern();
#else
        Application.OpenURL("market://details?id=" + Application.identifier);
#endif
        
        ButtonRateGameEnabled = false;
        CannotRateGame = true;
        _gameRated = true;
        HUDManager.Instance.MenuUI.ToggleRateGameButton(false);
#if !UNITY_WEBGL || UNITY_EDITOR
        GameManager.Instance.SaveGame();
#endif
    }

    public void IsGameRated(int canRate)
    {
        // Can rate the game
        if (canRate == 0)
        {
            _gameRated = false;
        }
        // Can't rate the game
        else if (canRate == 1)
        {
            CannotRateGame = true;
            _gameRated = true;
            if (ButtonRateGameEnabled)
            {
                HUDManager.Instance.MenuUI.ToggleRateGameButton(false);
                ButtonRateGameEnabled = false;
            }
        }
    }
}
