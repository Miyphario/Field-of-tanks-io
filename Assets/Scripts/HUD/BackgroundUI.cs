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

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        GetComponent<Image>().color = _disabledColor;
    }

    public void Show()
    {
        LeanTween.cancel(_rectTransform);
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        LeanTween.color(_rectTransform, _defaultColor, 0.5f).setIgnoreTimeScale(true);
    }

    public void Hide()
    {
        LeanTween.cancel(_rectTransform);
        LeanTween.color(_rectTransform, _disabledColor, 0.25f).setIgnoreTimeScale(true).setOnComplete(() => 
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _defaultColor = GetComponent<Image>().color;
    }
#endif
}
