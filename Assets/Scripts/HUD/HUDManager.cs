using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Random = UnityEngine.Random;

public class AudioClipSetup
{
    public AudioClip clip;
    public float volume;
    public float pitch;
}

public class HUDManager : MonoBehaviour
{
    [SerializeField] private BackgroundUI _darkBackground;
    public static HUDManager Instance { get; private set; }
    public float HealthbarRenderDistance => GameManager.Instance.MainCamera.orthographicSize + 10f;
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect => _canvasRect;
    [SerializeField] private TextMeshProUGUI _versionText;

    [Header("Bars")]
    [SerializeField] private RectTransform _tankBarsTransform;
    [SerializeField] private RectTransform _destructibleBarsTransform;

    [Header("HUDs")]
    [SerializeField] private UpgradesUI _upgradesUI;
    public MobileControls MobileControls => _mobileControls;
    [SerializeField] private MobileControls _mobileControls;
    public MenuUI MenuUI =>_menuUI;
    [SerializeField] private MenuUI _menuUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private KeyUIManager _keyUIManager;
    [SerializeField] private GameTutorialUI _gameTutorialUI;
    [SerializeField] private ScoresUI _scoresUI;
    public ScoresUI ScoresUI => _scoresUI;
    [SerializeField] private RectTransform _gameOverText;
    [SerializeField] private LeanTweenType _gameOverAnimType;
    private Vector2 _defaultGameOverTextSize;
    public event Action OnGameOverScreenHidden;
    public FpsCounterUI FPSCounterUI => _fpsCounterUI;
    [SerializeField] private FpsCounterUI _fpsCounterUI;
    [SerializeField] private PlayerInfoUI _playerInfoUI;
#if UNITY_EDITOR
    [SerializeField] private PauseUI _pauseUI;
#endif

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSourceOneShot;
    [SerializeField] private AudioSource _audioSource;
    private bool _canPlaySound = true;
    [SerializeField] private AudioClip _buttonClick;

    public bool ShowFPS { get; private set; }

    public void Initialize()
    {
        Instance = this;

        _defaultGameOverTextSize = _gameOverText.sizeDelta;
        _gameOverText.sizeDelta = Vector2.zero;
        _gameOverText.gameObject.Toggle(false);

        _darkBackground.Initialize();
        _fpsCounterUI.Initialize();

        _menuUI.Initialize();
        _mobileControls.Initialize();
        _mobileControls.gameObject.Toggle(false);

        _upgradesUI.Initialize();
        _keyUIManager.Initialize();
        _gameTutorialUI.Initialize();
        _playerInfoUI.Initialize();
#if UNITY_EDITOR
        _pauseUI.Initialize();
#endif
        _scoresUI.Initialize();
        _settingsUI.Initialize();

        GameManager.Instance.OnPauseChanged += pause =>
        {
            if (pause)
            {
                _darkBackground.Show();
            }
            else
            {
                _darkBackground.Hide();
            }
        };

        WorldManager.Instance.OnGameEnded += HandleGameEnded;
        WorldManager.Instance.OnGameStarted += HandleGameStarted;

        _versionText.text = Application.version;
    }

    private void Start()
    {
        HandleGameEnded();
    }

    private void HandleGameStarted()
    {
#if UNITY_EDITOR
        _pauseUI.Show();
#endif
        if (IsTouchInput())
        {
            _mobileControls.gameObject.Toggle(true);
        }

        _versionText.gameObject.Toggle(false);
    }

    private void HandleGameEnded()
    {
        HideHuds();
        if (WorldManager.Instance.IsPlaying)
        {
            ShowGameOverScreen();
        }
        else
        {
            _versionText.gameObject.Toggle(true);
            _menuUI.Show();
        }
    }

    private void HideHuds()
    {
#if UNITY_EDITOR
        _pauseUI.Hide();
#endif
        _upgradesUI.ToggleMenu(false, true);

        if (IsTouchInput())
        {
            _mobileControls.gameObject.Toggle(false);
        }
    }

    public BarUI CreateHealthbar(bool smallBar = false)
    {
        BarUI bar = null;
        RectTransform parent = smallBar ? _destructibleBarsTransform : _tankBarsTransform;

        for (int i = parent.childCount - 1;  i >= 1; i--)
        {
            bar = parent.GetChild(i).GetComponent<BarUI>();
            if (bar.IsFree)
                break;
            else
                bar = null;
        }

        if (bar == null)
            bar = Instantiate(parent.GetChild(0), parent).GetComponent<BarUI>();

        return bar;
    }

    public static bool IsPointerOnUI(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new(EventSystem.current){ position = screenPosition };
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        return raycastResults.Count > 0;
    }

    public void PlaySoundOneShot(UISound sound)
    {
        AudioClipSetup clip = GetUISoundClip(sound);
        if (clip == null) return;

        _audioSourceOneShot.pitch = clip.pitch;
        _audioSourceOneShot.volume = clip.volume;
        _audioSourceOneShot.PlayOneShot(clip.clip);
    }

    public AudioClipSetup GetUISoundClip(UISound sound)
    {
        AudioClipSetup clip = new();

        switch (sound)
        {
            case UISound.Button:
                clip.pitch = Random.Range(0.9f, 1.1f);
                clip.volume = Random.Range(0.9f, 1f);
                clip.clip = _buttonClick;
                return clip;

            case UISound.Exit:
                break;
        }

        return default;
    }

    private IEnumerator PlaySoundDelayIE(UISound sound)
    {
        _canPlaySound = false;

        AudioClipSetup clip = GetUISoundClip(sound);
        _audioSource.pitch = clip.pitch;
        _audioSource.volume = clip.volume;
        _audioSource.clip = clip.clip;
        _audioSource.Play();

        yield return new WaitForSecondsRealtime(_audioSource.GetClipRemainingTime());
        _canPlaySound = true;
    }

    public void PlayButtonSound()
    {
        PlaySoundOneShot(UISound.Button);
    }

    public void PlayButtonSoundDelay()
    {
        if (!_canPlaySound) return;

        StartCoroutine(PlaySoundDelayIE(UISound.Button));
    }

    public bool IsTouchInput()
    {
        return Application.isMobilePlatform ||
              (Application.isEditor && TouchSimulation.instance != null && TouchSimulation.instance.enabled);
    }

    public Vector2 GetScreenPosition(Vector2 objectPosition, in Vector2 offset)
    {
        objectPosition = GameManager.Instance.MainCamera.WorldToViewportPoint(objectPosition + offset);
        return new(objectPosition.x * CanvasRect.sizeDelta.x, objectPosition.y * CanvasRect.sizeDelta.y);
    }

    public void ShowGameOverScreen()
    {
        LeanTween.cancel(_gameOverText);
        _gameOverText.gameObject.Toggle(true);
        LeanTween.size(_gameOverText, _defaultGameOverTextSize, 0.8f).setIgnoreTimeScale(true).setEase(_gameOverAnimType).setOnComplete(() =>
        {
            IEnumerator hide()
            {
                yield return new WaitForSeconds(2f);
                LeanTween.size(_gameOverText, Vector2.zero, 0.6f).setIgnoreTimeScale(true).setEase(_gameOverAnimType).setOnComplete(() =>
                {
                    _gameOverText.gameObject.Toggle(false);
                    AdsManager.ShowAds();
                    OnGameOverScreenHidden?.Invoke();
                    HandleGameEnded();
                });
            }

            StartCoroutine(hide());
        });
    }
}
