using UnityEngine;
using TMPro;

public class LocalizationText : MonoBehaviour
{
    [SerializeField] private string _key;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (_key != null && _key.Trim() != string.Empty)
        {
            LocalizationManager.OnLanguageChanged += UpdateText;
            UpdateText();
        }
    }

    private void OnDestroy()
    {
        if (_key != null && _key.Trim() != string.Empty)
            LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (gameObject == null) return;
        if (_key == string.Empty)
        {
            _text.text = _key;
            return;
        }

        string newText = LocalizationManager.GetLocalizedText(_key);
        if (newText == null || newText.Trim() == string.Empty) return;
        _text.text = newText;
    }

    public void SetKey(string key)
    {
        bool handleExists = _key != null && _key.Trim() != string.Empty;
        _key = key;
        if (key == string.Empty)
        {
            if (handleExists)
                LocalizationManager.OnLanguageChanged -= UpdateText;
        }
        else
        {
            if (!handleExists)
                LocalizationManager.OnLanguageChanged += UpdateText;
        }

        UpdateText();
    }
}
