using System.Collections;
using UnityEngine;

public class GameTutorialUI : MonoBehaviour
{
    [Header("Touch")]
    [SerializeField] private RectTransform _touchControls;

    [Header("Keyboard")]
    [SerializeField] private RectTransform _keyboardControls;

    [Header("Gamepad")]
    [SerializeField] private RectTransform _gamepadControls;

    private Coroutine _hideRoutine;

    public void Initialize()
    {
        WorldManager.Instance.OnGameStarted += HandleGameStarted;
        GameManager.Instance.OnInputChanged += HandleInputChanged;
        WorldManager.Instance.OnGameEnded += HandleGameEnded;
        HandleInputChanged(GameManager.Instance.CurrentInputControl);
        HandleGameEnded();
    }

    private void HandleGameStarted()
    {
        if (!GameManager.Instance.GameTutorial) return;

        _keyboardControls.parent.gameObject.Toggle(true);
        HandleInputChanged(GameManager.Instance.CurrentInputControl);
        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);
        _hideRoutine = StartCoroutine(HideTutorialIE());
    }

    private void HandleGameEnded()
    {
        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);
        _keyboardControls.parent.gameObject.Toggle(false);
    }

    private void HandleInputChanged(InputControl control)
    {
        switch (control)
        {
            case InputControl.Keyboard:
                {
                    _touchControls.gameObject.Toggle(false);
                    _gamepadControls.gameObject.Toggle(false);
                    _keyboardControls.gameObject.Toggle(true);
                }
                break;

            case InputControl.Gamepad:
                {
                    _touchControls.gameObject.Toggle(false);
                    _gamepadControls.gameObject.Toggle(true);
                    _keyboardControls.gameObject.Toggle(false);
                }
                break;

            case InputControl.Touch:
                {
                    _touchControls.gameObject.Toggle(true);
                    _gamepadControls.gameObject.Toggle(false);
                    _keyboardControls.gameObject.Toggle(false);
                }
                break;
        }
    }

    private IEnumerator HideTutorialIE()
    {
        yield return new WaitForSeconds(6f);
        GameManager.Instance.GameTutorial = false;
        HandleGameEnded();
        GameManager.Instance.SaveGame();
    }
}
