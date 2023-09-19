using UnityEngine;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour
{
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _disabledColor;
    private RectTransform _rectTransform;

    public void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        gameObject.Toggle(false);
        GetComponent<Image>().color = _disabledColor;
    }

    public void Show()
    {
        LeanTween.cancel(_rectTransform);
        gameObject.Toggle(true);
        LeanTween.color(_rectTransform, _defaultColor, 0.5f).setIgnoreTimeScale(true);
    }

    public void Hide()
    {
        LeanTween.cancel(_rectTransform);
        LeanTween.color(_rectTransform, _disabledColor, 0.25f).setIgnoreTimeScale(true).setOnComplete(() => 
        {
            gameObject.Toggle(false);
        });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _defaultColor = GetComponent<Image>().color;
    }
#endif
}
