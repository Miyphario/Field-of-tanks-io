using System.Text;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private PlayerScore _score;

    public void Initialize()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        GameManager.Instance.OnScoreRemoved += RemoveScore;
    }

    private void RemoveScore(PlayerScore score)
    {
        if (score == _score)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateText()
    {
        StringBuilder sb = new();

        // Default values
        string frText = LocalizationManager.GetLocalizedText("score.Frags");
        if (frText == string.Empty) frText = "Frags";
        string lfText = LocalizationManager.GetLocalizedText("score.Lifetime");
        if (lfText == string.Empty) lfText = "Lifetime";
        string lvText = LocalizationManager.GetLocalizedText("score.Level");
        if (lvText == string.Empty) lvText = "Level";

        sb.Append(frText + ": " + _score.frags + ", ");
        sb.Append(lfText + ": " + NumToString.GetReducedTime(_score.time) + ", ");
        sb.AppendLine(lvText + ": " + _score.level);
        sb.Append(_score.date);

        _text.text = sb.ToString();
    }

    public void SetScore(PlayerScore score)
    {
        _score = score;
        UpdateText();
    }
}
