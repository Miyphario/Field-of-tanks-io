using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private BackgroundUI _darkBackground;
    public static HUDManager Instance { get; private set; }
    public RectTransform CanvasRect => _canvasRect;
    public float HealthbarRenderDistance => GameManager.Instance.MainCamera.orthographicSize + 10f;
    [SerializeField] private RectTransform _canvasRect;

    [Header("Bars")]
    [SerializeField] private RectTransform _tankBarsTransform;
    [SerializeField] private RectTransform _destructibleBarsTransform;

    [Header("HUDs")]
    [SerializeField] private UpgradesUI _upgradesUI;
    [SerializeField] private MobileControls _mobileControls;
    public MobileControls MobileControls => _mobileControls;
#if UNITY_EDITOR
    [SerializeField] private PlayerInfoUI _playerInfoUI;
    [SerializeField] private PauseUI _pauseUI;
#endif
    [SerializeField] private MenuUI _menuUI;
    public MenuUI MenuUI =>_menuUI;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonClick;

    public void Initialize()
    {
        Instance = this;

        _menuUI.Initialize();
        _mobileControls.Initialize();
        if (_mobileControls.gameObject.activeSelf)
            _mobileControls.gameObject.SetActive(false);

        _darkBackground.Initialize();
        _upgradesUI.Initialize();
#if UNITY_EDITOR
        _pauseUI.Initialize();
        _playerInfoUI.Initialize();
#endif

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
        HandleGameEnded();
    }

    private void HandleGameStarted()
    {
#if UNITY_EDITOR
        _pauseUI.Show();
#endif
        if (IsTouchInput())
        {
            if (!_mobileControls.gameObject.activeSelf)
                _mobileControls.gameObject.SetActive(true);
        }
    }

    private void HandleGameEnded()
    {
#if UNITY_EDITOR
        _pauseUI.Hide();
#endif
        _upgradesUI.Hide();

        if (IsTouchInput())
        {
            if (_mobileControls.gameObject.activeSelf)
                _mobileControls.gameObject.SetActive(false);
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

    public void PlaySound(UISound sound)
    {
        switch (sound)
        {
            case UISound.Button:
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.volume = Random.Range(0.9f, 1f);
                _audioSource.clip = _buttonClick;
                break;

            case UISound.Exit:
                break;
        }

        if (_audioSource.clip == null) return;

        _audioSource.PlayOneShot(_audioSource.clip);
        _audioSource.clip = null;
    }

    public void PlayButtonSound()
    {
        PlaySound(UISound.Button);
    }

    public bool IsTouchInput()
    {
        return Application.isMobilePlatform ||
              (Application.isEditor && TouchSimulation.instance != null && TouchSimulation.instance.enabled);
    }
}
