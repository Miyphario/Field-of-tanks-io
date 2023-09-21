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
    [SerializeField] private Button _buttonRateGame;
    [SerializeField] private Vector2 _buttonRateGameAnimPos;
    private Vector2 _buttonRateGameDefaultPos;

    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;

    public void Initialize()
    {
        _buttonPlayDefaultPos = _buttonPlay.GetComponent<RectTransform>().anchoredPosition;
        _buttonSettingsDefaultPos = _buttonSettings.GetComponent<RectTransform>().anchoredPosition;
        _buttonRateGameDefaultPos = _buttonRateGame.GetComponent<RectTransform>().anchoredPosition;

        WorldManager.Instance.OnGameEnded += HandleGameEnded;
        WorldManager.Instance.OnReadyToSpawn += HandleReadyToSpawn;

        ToggleRateGameButton(false, true);
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
        });

        LeanTween.move(_buttonSettings.GetComponent<RectTransform>(), _buttonSettingsAnimPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => 
        {
            _buttonSettings.gameObject.Toggle(false);
        });

        ToggleRateGameButton(false);
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

        ToggleRateGameButton(true);
    }

    public void ToggleRateGameButton(bool enable, bool force = false)
    {
        bool canShow = Yandex.Instance != null && !Yandex.Instance.CannotRateGame && Yandex.Instance.ButtonRateGameEnabled;
        if (enable)
        {
            if (canShow)
            {
                LeanTween.cancel(_buttonRateGame.gameObject);
                _buttonRateGame.interactable = false;
                _buttonRateGame.gameObject.Toggle(true);
                LeanTween.move(_buttonRateGame.GetComponent<RectTransform>(), _buttonRateGameDefaultPos, _animSpeed).setIgnoreTimeScale(true)
                .setEase(_animType).setOnComplete(() => _buttonRateGame.interactable = true);
            }
        }
        else
        {
            LeanTween.cancel(_buttonRateGame.gameObject);
            _buttonRateGame.interactable = false;
            if (force)
                _buttonRateGame.gameObject.Toggle(false);
            LeanTween.move(_buttonRateGame.GetComponent<RectTransform>(), _buttonRateGameAnimPos, _animSpeed).setIgnoreTimeScale(true)
            .setEase(_animType).setOnComplete(() => _buttonRateGame.gameObject.Toggle(false));
        }
    }
}
