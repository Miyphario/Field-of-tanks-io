using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private Button _firstUpgradeButton;
    [SerializeField] private Image _firstButtonImage;
    private TextMeshProUGUI _firstButtonText;

    [SerializeField] private Button _secondUpgradeButton;
    [SerializeField] private Image _secondButtonImage;
    private TextMeshProUGUI _secondButtonText;

    [SerializeField] private Button _thirdUpgradeButton;
    [SerializeField] private Image _thirdButtonImage;
    private TextMeshProUGUI _thirdButtonText;

    [SerializeField] private Button _backButton;

    [Header("Images")]
    [SerializeField]
    private Sprite _tankSprite;
    [SerializeField] private Sprite _gunSprite;
    [SerializeField] private Sprite _healthSprite;
    [SerializeField] private Sprite _speedSprite;
    [SerializeField] private Sprite _damageSprite;
    [SerializeField] private Sprite _fireRateSprite;
    [SerializeField] private Sprite _bulletSpeedSprite;
    [SerializeField] private Sprite _touchDamageSprite;

    [Header("Menus")]
    [SerializeField] private RectTransform _menuRect;
    [Header("Animations")]
    private Vector2 _defaultMenuPosition;
    [SerializeField] private Vector2 _animMenuPosition;
    private Vector2 _defaultBackButtonPosition;
    [SerializeField] private Vector2 _animBackButtonPosition;
    [SerializeField] private LeanTweenType _animType;

    public void Initialize()
    {
        WorldManager.Instance.OnPlayerCreated += player => player.OnMenuSelected += HandlePlayerMenuSelected;

        _firstButtonText = _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
        _secondButtonText = _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
        _thirdButtonText = _thirdUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();

        _firstUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(1));
        _secondUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(2));
        _thirdUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(3));
        _backButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.UpgradeMenuBack());

        _defaultMenuPosition = _menuRect.anchoredPosition;

        RectTransform backButtonRect = _backButton.GetComponent<RectTransform>();
        _defaultBackButtonPosition = backButtonRect.anchoredPosition;
    }

    private void HandlePlayerMenuSelected(UpgradeMenu menu)
    {
        Player pl = WorldManager.Instance.HostPlayer;

        switch (menu)
        {
            case UpgradeMenu.None:
                ShowButtons(0);
                break;

            case UpgradeMenu.Base:
                _firstButtonImage.sprite = _tankSprite;
                _firstButtonText.GetComponent<LocalizationText>().SetKey("up.Tank");
                _firstUpgradeButton.interactable = true;

                _secondButtonImage.sprite = _gunSprite;
                _secondButtonText.GetComponent<LocalizationText>().SetKey("up.Gun");
                _secondUpgradeButton.interactable = true;

                ShowButtons(2);
                ToggleBackButton(false);
                break;

            case UpgradeMenu.Tank:
                _firstButtonImage.sprite = _healthSprite;
                if (pl.CanUpgrade(UpgradeType.MaxHealth))
                {
                    _firstUpgradeButton.interactable = true;
                    _firstButtonText.GetComponent<LocalizationText>().SetKey("up.Health");
                }
                else
                {
                    _firstUpgradeButton.interactable = false;
                    _firstButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                _secondButtonImage.sprite = _speedSprite;
                if (pl.CanUpgrade(UpgradeType.Speed))
                {
                    _secondUpgradeButton.interactable = true;
                    _secondButtonText.GetComponent<LocalizationText>().SetKey("up.Speed");
                }
                else
                {
                    _secondUpgradeButton.interactable = false;
                    _secondButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                _thirdButtonImage.sprite = _touchDamageSprite;
                if (pl.CanUpgrade(UpgradeType.TouchDamage))
                {
                    _thirdUpgradeButton.interactable = true;
                    _thirdButtonText.GetComponent<LocalizationText>().SetKey("up.Damage");
                }
                else
                {
                    _thirdUpgradeButton.interactable = false;
                    _thirdButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                ShowButtons(3);
                ToggleBackButton(true);
                break;

            case UpgradeMenu.Gun:
                _firstButtonImage.sprite = _damageSprite;
                if (pl.CanUpgrade(UpgradeType.Damage))
                {
                    _firstUpgradeButton.interactable = true;
                    _firstButtonText.GetComponent<LocalizationText>().SetKey("up.Damage");
                }
                else
                {
                    _firstUpgradeButton.interactable = false;
                    _firstButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                _secondButtonImage.sprite = _fireRateSprite;
                if (pl.CanUpgrade(UpgradeType.FireRate))
                {
                    _secondUpgradeButton.interactable = true;
                    _secondButtonText.GetComponent<LocalizationText>().SetKey("up.FireRate");
                }
                else
                {
                    _secondUpgradeButton.interactable = false;
                    _secondButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                _thirdButtonImage.sprite = _bulletSpeedSprite;
                if (pl.CanUpgrade(UpgradeType.BulletSpeed))
                {
                    _thirdUpgradeButton.interactable = true;
                    _thirdButtonText.GetComponent<LocalizationText>().SetKey("up.BulletSpeed");
                }
                else
                {
                    _thirdUpgradeButton.interactable = false;
                    _thirdButtonText.GetComponent<LocalizationText>().SetKey("up.Max");
                }

                ShowButtons(3);
                ToggleBackButton(true);
                break;

            case UpgradeMenu.NewGun:
                GameObject[] guns = PrefabManager.Instance.GetGunsByLevel(WorldManager.Instance.HostPlayer.Level);
                if (guns.Length <= 0) return;

                _firstButtonImage.sprite = guns[0].GetComponent<Gun>().UISprite;
                _firstButtonText.GetComponent<LocalizationText>().SetKey(string.Empty);
                _firstUpgradeButton.interactable = true;

                _secondButtonImage.sprite = guns[1].GetComponent<Gun>().UISprite;
                _secondButtonText.GetComponent<LocalizationText>().SetKey(string.Empty);
                _secondUpgradeButton.interactable = true;

                ShowButtons(2);
                ToggleBackButton(false);
                break;
        }
    }

    public void ShowButtons(int count, bool force = false)
    {
        if (count == 0)
        {
            ToggleMenu(false, force);
            return;
        }
        else
        {
            ToggleMenu(true, force);
        }

        switch (count)
        {
            case 1:
                _firstUpgradeButton.gameObject.Toggle(true);
                _secondUpgradeButton.gameObject.Toggle(false);
                _thirdUpgradeButton.gameObject.Toggle(false);
                break;

            case 2:
                _firstUpgradeButton.gameObject.Toggle(true);
                _secondUpgradeButton.gameObject.Toggle(true);
                _thirdUpgradeButton.gameObject.Toggle(false);
                break;

            case 3:
                _firstUpgradeButton.gameObject.Toggle(true);
                _secondUpgradeButton.gameObject.Toggle(true);
                _thirdUpgradeButton.gameObject.Toggle(true);
                break;
        }

        if (_firstUpgradeButton.gameObject.activeSelf && _firstButtonImage.sprite != null)
        {
            _firstButtonImage.preserveAspect = true;
        }

        if (_secondUpgradeButton.gameObject.activeSelf && _secondButtonImage.sprite != null)
        {
            _secondButtonImage.preserveAspect = true;
        }

        if (_thirdUpgradeButton.gameObject.activeSelf && _thirdButtonImage.sprite != null)
        {
            _thirdButtonImage.preserveAspect = true;
        }
    }

    private void ToggleBackButton(bool active, bool force = false)
    {
        if (!force && _backButton.gameObject.activeSelf == active) return;

        RectTransform backButtonRect = _backButton.GetComponent<RectTransform>();
        LeanTween.cancel(backButtonRect);
        float animTime = force ? 0f : 0.2f;

        if (active)
        {
            _backButton.gameObject.Toggle(true);
            LeanTween.move(backButtonRect, _defaultBackButtonPosition, animTime).setIgnoreTimeScale(true).setEase(_animType);
        }
        else
        {
            if (force)
            {
                _backButton.gameObject.Toggle(false);
            }

            LeanTween.move(backButtonRect, _animBackButtonPosition, animTime).setIgnoreTimeScale(true).setEase(_animType).
            setOnComplete(() => 
            {
                _backButton.gameObject.Toggle(false);
            });
        }
    }

    public void ToggleMenu(bool enable, bool force = false)
    {
        if (!force && _menuRect.gameObject.activeSelf == enable) return;

        LeanTween.cancel(_menuRect);
        float animTime = force ? 0f : 0.45f;

        if (enable)
        {
            _menuRect.gameObject.Toggle(true);
            LeanTween.move(_menuRect, _defaultMenuPosition, animTime).setIgnoreTimeScale(true).setEase(_animType);
        }
        else
        {
            if (force)
            {
                _menuRect.gameObject.Toggle(false);
            }

            ToggleBackButton(enable, force);
            LeanTween.move(_menuRect, _animMenuPosition, animTime).setIgnoreTimeScale(true).setEase(_animType).
            setOnComplete(() => 
            {
                _menuRect.gameObject.Toggle(false);
            });
        }
    }
}
