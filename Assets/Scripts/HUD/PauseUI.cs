using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _unpauseButton;

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
}
