using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private BackgroundUI _darkBackground;
    public static HUDManager Instance { get; private set; }
    public RectTransform CanvasRect => _canvasRect;
    public float HealthbarRenderDistance => GameManager.Instance.MainCamera.orthographicSize + 10f;
    [SerializeField] private RectTransform _canvasRect;

    [Header("Bars")]
    [SerializeField] private RectTransform _tankBarsTransform;
    [SerializeField] private RectTransform _destructibleBarsTransform;

    [Header("HUDs")]
    [SerializeField]
    private UpgradesUI _upgradesUI;
    [SerializeField] private MobileControls _mobileControls;
    public MobileControls MobileControls => _mobileControls;
    [SerializeField] private PlayerInfoUI _playerInfoUI;
    [SerializeField] private PauseUI _pauseUI;
    public PauseUI PauseUI => _pauseUI;

    public void Initialize()
    {
        Instance = this;

        //if (Application.isMobilePlatform)
            _mobileControls.Initialize();
        //else
            //_mobileControls.gameObject.SetActive(false);

        _upgradesUI.Initialize();
        _playerInfoUI.Initialize();
        _darkBackground.Initialize();
        _pauseUI.Initialize();

        GameManager.Instance.OnPauseChanged += pause =>
        {
            if (pause)
            {
                _darkBackground.Show();
            }
            else
            {
                _darkBackground.Hide();
            }
        };
    }

    public BarUI CreateHealthbar(bool smallBar = false)
    {
        BarUI bar = null;
        RectTransform parent = smallBar ? _destructibleBarsTransform : _tankBarsTransform;

        for (int i = parent.childCount - 1;  i >= 1; i--)
        {
            bar = parent.GetChild(i).GetComponent<BarUI>();
            if (bar.IsFree)
                break;
            else
                bar = null;
        }

        if (bar == null)
            bar = Instantiate(parent.GetChild(0), parent).GetComponent<BarUI>();

        return bar;
    }

    public static bool IsPointerOnUI(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new(EventSystem.current){ position = screenPosition };
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        return raycastResults.Count > 0;
    }
}
