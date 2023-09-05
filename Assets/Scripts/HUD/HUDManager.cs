using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    public RectTransform CanvasRect => _canvasRect;
    public float HealthbarRenderDistance => GameManager.Instance.MainCamera.orthographicSize + 10f;
    public const float HEALTHBAR_RENDER_TIME = 2f;

    [SerializeField] private RectTransform _canvasRect;
    [SerializeField, Header("Bars")] private RectTransform _tankBarsTransform;
    [SerializeField] private RectTransform _destructibleBarsTransform;

    [SerializeField, Header("HUDs")]
    private UpgradesUI _upgradesUI;
    [SerializeField] private MobileControls _mobileControls;
    public MobileControls MobileControls => _mobileControls;
    [SerializeField] private PlayerInfoUI _playerInfoUI;

    public void Initialize()
    {
        Instance = this;

        if (Application.isMobilePlatform)
            _mobileControls.Initialize();
        else
            _mobileControls.gameObject.SetActive(false);

        _upgradesUI.Initialize();
        _playerInfoUI.Initialize();
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
}
