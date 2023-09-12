using UnityEngine;

public class FloatingStick : MonoBehaviour
{
    [SerializeField] private RectTransform _knob;
    public RectTransform Knob => _knob;
    private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;

    public void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
