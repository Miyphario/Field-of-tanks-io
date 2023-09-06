using UnityEngine;

public class PlayerController : TankController
{
    public new Player Tank => base.Tank as Player;

    private Vector2 _moveInput;
    private Vector2 _currentMoveInput;
    public Vector2 MoveInput => _currentMoveInput;
    private Vector2 _smoothInput;

    private Vector2 _lookInput;
    private Vector2 _mousePosition;

    private bool _isLooking;

    protected override void Start()
    {
        base.Start();

        InputManager.Instance.OnMove += Move;

        InputManager.Instance.OnLookStarted += ShootStart;
        InputManager.Instance.OnLook += Look;
        InputManager.Instance.OnLookEnded += ShootEnd;

        InputManager.Instance.OnShootStarted += ShootStart;
        InputManager.Instance.OnShootEnded += ShootEnd;

        InputManager.Instance.OnMouseMove += pos => _mousePosition = pos;

        InputManager.Instance.OnFirstUpgrade += () => Tank.SelectUpgrade(1);
        InputManager.Instance.OnSecondUpgrade += () => Tank.SelectUpgrade(2);
        InputManager.Instance.OnThirdUpgrade += () => Tank.SelectUpgrade(3);

        // Mobile controls
        if (Application.isMobilePlatform)
        {
            HUDManager.Instance.MobileControls.OnMove += Move;
            HUDManager.Instance.MobileControls.OnMoveEnded += () => Move(Vector2.zero);

            HUDManager.Instance.MobileControls.OnLookStarted += ShootStart;
            HUDManager.Instance.MobileControls.OnLook += Look;
            HUDManager.Instance.MobileControls.OnLookEnded += ShootEnd;
        }
    }

    private void Update()
    {
        _currentMoveInput = Vector2.SmoothDamp(_currentMoveInput, _moveInput, ref _smoothInput, _rb.drag / 60f);
        _currentMoveInput = WorldManager.Instance.ClampMoveInput(_rb.position, _currentMoveInput);

        Vector2 rotDirection;
        if (GameManager.Instance.IsKeyboardControls)
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

        transform.rotation = transform.rotation.RotationToDirection(transform.rotation, rotDirection);
    }

    private void FixedUpdate()
    {
        _rb.velocity = Tank.Speed * _currentMoveInput;
    }


    private void ShootStart()
    {
        Tank.Gun.ShootStart();
    }

    private void ShootEnd()
    {
        Tank.Gun.ShootEnd();
        _isLooking = false;
    }

    private void Look(Vector2 input)
    {
        _lookInput = input;
        _isLooking = true;
    }

    private void Move(Vector2 input)
    {
        _moveInput = input;

        if (!GameManager.Instance.IsKeyboardControls)
        {
            if (_moveInput != Vector2.zero)
                if (!_isLooking)
                    _lookInput = _moveInput;
        }
    }
}
