using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private Vector2 _buttonPlayAnimPos;
    private Vector2 _buttonPlayDefaultPos;
    [SerializeField] private RectTransform _rightButtonsPanel;
    [SerializeField] private Vector2 _rightButtonsPanelAnimPos;
    private Vector2 _rightButtonPanelDefaultPos;
    [SerializeField] private Button _buttonRateGame;
    [SerializeField] private Vector2 _buttonRateGameAnimPos;
    private Vector2 _buttonRateGameDefaultPos;

    [SerializeField] private RectTransform _authWindow;
    [SerializeField] private Vector2 _authWindowAnimPos;
    private Vector2 _authWindowDefaultPos;

    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;

    public void Initialize()
    {
        _buttonPlayDefaultPos = _buttonPlay.GetComponent<RectTransform>().anchoredPosition;
        _rightButtonPanelDefaultPos = _rightButtonsPanel.anchoredPosition;
        _buttonRateGameDefaultPos = _buttonRateGame.GetComponent<RectTransform>().anchoredPosition;
        _authWindowDefaultPos = _authWindow.anchoredPosition;

        WorldManager.Instance.OnReadyToSpawn += HandleReadyToSpawn;

        ToggleRateGameButton(false, true);
        ToggleAuthWindow(false, true);
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

    private void ToggleButtonInteractable(bool enable)
    {
        for (int i = _rightButtonsPanel.transform.childCount - 1; i >= 0; i--)
        {
            Button btn = _rightButtonsPanel.transform.GetChild(i).GetComponent<Button>();
            btn.interactable = enable;
        }
    }

    public void Hide()
    {
        LeanTween.cancel(_buttonPlay.gameObject);
        LeanTween.cancel(_rightButtonsPanel.gameObject);
        _buttonPlay.interactable = false;
        ToggleButtonInteractable(false);

        LeanTween.move(_buttonPlay.GetComponent<RectTransform>(), _buttonPlayAnimPos, _animSpeed + 0.12f).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _buttonPlay.gameObject.Toggle(false));

        LeanTween.move(_rightButtonsPanel.GetComponent<RectTransform>(), _rightButtonsPanelAnimPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => _rightButtonsPanel.gameObject.Toggle(false));

        ToggleRateGameButton(false);
    }

    public void Show()
    {
        LeanTween.cancel(_buttonPlay.gameObject);
        LeanTween.cancel(_rightButtonsPanel.gameObject);
        
        _buttonPlay.interactable = false;
        _buttonPlay.gameObject.Toggle(true);
        ToggleButtonInteractable(false);
        _rightButtonsPanel.gameObject.Toggle(true);

        LeanTween.move(_buttonPlay.GetComponent<RectTransform>(), _buttonPlayDefaultPos, _animSpeed + 0.12f).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() =>
        {
            if (WorldManager.Instance.IsReady)
                _buttonPlay.interactable = true;
        });
        LeanTween.move(_rightButtonsPanel.GetComponent<RectTransform>(), _rightButtonPanelDefaultPos, _animSpeed).setIgnoreTimeScale(true).
        setEase(_animType).setOnComplete(() => ToggleButtonInteractable(true));

        ToggleRateGameButton(true);
    }

    public void ToggleAuthWindow(bool enable, bool force = false)
    {
        LeanTween.cancel(_authWindow);

        if (enable)
        {
            _authWindow.gameObject.Toggle(true);
            LeanTween.move(_authWindow, _authWindowDefaultPos, _animSpeed).setIgnoreTimeScale(true).setEase(_animType);
        }
        else
        {
            if (force)
                _authWindow.gameObject.Toggle(false);

            LeanTween.move(_authWindow, _authWindowAnimPos, _animSpeed).setIgnoreTimeScale(true).setEase(_animType).
                setOnComplete(() => _authWindow.gameObject.Toggle(false));
        }
    }

    public void HideAuthWindow()
    {
        ToggleAuthWindow(false);
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
