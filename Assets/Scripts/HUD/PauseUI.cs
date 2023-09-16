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
        if (_unpauseButton.gameObject.activeSelf)
            _unpauseButton.gameObject.SetActive(false);
        
        if (!_pauseButton.gameObject.activeSelf)
            _pauseButton.gameObject.SetActive(true);

        GameManager.Instance.OnPauseChanged += pause =>
        {
            if (pause)
            {
                if (!_unpauseButton.gameObject.activeSelf)
                    _unpauseButton.gameObject.SetActive(true);
                
                if (_pauseButton.gameObject.activeSelf)
                    _pauseButton.gameObject.SetActive(false);
            }
            else
            {
                if (_unpauseButton.gameObject.activeSelf)
                    _unpauseButton.gameObject.SetActive(false);
                
                if (!_pauseButton.gameObject.activeSelf)
                    _pauseButton.gameObject.SetActive(true);
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
