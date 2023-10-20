using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Toggle _batterySaveToggle;
    [SerializeField] private Toggle _showFpsToggle;

    [Header("Animations")]
    [SerializeField] private RectTransform _settingsLayout;
    [SerializeField] private Vector2 _animSettingsLayoutPos;
    private Vector2 _defaultSettingsLayoutPos;
    [SerializeField] private Button _backButton;
    [SerializeField] private Vector2 _animBackButtonPos;
    private Vector2 _defaultBackButtonPos;
    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;
    private bool _saveLoaded;

    private void Awake()
    {
#if !UNITY_EDITOR
        if (!Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer)
            _batterySaveToggle.transform.parent.gameObject.Toggle(false);
#endif
        
        Hide(true);
    }

    public void Initialize()
    {
        _defaultSettingsLayoutPos = _settingsLayout.anchoredPosition;
        _defaultBackButtonPos = _backButton.GetComponent<RectTransform>().anchoredPosition;
        SetSettings();
        GameManager.Instance.OnSaveLoaded += HandleSaveLoaded;
        if (!SaveSystem.SaveExists)
        {
            HandleSaveLoaded(null);
        }
    }

    private void HandleSaveLoaded(SaveData data)
    {
        SetSettings();
        _saveLoaded = true;
    }

    public void Show()
    {
        LeanTween.cancel(_settingsLayout);
        LeanTween.cancel(_backButton.gameObject);
        _backButton.interactable = false;
        
        _settingsLayout.gameObject.Toggle(true);
        _backButton.gameObject.Toggle(true);

        LeanTween.move(_settingsLayout, _defaultSettingsLayoutPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType);
        LeanTween.move(_backButton.GetComponent<RectTransform>(), _defaultBackButtonPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _backButton.interactable = true);
    }

    public void Hide(bool force = false)
    {
        LeanTween.cancel(_settingsLayout);
        LeanTween.cancel(_backButton.gameObject);
        _backButton.interactable = false;
        float animSpeed = _animSpeed;

        if (force)
        {
            _settingsLayout.gameObject.Toggle(false);
            _backButton.gameObject.Toggle(false);
            
            animSpeed = 0f;
        }

        LeanTween.move(_settingsLayout, _animSettingsLayoutPos, animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _settingsLayout.gameObject.Toggle(false));

        LeanTween.move(_backButton.GetComponent<RectTransform>(), _animBackButtonPos, animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _backButton.gameObject.Toggle(false));
    }

    public void SetBatterySave(bool enable)
    {
        GameManager.Instance.SetBatterySave(enable);

        if (_saveLoaded)
        {
            DelayedSaveGame();
            HUDManager.Instance.PlayButtonSound();
        }
    }

    public void SetMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);

        if (_saveLoaded)
        {
            DelayedSaveGame();
            HUDManager.Instance.PlayButtonSoundDelay();
        }
    }

    public void ResetTutorial()
    {
        if (GameManager.Instance.GameTutorial) return;

        GameManager.Instance.GameTutorial = true;

        if (_saveLoaded)
            DelayedSaveGame();
    }

    public void ToggleFpsCounter(bool enable)
    {
        HUDManager.Instance.FPSCounterUI.Toggle(enable);

        if (_saveLoaded)
        {
            DelayedSaveGame();
            HUDManager.Instance.PlayButtonSound();
        }
    }

    public void DelayedSaveGame() => WorldManager.Instance.SaveGame(1f);

    private void SetSettings()
    {
        _masterVolumeSlider.value = SoundManager.Instance.GetMasterVolume();
        _batterySaveToggle.isOn = GameManager.Instance.GetBatterySave();
        _showFpsToggle.isOn = HUDManager.Instance.FPSCounterUI.ShowFPS;
    }
}
