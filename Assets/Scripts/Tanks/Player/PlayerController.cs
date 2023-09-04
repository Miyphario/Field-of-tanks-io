using UnityEngine;

public class PlayerController : TankController
{
    public new Player Tank => base.Tank as Player;

    private Vector2 _input;
    private Vector2 _currentMoveInput;
    public Vector2 MoveInput => _currentMoveInput;
    private Vector2 _smoothInput;

    private Vector2 _lookInput;
    private Vector2 _mousePosition;

    protected override void Start()
    {
        base.Start();

        InputManager.Instance.OnMove += inp => _input = inp;
        InputManager.Instance.OnLook += inp => _lookInput = inp;

        InputManager.Instance.OnLookStarted += inp => Tank.Gun.ShootStart();
        InputManager.Instance.OnLookEnded += inp => Tank.Gun.ShootEnd();

        InputManager.Instance.OnShootStarted += () => Tank.Gun.ShootStart();
        InputManager.Instance.OnShootEnded += () => Tank.Gun.ShootEnd();

        InputManager.Instance.OnMouseMove += pos => _mousePosition = pos;

        InputManager.Instance.OnFirstUpgrade += () => Tank.SelectUpgrade(1);
        InputManager.Instance.OnSecondUpgrade += () => Tank.SelectUpgrade(2);
        InputManager.Instance.OnThirdUpgrade += () => Tank.SelectUpgrade(3);
    }

    private void Update()
    {
        _currentMoveInput = Vector2.SmoothDamp(_currentMoveInput, _input, ref _smoothInput, _rb.drag / 60f);
        _currentMoveInput = WorldManager.Instance.ClampMoveInput(_rb.position, _currentMoveInput);

        if (GameManager.Instance.IsKeyboardControls)
        {
            Vector2 mousePos = GameManager.Instance.MainCamera.ScreenToWorldPoint(_mousePosition);
            RotateToPoint(mousePos, -90f);
        }
        else
        {
            Vector2 inp;
            if (_lookInput == Vector2.zero)
                inp = _input;
            else
                inp = _lookInput;

            transform.rotation = RotationToDirection(inp);
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = Tank.Speed * _currentMoveInput;
    }
}
