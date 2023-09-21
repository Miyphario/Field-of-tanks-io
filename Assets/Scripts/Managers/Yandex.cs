using System.Runtime.InteropServices;
using UnityEngine;

public class Yandex : MonoBehaviour
{
    public static Yandex Instance { get; private set; }
    
    [DllImport("__Internal")]
    private static extern void RateGameExtern();
    [DllImport("__Internal")]
    private static extern void CanRateGameExtern();

    private bool _gameRated;
    public bool CannotRateGame { get; private set; }
    public bool ButtonRateGameEnabled { get; private set; }
    private string _playerName;

    public void Initialize()
    {
        /* if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            DisableRateButton();
            Destroy(gameObject);
            return;
        } */

        Instance = this;

        //CanRateGameExtern();
        WorldManager.Instance.OnGameEnded += HandleOnGameEnded;
    }

    private void HandleOnGameEnded()
    {
        if (CannotRateGame) return;

        ButtonRateGameEnabled = true;
        HUDManager.Instance.MenuUI.ToggleRateGameButton(true);
    }

    public void SetPlayerName(string name)
    {
        _playerName = name;
    }

    public void RateGame()
    {
        if (CannotRateGame) return;

        if (!_gameRated)
            RateGameExtern();
        
        CannotRateGame = true;
        
    }

    public void CanRateGame(bool canRate)
    {
        _gameRated = !canRate;
        if (_gameRated) CannotRateGame = true;
    }
}
