using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance { get; private set; }

    [SerializeField] private WorldManager _worldManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private HUDManager _hudManager;
    [SerializeField] private PrefabManager _prefabManager;
    [SerializeField] private ScreenshotManager _screenshotManager;
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private Yandex _yandex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LocalizationManager.Initialize();
        _ = new InputManager();
        _ = new GameManager();
        AdsManager.Initialize();

        _prefabManager.Initialize();
        _soundManager.Initialize();
        _worldManager.Initialize();
        _hudManager.Initialize();
        _cameraManager.Initialize();
        _screenshotManager.Initialize();
        _yandex.Initialize();
    }
}
