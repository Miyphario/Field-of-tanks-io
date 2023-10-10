using UnityEngine;

public class KeyUIManager : MonoBehaviour
{
    public static KeyUIManager Instance { get; private set; }

    [Header("Keyboard")]
    [SerializeField] private Sprite _keyboardButton;
    public Sprite KeyboardButton => _keyboardButton;
    [SerializeField] private Color _keyboardDefaultColor;
    public Color KeyboardDefaultColor => _keyboardDefaultColor;
    [SerializeField] private Vector2 _keyboardTextAlignmentMin;
    public Vector2 KeyboardTextAlignmentMin => _keyboardTextAlignmentMin;
    [SerializeField] private Vector2 _keyboardTextAlignmentMax;
    public Vector2 KeyboardTextAlignmentMax => _keyboardTextAlignmentMax;

    [Header("Gamepad")]
    [SerializeField] private Sprite _gamepadCircleButton;
    public Sprite GamepadCircleButton => _gamepadCircleButton;
    [SerializeField] private Vector2 _gamepadTextAlignmentMin;
    public Vector2 GamepadTextAlignmentMin => _gamepadTextAlignmentMin;
    [SerializeField] private Vector2 _gamepadTextAlignmentMax;
    public Vector2 GamepadTextAlignmentMax => _gamepadTextAlignmentMax;
    [SerializeField] private Color _gamepadAColor;
    public Color GamepadAColor => _gamepadAColor;
    [SerializeField] private Color _gamepadBColor;
    public Color GamepadBColor => _gamepadBColor;
    [SerializeField] private Color _gamepadXColor;
    public Color GamepadXColor => _gamepadXColor;
    [SerializeField] private Color _gamepadYColor;
    public Color GamepadYColor => _gamepadYColor;

    [SerializeField] private Sprite _gamepadLeftTriggerButton;
    public Sprite GamepadLeftTriggerButton => _gamepadLeftTriggerButton;
    [SerializeField] private Sprite _gamepadRightTriggerButton;
    public Sprite GamepadRightTriggerButton => _gamepadRightTriggerButton;
    [SerializeField] private Sprite _gamepadLeftShoulderButton;
    public Sprite GamepadLeftShoulderButton => _gamepadLeftShoulderButton;
    [SerializeField] private Sprite _gamepadRightShoulderButton;
    public Sprite GamepadRightShoulderButton => _gamepadRightShoulderButton;
    [SerializeField] private Sprite _gamepadStartButton;
    public Sprite GamepadStartButton => _gamepadStartButton;
    [SerializeField] private Sprite _gamepadBackButton;
    public Sprite GamepadBackButton => _gamepadBackButton;
    [SerializeField] private Sprite _gamepadStickPress;
    public Sprite GamepadStickPress => _gamepadStickPress;
    [SerializeField] private Sprite _gamepadStickUp;
    public Sprite GamepadStickUp => _gamepadStickUp;
    [SerializeField] private Sprite _gamepadStickRight;
    public Sprite GamepadStickRight => _gamepadStickRight;
    [SerializeField] private Sprite _gamepadStickDown;
    public Sprite GamepadStickDown => _gamepadStickDown;
    [SerializeField] private Sprite _gamepadStickLeft;
    public Sprite GamepadStickLeft => _gamepadStickLeft;
    [SerializeField] private Sprite _gamepadUp;
    public Sprite GamepadUp => _gamepadUp;
    [SerializeField] private Sprite _gamepadRight;
    public Sprite GamepadRight => _gamepadRight;
    [SerializeField] private Sprite _gamepadDown;
    public Sprite GamepadDown => _gamepadDown;
    [SerializeField] private Sprite _gamepadLeft;
    public Sprite GamepadLeft => _gamepadLeft;
    [SerializeField] private Color _gamepadDefaultColor;
    public Color GamepadDefaultColor => _gamepadDefaultColor;

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public string KeyToString(KeyCode key)
    {
        if (key.ToString().ToLower().StartsWith("alpha"))
        {
            return key.ToString()[^1].ToString();
        }

        return key.ToString();
    }
}
