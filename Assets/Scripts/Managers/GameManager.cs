using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager
{
    public static GameManager Instance { get; private set; }
    public bool IsKeyboardControls { get; private set; } = true;

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

    public GameManager()
    {
        Instance = this;

        _camera = Camera.main;

        if (!Application.isMobilePlatform)
        {
            Bootstrap.Instance.StartCoroutine(CheckControlsTypeIE());
        }
        else
        {
            IsKeyboardControls = false;
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
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                yield break;
            }

            if (Gamepad.current == null)
            {
                IsKeyboardControls = true;
                continue;
            }

            var gamepad = Gamepad.current.lastUpdateTime;
            var mouse = Mouse.current.lastUpdateTime;
            var keyboard = Keyboard.current.lastUpdateTime;
            if (gamepad > keyboard && gamepad > mouse)
                IsKeyboardControls = false;
            else
                IsKeyboardControls = true;
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
}
