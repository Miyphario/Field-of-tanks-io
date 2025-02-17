using UnityEngine;

public class PlayerController : TankController
{
    public new Player Tank => base.Tank as Player;

    [SerializeField] private GameObject _shootLine;

    private Vector2 _moveInput;
    private Vector2 _currentMoveInput;
    public Vector2 MoveInput => _currentMoveInput;
    private Vector2 _smoothInput;

    private Vector2 _lookInput;
    private Vector2 _mousePosition;

    private bool _isLooking;

    protected override void Awake()
    {
        base.Awake();
        
        Tank.OnDestroyed += tank => 
        {
            DisabledInput = true;
            _moveInput = Vector2.zero;
            _currentMoveInput = Vector2.zero;
            _lookInput = Vector2.zero;
            ShootEnd();
        };
        GameManager.Instance.OnPauseChanged += HandlePauseToggled;
        GameManager.Instance.OnGameRestarting += () => 
        {
            StopInput();
            HandlePauseToggled(true);
        };
    }

    protected override void Start()
    {
        base.Start();
        StartInput();
    }

    public void Initialize()
    {
        _shootLine.Toggle(false);
        DisabledInput = false;
    }

    private void StartInput()
    {
        InputManager.Instance.OnShootStarted += ShootStartMouse;
        InputManager.Instance.OnShootEnded += ShootEnd;

        InputManager.Instance.OnFirstUpgrade += Tank.SelectUpgrade;
        InputManager.Instance.OnSecondUpgrade += Tank.SelectUpgrade;
        InputManager.Instance.OnThirdUpgrade += Tank.SelectUpgrade;
        InputManager.Instance.OnBack += Tank.UpgradeMenuBack;

#if UNITY_EDITOR
        InputManager.Instance.OnEscape += GameManager.Instance.TogglePause;
#endif

        // Touch
        if (Application.isMobilePlatform)
        {
            HUDManager.Instance.MobileControls.OnMove += Move;
            HUDManager.Instance.MobileControls.OnMoveEnded += Move;

            HUDManager.Instance.MobileControls.OnLookStarted += ShootStart;
            HUDManager.Instance.MobileControls.OnLook += Look;
            HUDManager.Instance.MobileControls.OnLookEnded += ShootEnd;
        }
        // Keyboard & Gamepad
        else
        {
            InputManager.Instance.OnMove += Move;

            InputManager.Instance.OnLookStarted += ShootStart;
            InputManager.Instance.OnLook += Look;
            InputManager.Instance.OnLookEnded += ShootEnd;

            InputManager.Instance.OnMouseMove += MouseMove;
        }

        InputManager.Instance.Enable();
    }

    private void StopInput()
    {
        InputManager.Instance.Disable();

        InputManager.Instance.OnShootStarted -= ShootStartMouse;
        InputManager.Instance.OnShootEnded -= ShootEnd;

        InputManager.Instance.OnFirstUpgrade -= Tank.SelectUpgrade;
        InputManager.Instance.OnSecondUpgrade -= Tank.SelectUpgrade;
        InputManager.Instance.OnThirdUpgrade -= Tank.SelectUpgrade;
        InputManager.Instance.OnBack -= Tank.UpgradeMenuBack;

#if UNITY_EDITOR
        InputManager.Instance.OnEscape -= GameManager.Instance.TogglePause;
#endif

        // Touch
        if (Application.isMobilePlatform)
        {
            HUDManager.Instance.MobileControls.OnMove -= Move;
            HUDManager.Instance.MobileControls.OnMoveEnded -= Move;

            HUDManager.Instance.MobileControls.OnLookStarted -= ShootStart;
            HUDManager.Instance.MobileControls.OnLook -= Look;
            HUDManager.Instance.MobileControls.OnLookEnded -= ShootEnd;
        }
        // Keyboard & Gamepad
        else
        {
            InputManager.Instance.OnMove -= Move;

            InputManager.Instance.OnLookStarted -= ShootStart;
            InputManager.Instance.OnLook -= Look;
            InputManager.Instance.OnLookEnded -= ShootEnd;

            InputManager.Instance.OnMouseMove -= MouseMove;
        }
    }

    private void Update()
    {
        _currentMoveInput = Vector2.SmoothDamp(_currentMoveInput, _moveInput, ref _smoothInput, _rb.drag / 60f);
        WorldManager.Instance.ClampMoveInput(_rb.position, ref _currentMoveInput);

        if (DisabledInput) return;
        Vector2 rotDirection;
        if (GameManager.Instance.CurrentInputControl == InputControl.Keyboard)
        {
            Vector2 mousePos = GameManager.Instance.MainCamera.ScreenToWorldPoint(_mousePosition);
            rotDirection = transform.position.DirectionToPoint(mousePos);
        }
        else
        {
            if (_lookInput == Vector2.zero)
                rotDirection = _moveInput;
            else
                rotDirection = _lookInput;
        }
        
        Quaternion toRot = QuaternionExt.RotationToDirection(rotDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, 1000f * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Tank.Speed * _currentMoveInput;
    }


    private void ShootStart()
    {
        if (DisabledInput) return;

        Tank.Gun.ShootStart();
        _shootLine.Toggle(true);
    }

    private void ShootStartMouse()
    {
        if (HUDManager.IsPointerOnUI(_mousePosition)) return;

        ShootStart();
    }

    private void ShootEnd()
    {
        Tank.Gun.ShootEnd();
        _isLooking = false;
        _shootLine.Toggle(false);
    }

    private void Look(Vector2 input)
    {
        if (DisabledInput) return;

        _lookInput = input;
        _isLooking = true;
    }

    private void Move(Vector2 input)
    {
        if (DisabledInput) return;

        _moveInput = input;

        if (GameManager.Instance.CurrentInputControl != InputControl.Keyboard)
        {
            if (_moveInput != Vector2.zero)
                if (!_isLooking)
                    _lookInput = _moveInput;
        }
    }

    private void MouseMove(Vector2 position)
    {
        if (DisabledInput) return;
        _mousePosition = position;
    }

    private void HandlePauseToggled(bool pause)
    {
        if (!pause)
        {
            DisabledInput = false;
            return;
        }

        ShootEnd();
        Move(Vector2.zero);
        DisabledInput = true;
    }
}
