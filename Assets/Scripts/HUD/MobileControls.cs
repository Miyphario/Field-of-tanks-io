using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class MobileControls : MonoBehaviour
{
    [SerializeField] private FloatingStick _movementStick;
    private Finger _movementFinger;
    private Vector2 _movementAmount;

    [SerializeField] private FloatingStick _lookStick;
    private Finger _lookFinger;
    private Vector2 _lookAmount;

    [SerializeField] private Vector2 _stickSize;

    public event Action OnMoveStarted;
    public event Action<Vector2> OnMove;
    public event Action OnMoveEnded;

    public event Action OnLookStarted;
    public event Action<Vector2> OnLook;
    public event Action OnLookEnded;

    public void Initialize()
    {
        _movementStick.Initialize();
        _lookStick.Initialize();

        _movementStick.RectTransform.anchoredPosition = _stickSize;
        _lookStick.RectTransform.anchoredPosition = new Vector2(Screen.width - _stickSize.x, _stickSize.y);
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerUp(Finger finger)
    {
        if (_movementFinger == finger)
        {
            _movementFinger = null;
            _movementStick.Knob.anchoredPosition = Vector2.zero;
            if (_movementStick.gameObject.activeSelf)
                _movementStick.gameObject.SetActive(false);
            _movementAmount = Vector2.zero;
            OnMoveEnded?.Invoke();
        }
        else if (_lookFinger == finger)
        {
            _lookFinger = null;
            _lookStick.Knob.anchoredPosition = Vector2.zero;
            if (_lookStick.gameObject.activeSelf)
                _lookStick.gameObject.SetActive(false);
            _lookAmount = Vector2.zero;
            OnLookEnded?.Invoke();
        }
    }

    private void HandleFingerMove(Finger finger)
    {
        if (_movementFinger != finger && _lookFinger != finger) return;

        Vector2 knobPosition;
        float maxDistance = _stickSize.x / 2f;
        ETouch.Touch currentTouch = finger.currentTouch;
        FloatingStick currentStick = _movementFinger == finger ? _movementStick : _lookStick;

        if (Vector2.Distance(currentTouch.screenPosition, currentStick.RectTransform.anchoredPosition) > maxDistance)
        {
            knobPosition = (currentTouch.screenPosition - currentStick.RectTransform.anchoredPosition).normalized * maxDistance;
        }
        else
        {
            knobPosition = currentTouch.screenPosition - currentStick.RectTransform.anchoredPosition;
        }

        currentStick.Knob.anchoredPosition = knobPosition;

        if (currentStick == _movementStick)
        {
            _movementAmount = knobPosition / maxDistance;
            OnMove?.Invoke(_movementAmount);
        }
        else
        {
            _lookAmount = knobPosition / maxDistance;
            OnLook?.Invoke(_lookAmount);
        }
    }

    private void HandleFingerDown(Finger finger)
    {
        if (_movementFinger == null && finger.screenPosition.x < Screen.width / 2f)
        {
            _movementFinger = finger;
            _movementAmount = Vector2.zero;
            if (!_movementStick.gameObject.activeSelf)
                _movementStick.gameObject.SetActive(true);
            _movementStick.RectTransform.sizeDelta = _stickSize;
            _movementStick.RectTransform.anchoredPosition = ClampStartPosition(finger.screenPosition);
            OnMoveStarted?.Invoke();
        }
        else if (_lookFinger == null && finger.screenPosition.x >= Screen.width / 2f)
        {
            _lookFinger = finger;
            _lookAmount = Vector2.zero;
            if (!_lookStick.gameObject.activeSelf)
                _lookStick.gameObject.SetActive(true);
            _lookStick.RectTransform.sizeDelta = _stickSize;
            _lookStick.RectTransform.anchoredPosition = ClampStartPosition(finger.screenPosition);
            OnLookStarted?.Invoke();
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < _stickSize.x / 2f)
        {
            startPosition.x = _stickSize.x / 2f;
        }
        else if (startPosition.x > Screen.width - _stickSize.x / 2f)
        {
            startPosition.x = Screen.width - _stickSize.x / 2f;
        }

        if (startPosition.y < _stickSize.y / 2f)
        {
            startPosition.y = _stickSize.y / 2f;
        }
        else if (startPosition.y > Screen.height - _stickSize.y / 2f)
        {
            startPosition.y = Screen.height - _stickSize.y / 2f;
        }

        return startPosition;
    }
}
