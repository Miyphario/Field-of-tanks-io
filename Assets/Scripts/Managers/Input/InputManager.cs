using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager
{
    public static InputManager Instance { get; private set; }

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnLookStarted;
    public event Action OnLookEnded;
    public event Action OnShootStarted;
    public event Action OnShootEnded;

    public event Action<Vector2, float> OnTouchStart;
    public event Action<Vector2, float> OnTouchEnd;
    public event Action<Vector2> OnMouseMove;

    public event Action<int> OnFirstUpgrade;
    public event Action<int> OnSecondUpgrade;
    public event Action<int> OnThirdUpgrade;
    public event Action OnBack;
    public event Action OnEscape;

    private readonly PlayerControls _controls = new();

    public InputManager()
    {
        Instance = this;

        Disable();

        _controls.Player.Move.started += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        _controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        _controls.Player.Move.canceled += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());

        _controls.Player.Look.started += ctx => OnLookStarted?.Invoke();
        _controls.Player.Look.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        _controls.Player.Look.canceled += ctx => OnLookEnded?.Invoke();

        _controls.Player.Shoot.started += ctx => OnShootStarted?.Invoke();
        _controls.Player.Shoot.canceled += ctx => OnShootEnded?.Invoke();

        _controls.Player.TouchPress.started += ctx => OnTouchStart?.Invoke(_controls.Player.TouchPosition.ReadValue<Vector2>(), (float)ctx.startTime);
        _controls.Player.TouchPress.canceled += ctx => OnTouchEnd?.Invoke(_controls.Player.TouchPosition.ReadValue<Vector2>(), (float)ctx.time);

        _controls.Player.TouchPosition.performed += ctx => OnMouseMove?.Invoke(_controls.Player.TouchPosition.ReadValue<Vector2>());
        _controls.Player.TouchPosition.canceled += ctx => OnMouseMove?.Invoke(_controls.Player.TouchPosition.ReadValue<Vector2>());

        _controls.Player.FirstUpgrade.canceled += ctx => OnFirstUpgrade?.Invoke(1);
        _controls.Player.SecondUpgrade.canceled += ctx => OnSecondUpgrade?.Invoke(2);
        _controls.Player.ThirdUpgrade.canceled += ctx => OnThirdUpgrade?.Invoke(3);

        _controls.Player.Back.canceled += ctx => OnBack?.Invoke();
        _controls.Player.Escape.canceled += ctx => OnEscape?.Invoke();
    }

    private void FingerDown(Finger finger)
    {
        if (OnTouchStart == null) return;
        OnTouchStart?.Invoke(finger.screenPosition, Time.time);
    }

    public void Enable()
    {
        _controls?.Enable();

        if (Application.isMobilePlatform)
        {
            //TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        }
    }

    public void Disable()
    {
        if (TouchSimulation.instance != null && TouchSimulation.instance.enabled)
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
            EnhancedTouchSupport.Disable();
            //TouchSimulation.Disable();
        }

        _controls?.Disable();
    }
}
