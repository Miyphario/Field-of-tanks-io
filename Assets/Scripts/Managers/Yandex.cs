using UnityEngine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class Yandex : MonoBehaviour
{
    public static Yandex Instance { get; private set; }

#if UNITY_WEBGL
     [DllImport("__Internal")]
     private static extern void RateGameExtern();
     [DllImport("__Internal")]
     private static extern void CanRateGameExtern();
     [DllImport("__Internal")]
     private static extern void SetToLeaderboardExtern(int frags);
     [DllImport("__Internal")]
     private static extern void GetAuthExtern();
     [DllImport("__Internal")]
     private static extern void AuthExtern();
#endif

    private bool _gameRated;
    public bool CannotRateGame { get; private set; }
    public bool ButtonRateGameEnabled { get; private set; }
    public bool GameRated => _gameRated;
    public bool IsAuth { get; private set; }
    private bool _canAuth = true;

    public void Initialize()
    {
        Instance = this;
#if UNITY_WEBGL && !UNITY_EDITOR
        CanRateGameExtern();
        GetAuthExtern();
#elif UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        GameManager.Instance.OnSaveLoaded += data =>
        {
            _gameRated = data.gameRated;
            if (_gameRated) CannotRateGame = true;
        };
#endif
#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        GameManager.Instance.OnScoreAdded += (score, index) =>
        {
            SetToLeaderboard(score.frags);
        };

        HUDManager.Instance.OnGameOverScreenHidden += HandleOnGameEnded;
#else
        _gameRated = true;
        CannotRateGame = true;
#endif
    }

    private void HandleOnGameEnded()
    {
#if UNITY_WEBGL
        if (_canAuth && !IsAuth)
        {
            _canAuth = false;
            HUDManager.Instance.MenuUI.ToggleAuthWindow(true);
        }
#endif

        if (CannotRateGame) return;

        ButtonRateGameEnabled = true;
    }

    public void RateGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RateGameExtern();
#elif UNITY_ANDROID || UNITY_IOS
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

    public void SetToLeaderboard(int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetToLeaderboardExtern(score);
#endif
    }

    public void SetAuth(int auth)
    {
        IsAuth = auth == 1;
        Debug.Log("Auth: " + IsAuth);
    }

    public void Auth()
    {
#if UNITY_WEBGL
        AuthExtern();
#endif
    }
}
