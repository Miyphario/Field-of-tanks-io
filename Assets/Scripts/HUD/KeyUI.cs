using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    public void Change(KeyCode key)
    {
        _image.type = Image.Type.Simple;

        _image.sprite = KeyUIManager.Instance.KeyboardButton;
        _image.color = KeyUIManager.Instance.KeyboardDefaultColor;
        _text.text = KeyUIManager.Instance.KeyToString(key);
        _text.rectTransform.offsetMin = KeyUIManager.Instance.KeyboardTextAlignmentMin;
        _text.rectTransform.offsetMax = KeyUIManager.Instance.KeyboardTextAlignmentMax;

        _image.preserveAspect = true;
    }

    public void Change(GamepadButton button)
    {
        _image.type = Image.Type.Simple;
        _text.rectTransform.offsetMin = KeyUIManager.Instance.GamepadTextAlignmentMin;
        _text.rectTransform.offsetMax = KeyUIManager.Instance.GamepadTextAlignmentMax;

        if (button != GamepadButton.A && button != GamepadButton.B &&
            button != GamepadButton.X && button != GamepadButton.Y)
        {
            _image.color = KeyUIManager.Instance.GamepadDefaultColor;
        }
        else
        {
            _image.sprite = KeyUIManager.Instance.GamepadCircleButton;
        }

        if (button <= GamepadButton.Left)
        {
            _text.text = button.ToString();
        }
        else if (button <= GamepadButton.LSLeft)
        {
            _text.text = "LS";
        }
        else
        {
            _text.text = "RS";
        }

        switch (button)
        {
            case GamepadButton.A:
                _image.color = KeyUIManager.Instance.GamepadAColor;
                break;

            case GamepadButton.B:
                _image.color = KeyUIManager.Instance.GamepadBColor;
                break;

            case GamepadButton.X:
                _image.color = KeyUIManager.Instance.GamepadXColor;
                break;

            case GamepadButton.Y:
                _image.color = KeyUIManager.Instance.GamepadYColor;
                break;

            case GamepadButton.RT:
                _image.sprite = KeyUIManager.Instance.GamepadRightTriggerButton;
                break;

            case GamepadButton.LT:
                _image.sprite = KeyUIManager.Instance.GamepadLeftTriggerButton;
                break;

            case GamepadButton.LB:
                _image.sprite = KeyUIManager.Instance.GamepadLeftShoulderButton;
                break;

            case GamepadButton.RB:
                _image.sprite = KeyUIManager.Instance.GamepadRightShoulderButton;
                break;

            case GamepadButton.Back:
                _image.sprite = KeyUIManager.Instance.GamepadBackButton;
                break;

            case GamepadButton.Start:
                _image.sprite = KeyUIManager.Instance.GamepadStartButton;
                break;

            case GamepadButton.Up:
                _image.sprite = KeyUIManager.Instance.GamepadUp;
                break;

            case GamepadButton.Right:
                _image.sprite = KeyUIManager.Instance.GamepadRight;
                break;

            case GamepadButton.Down:
                _image.sprite = KeyUIManager.Instance.GamepadDown;
                break;

            case GamepadButton.Left:
                _image.sprite = KeyUIManager.Instance.GamepadLeft;
                break;

            case GamepadButton.RS:
            case GamepadButton.LS:
                _image.sprite = KeyUIManager.Instance.GamepadStickPress;
                break;

            case GamepadButton.RSUp:
            case GamepadButton.LSUp:
                _image.sprite = KeyUIManager.Instance.GamepadStickUp;
                break;
            
            case GamepadButton.RSRight:
            case GamepadButton.LSRight:
                _image.sprite = KeyUIManager.Instance.GamepadStickUp;
                break;

            case GamepadButton.RSDown:
            case GamepadButton.LSDown:
                _image.sprite = KeyUIManager.Instance.GamepadStickDown;
                break;

            case GamepadButton.RSLeft:
            case GamepadButton.LSLeft:
                _image.sprite = KeyUIManager.Instance.GamepadStickLeft;
                break;
        }

        _image.preserveAspect = true;
    }
}
