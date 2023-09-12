using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    public bool IsFree { get; private set; } = true;
    [SerializeField] private Vector3 _offset;

    private GameObject _owner;
    private Slider _slider;
    private float _curBarValue;
    private RectTransform _rectTransform;

    public void Initialize(GameObject owner, float minValue, float maxValue, bool enable = false)
    {
        if (_slider == null)
            _slider = GetComponent<Slider>();

        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        IsFree = false;

        _owner = owner;
        _slider.minValue = minValue;
        _slider.maxValue = maxValue;

        _curBarValue = maxValue;
        _slider.value = maxValue;
        UpdatePosition();

        if (enable)
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
    }

    private void Update()
    {
        _slider.value = Mathf.Lerp(_slider.value, _curBarValue, 8f * Time.deltaTime);
        UpdatePosition();
    }

    public void SetValue(float value, bool force = false)
    {
        _curBarValue = value;

        if (force)
            _slider.value = _curBarValue;
    }

    public void SetMaxValue(float maxValue)
    {
        _slider.maxValue = maxValue;
    }

    public void Enable()
    {
        if (gameObject.activeSelf) return;

        UpdatePosition();
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _owner = null;
        IsFree = true;
        Disable();
    }

    public void UpdatePosition()
    {
        if (_owner == null)
        {
            Dispose();
            return;
        }

        RectTransform canvasRect = HUDManager.Instance.CanvasRect;
        Vector2 goalPos = GameManager.Instance.MainCamera.WorldToViewportPoint(_owner.transform.position + _offset);
        Vector2 screenPos = new(goalPos.x * canvasRect.sizeDelta.x, goalPos.y * canvasRect.sizeDelta.y);

        _rectTransform.anchoredPosition = screenPos;
    }
}
