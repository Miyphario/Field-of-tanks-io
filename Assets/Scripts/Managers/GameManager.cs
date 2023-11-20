using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool SaveLoaded { get; private set; }

    private List<PlayerScore> _scores = new();
    public IReadOnlyList<PlayerScore> Scores => _scores;
    public event Action<PlayerScore, int> OnScoreAdded;
    public event Action<PlayerScore> OnScoreRemoved;

    public GameManager()
    {
        Instance = this;

        _camera = Camera.main;

        Bootstrap.Instance.StartCoroutine(CheckControlsTypeIE());

        if (Application.isMobilePlatform)
        {
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
        if (Application.isMobilePlatform)
        {
            CurrentInputControl = InputControl.Touch;
            OnInputChanged?.Invoke(InputControl.Touch);
        }

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Gamepad.current == null)
            {
                if (Application.isMobilePlatform)
                {
                    if (CurrentInputControl != InputControl.Touch)
                    {
                        CurrentInputControl = InputControl.Touch;
                        OnInputChanged?.Invoke(CurrentInputControl);
                    }
                }
                else if (CurrentInputControl != InputControl.Keyboard)
                {
                    CurrentInputControl = InputControl.Keyboard;
                    OnInputChanged?.Invoke(CurrentInputControl);
                }

                continue;
            }

            var gamepad = Gamepad.current.lastUpdateTime;

            if (Application.isMobilePlatform)
            {
                var touch = Touchscreen.current.lastUpdateTime;
                if (gamepad > touch)
                {
                    if (CurrentInputControl != InputControl.Gamepad)
                    {
                        CurrentInputControl = InputControl.Gamepad;
                        OnInputChanged?.Invoke(CurrentInputControl);
                    }
                }
                else
                {
                    if (CurrentInputControl != InputControl.Touch)
                    {
                        CurrentInputControl = InputControl.Touch;
                        OnInputChanged?.Invoke(CurrentInputControl);
                    }
                }

                continue;
            }

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

    public bool GetBatterySave() => Application.targetFrameRate == 30;

    public void SaveGame()
    {
        SaveData data = new()
        {
            gameTutorial = GameTutorial,
            gameRated = Yandex.Instance.GameRated,
            batterySaving = GetBatterySave(),
            masterVolume = SoundManager.Instance.GetMasterVolume(),
            showFps = HUDManager.Instance.FPSCounterUI.ShowFPS,
            scores = _scores
        };
        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
#if !UNITY_WEBGL
        if (SaveLoaded) return;
#endif
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
        data ??= new();

        GameTutorial = data.gameTutorial;
        if (data.scores != null)
        {
            for (int i = data.scores.Count - 1; i >= 0; i--)
            {
                if (data.scores[i] == null) data.scores.RemoveAt(i);
            }
        }
        _scores = data.scores;
        SetBatterySave(data.batterySaving);
        Debug.LogWarning("Save data is loaded!");
        SaveLoaded = true;
        OnSaveLoaded?.Invoke(data);
    }

    public void SetNewScore(PlayerScore score)
    {
        if (_scores.Count >= 50)
        {
            int deleteIndex = -1;
            for (int i = _scores.Count - 1; i >= 0; i--)
            {
                if (_scores[i].frags < score.frags)
                {
                    deleteIndex = i;
                    break;
                }
                else if (_scores[i].frags == score.frags)
                {
                    if (_scores[i].time < score.time)
                    {
                        deleteIndex = i;
                        break;
                    }
                    else if (_scores[i].time == score.time)
                    {
                        if (_scores[i].level < score.level)
                        {
                            deleteIndex = i;
                            break;
                        }
                    }
                }
            }

            if (deleteIndex > -1)
            {
                OnScoreRemoved?.Invoke(_scores[deleteIndex]);
                _scores.RemoveAt(deleteIndex);
            }
            else return;
        }

        _scores.Add(score);
        _scores.Sort();
        OnScoreAdded?.Invoke(score, _scores.IndexOf(score));
        SaveGame();
    }
}
