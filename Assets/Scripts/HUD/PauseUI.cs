using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _unpauseButton;

    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

    public void Initialize()
    {
        _unpauseButton.gameObject.Toggle(false);
        _pauseButton.gameObject.Toggle(true);

        GameManager.Instance.OnPauseChanged += pause =>
        {
            if (pause)
            {
                _unpauseButton.gameObject.Toggle(true);
                _pauseButton.gameObject.Toggle(false);
            }
            else
            {
                _unpauseButton.gameObject.Toggle(false);
                _pauseButton.gameObject.Toggle(true);
            }
        };
    }

    public void Pause()
    {
        GameManager.Instance.IsPaused = true;
    }

    public void Unpause()
    {
        GameManager.Instance.IsPaused = false;
    }

    public void Show()
    {
        GameObject exceptObj = GameManager.Instance.IsPaused ? _pauseButton.transform.parent.gameObject :
                                                                   _unpauseButton.transform.gameObject;
        Helper.EnableAllExcept(transform, exceptObj);
    }

    public void Hide() => Helper.DisableAll(gameObject);
}
