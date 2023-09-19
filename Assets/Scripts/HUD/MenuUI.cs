using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Vector2 _buttonPlayAnimPos;
    private Vector2 _buttonPlayDefaultPos;
    [SerializeField] private Button _buttonSettings;
    [SerializeField] private Vector2 _buttonSettingsAnimPos;
    private Vector2 _buttonSettingsDefaultPos;

    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;

    public void Initialize()
    {
        _buttonPlayDefaultPos = _buttonPlay.GetComponent<RectTransform>().anchoredPosition;
        _buttonSettingsDefaultPos = _buttonSettings.GetComponent<RectTransform>().anchoredPosition;

        WorldManager.Instance.OnGameEnded += HandleGameEnded;
        WorldManager.Instance.OnReadyToSpawn += HandleReadyToSpawn;
    }

    private void HandleGameEnded()
    {
        Show();
    }

    private void HandleReadyToSpawn(bool notReady)
    {
        _buttonPlay.interactable = !notReady;
    }

    public void StartGame()
    {
        if (!WorldManager.Instance.StartGame()) return;
        Hide();
    }

    public void Hide()
    {
        LeanTween.cancel(_buttonPlay.gameObject);
        LeanTween.cancel(_buttonSettings.gameObject);
        _buttonPlay.interactable = false;
        _buttonSettings.interactable = false;

        LeanTween.move(_buttonPlay.GetComponent<RectTransform>(), _buttonPlayAnimPos, _animSpeed + 0.12f).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => 
        {
            _buttonPlay.gameObject.Toggle(false);
            _buttonPlay.interactable = true;
        });

        LeanTween.move(_buttonSettings.GetComponent<RectTransform>(), _buttonSettingsAnimPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => 
        {
             _buttonSettings.gameObject.Toggle(false);
            _buttonSettings.interactable = true;
        });
    }

    public void Show()
    {
        LeanTween.cancel(_buttonPlay.gameObject);
        LeanTween.cancel(_buttonSettings.gameObject);
        
        _buttonPlay.interactable = false;
        _buttonSettings.interactable = false;
        _buttonPlay.gameObject.Toggle(true);
        _buttonSettings.gameObject.Toggle(true);

        LeanTween.move(_buttonPlay.GetComponent<RectTransform>(), _buttonPlayDefaultPos, _animSpeed + 0.12f).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() =>
        {
            if (WorldManager.Instance.IsReady)
                _buttonPlay.interactable = true;
        });
        LeanTween.move(_buttonSettings.GetComponent<RectTransform>(), _buttonSettingsDefaultPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _buttonSettings.interactable = true);
    }
}
