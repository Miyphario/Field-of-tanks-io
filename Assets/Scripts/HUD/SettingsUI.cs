using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Toggle _batterySaveToggle;

    [SerializeField] private RectTransform _settingsLayout;
    [SerializeField] private Vector2 _animSettingsLayoutPos;
    private Vector2 _defaultSettingsLayoutPos;
    [SerializeField] private Button _backButton;
    [SerializeField] private Vector2 _animBackButtonPos;
    private Vector2 _defaultBackButtonPos;
    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;

    private void Awake()
    {
        if (!Application.isMobilePlatform)
            _batterySaveToggle.transform.parent.gameObject.Toggle(false);
        
        Hide(true);
    }

    public void Initialize()
    {
        _defaultSettingsLayoutPos = _settingsLayout.anchoredPosition;
        _defaultBackButtonPos = _backButton.GetComponent<RectTransform>().anchoredPosition;
        SetSettings();
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
        setEase(_animType).setOnComplete(() => 
        {
            _settingsLayout.gameObject.Toggle(false);
        });

        LeanTween.move(_backButton.GetComponent<RectTransform>(), _animBackButtonPos, animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => 
        {
            _backButton.gameObject.Toggle(false);
            _backButton.interactable = true;
        });
    }

    public void SetBatterySave(bool enable) => GameManager.Instance.SetBatterySave(enable);

    private void SetSettings()
    {
        _masterVolumeSlider.value = SoundManager.Instance.GetMasterVolume();
        _batterySaveToggle.isOn = GameManager.Instance.GetBatterySave();
    }
}
