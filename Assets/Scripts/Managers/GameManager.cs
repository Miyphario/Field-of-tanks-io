using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static GameManager Instance { get; private set; }
    public InputControl CurrentInputControl { get; private set; } = InputControl.Keyboard;
    public event Action<InputControl> OnInputChanged;

    private Camera _camera;
    public Camera MainCamera => _camera;

    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            if (value)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if (_isPaused != value)
                OnPauseChanged?.Invoke(value);
            _isPaused = value;
        }
    }
    private bool _isPaused;

    public event Action<bool> OnPauseChanged;
    public event Action OnGameRestarting;
    public bool GameRestarting { get; private set; }
    public bool GameTutorial { get; set; } = true;
    public event Action<SaveData> OnSaveLoaded;

    public GameManager()
    {
        Instance = this;

        _camera = Camera.main;

        Bootstrap.Instance.StartCoroutine(CheckControlsTypeIE());

        if (Application.isMobilePlatform)
        {
            CurrentInputControl = InputControl.Touch;
            Application.targetFrameRate = 60;
        }
    }

    public void RestartScene(float time)
    {
        GameRestarting = true;
        OnGameRestarting?.Invoke();
        InputManager.Instance.Disable();
        Bootstrap.Instance.StartCoroutine(RestartSceneIE(time));
    }

    public void RestartScene()
    {
        RestartScene(0);
    }

    private IEnumerator RestartSceneIE(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator CheckControlsTypeIE()
    {
        if (Application.isMobilePlatform) yield break;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Gamepad.current == null)
            {
                if (CurrentInputControl != InputControl.Keyboard)
                {
                    CurrentInputControl = InputControl.Keyboard;
                    OnInputChanged?.Invoke(CurrentInputControl);
                }
                continue;
            }

            var gamepad = Gamepad.current.lastUpdateTime;
            var mouse = Mouse.current.lastUpdateTime;
            var keyboard = Keyboard.current.lastUpdateTime;
            if (gamepad > keyboard && gamepad > mouse)
            {
                if (CurrentInputControl != InputControl.Gamepad)
                {
                    CurrentInputControl = InputControl.Gamepad;
                    OnInputChanged?.Invoke(CurrentInputControl);
                }
            }
            else
            {
                if (CurrentInputControl != InputControl.Keyboard)
                {
                    CurrentInputControl = InputControl.Keyboard;
                    OnInputChanged?.Invoke(CurrentInputControl);
                }

            }
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    public void SetBatterySave(bool enable)
    {
        if (enable)
        {
            Application.targetFrameRate = 30;
        }
        else
        {
            if (Application.isMobilePlatform)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = 0;
        }
    }

    public bool GetBatterySave()
    {
        return Application.targetFrameRate == 30;
    }

    public void SaveGame()
    {
        SaveData data = new()
        {
            gameTutorial = GameTutorial,
            gameRated = Yandex.Instance.GameRated,
            batterySaving = GetBatterySave(),
            masterVolume = SoundManager.Instance.GetMasterVolume()
        };
        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadData();
        SetupGameSave(data);
    }

    public void LoadGame(string json)
    {
        SaveData data = SaveSystem.LoadData(json);
        SetupGameSave(data);
    }

    private void SetupGameSave(SaveData data)
    {
        if (data == null) return;

        GameTutorial = data.gameTutorial;
        SetBatterySave(data.batterySaving);
        OnSaveLoaded?.Invoke(data);
    }
}
