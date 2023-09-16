using TMPro;
using UnityEngine;

public class FpsCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private int _frameCounter;
    private float _timeCounter;
    private float _lastFrameRate;
    private float _refreshTime = 0.5f;

    private void Update()
    {
        if (_timeCounter < _refreshTime)
        {
            _timeCounter += Time.deltaTime;
            _frameCounter++;
        }
        else
        {
            _lastFrameRate = _frameCounter/_timeCounter;
            _frameCounter = 0;
            _timeCounter = 0;
            _text.text = "fps: " + Mathf.RoundToInt(_lastFrameRate);
        }
    }
}
