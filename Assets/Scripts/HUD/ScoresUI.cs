using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoresUI : MonoBehaviour
{
    [SerializeField] private RectTransform _emptyText;
    [SerializeField] private ScoreUI _scorePrefab;
    [SerializeField] private RectTransform _scoresTransform;
    [SerializeField] private Vector2 _scoresAnimPos;
    private Vector2 _scoresDefaultAnimPos;
    [SerializeField] private Button _backButton;
    [SerializeField] private Vector2 _backButtonAnimPos;
    private Vector2 _backButtonDefaultAnimPos;
    [SerializeField] private LeanTweenType _animType;
    [SerializeField] private float _animSpeed = 0.5f;

    private void Awake()
    {
        Hide(true);
    }

    public void Initialize()
    {
        _scoresDefaultAnimPos = _scoresTransform.anchoredPosition;
        _backButtonDefaultAnimPos = _backButton.GetComponent<RectTransform>().anchoredPosition;

        GameManager.Instance.OnSaveLoaded += data => StartCoroutine(SetScoresIE(data.scores));
        GameManager.Instance.OnScoreAdded += AddNewScore;
        GameManager.Instance.OnScoreRemoved += HandleScoreRemoved;
    }

    private void HandleScoreRemoved(PlayerScore score)
    {
        if (GameManager.Instance.Scores.Count <= 1) _emptyText.gameObject.Toggle(true);
    }

    private void AddNewScore(PlayerScore score, int index)
    {
        _emptyText.gameObject.Toggle(false);
        ScoreUI sc = Instantiate(_scorePrefab, _scorePrefab.transform.parent);
        sc.SetScore(score);
        sc.Initialize();
        if (index > -1) sc.transform.SetSiblingIndex(index + 2);
        sc.gameObject.Toggle(true);
    }

    private void AddNewScore(PlayerScore score)
    {
        AddNewScore(score, -1);
    }

    public IEnumerator SetScoresIE(IEnumerable<PlayerScore> scores)
    {
        if (scores.Count() <= 0) _emptyText.gameObject.Toggle(true);

        for (int i = _scorePrefab.transform.parent.childCount - 1; i >= 2; i--)
        {
            Destroy(_scorePrefab.transform.parent.GetChild(i).gameObject);
        }

        foreach (PlayerScore sc in scores)
        {
            AddNewScore(sc);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public void Show()
    {
        LeanTween.cancel(_scoresTransform);
        LeanTween.cancel(_backButton.gameObject);
        _backButton.interactable = false;

        _backButton.gameObject.Toggle(true);
        _scoresTransform.gameObject.Toggle(true);

        LeanTween.move(_scoresTransform, _scoresDefaultAnimPos, _animSpeed).setIgnoreTimeScale(true).
            setEase(_animType);
        LeanTween.move(_backButton.GetComponent<RectTransform>(), _backButtonDefaultAnimPos, _animSpeed).setIgnoreTimeScale(true).
            setEase(_animType).setOnComplete(() => _backButton.interactable = true);
    }

    public void Hide(bool force = false)
    {
        LeanTween.cancel(_scoresTransform);
        LeanTween.cancel(_backButton.gameObject);
        _backButton.interactable = false;

        float animSpeed = _animSpeed;
        if (force)
        {
            _scoresTransform.gameObject.Toggle(false);
            _backButton.gameObject.Toggle(false);
            animSpeed = 0;
        }

        LeanTween.move(_scoresTransform, _scoresAnimPos, _animSpeed).setIgnoreTimeScale(true).
            setEase(_animType).setOnComplete(() => _scoresTransform.gameObject.Toggle(false));
        LeanTween.move(_backButton.GetComponent<RectTransform>(), _backButtonAnimPos, _animSpeed).setIgnoreTimeScale(true).
            setEase(_animType).setOnComplete(() => _backButton.gameObject.Toggle(false));
    }
}
