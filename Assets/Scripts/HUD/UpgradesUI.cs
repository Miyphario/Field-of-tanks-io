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

    [SerializeField, Header("Images")]
    private Sprite _tankSprite;
    [SerializeField] private Sprite _gunSprite;
    [SerializeField] private Sprite _healthSprite;
    [SerializeField] private Sprite _speedSprite;
    [SerializeField] private Sprite _damageSprite;
    [SerializeField] private Sprite _fireRateSprite;
    [SerializeField] private Sprite _bulletSpeedSprite;

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

        SetActiveButtons(0);
        SetBackButtonActive(false);
    }

    private void HandlePlayerMenuSelected(UpgradeMenu menu)
    {
        Player pl = WorldManager.Instance.HostPlayer;

        switch (menu)
        {
            case UpgradeMenu.None:
                SetActiveButtons(0);
                SetBackButtonActive(false);
                break;

            case UpgradeMenu.Base:
                _firstButtonImage.sprite = _tankSprite;
                _firstButtonText.text = "Tank";
                _firstUpgradeButton.interactable = true;

                _secondButtonImage.sprite = _gunSprite;
                _secondButtonText.text = "Gun";
                _secondUpgradeButton.interactable = true;

                SetActiveButtons(2);
                SetBackButtonActive(false);
                break;

            case UpgradeMenu.Tank:
                _firstButtonImage.sprite = _healthSprite;
                if (pl.CanUpgrade(UpgradeType.MaxHealth))
                {
                    _firstUpgradeButton.interactable = true;
                    _firstButtonText.text = "Health";
                }
                else
                {
                    _firstUpgradeButton.interactable = false;
                    _firstButtonText.text = "MAX";
                }

                _secondButtonImage.sprite = _speedSprite;
                if (pl.CanUpgrade(UpgradeType.Speed))
                {
                    _secondUpgradeButton.interactable = true;
                    _secondButtonText.text = "Speed";
                }
                else
                {
                    _secondUpgradeButton.interactable = false;
                    _secondButtonText.text = "MAX";
                }

                _thirdButtonImage.sprite = _damageSprite;
                if (pl.CanUpgrade(UpgradeType.TouchDamage))
                {
                    _thirdUpgradeButton.interactable = true;
                    _thirdButtonText.text = "Damage";
                }
                else
                {
                    _thirdUpgradeButton.interactable = false;
                    _thirdButtonText.text = "MAX";
                }

                SetActiveButtons(3);
                SetBackButtonActive(true);
                break;

            case UpgradeMenu.Gun:
                _firstButtonImage.sprite = _damageSprite;
                if (pl.CanUpgrade(UpgradeType.Damage))
                {
                    _firstUpgradeButton.interactable = true;
                    _firstButtonText.text = "Damage";
                }
                else
                {
                    _firstUpgradeButton.interactable = false;
                    _firstButtonText.text = "MAX";
                }

                _secondButtonImage.sprite = _fireRateSprite;
                if (pl.CanUpgrade(UpgradeType.FireRate))
                {
                    _secondUpgradeButton.interactable = true;
                    _secondButtonText.text = "Fire Rate";
                }
                else
                {
                    _secondUpgradeButton.interactable = false;
                    _secondButtonText.text = "MAX";
                }

                _thirdButtonImage.sprite = _bulletSpeedSprite;
                if (pl.CanUpgrade(UpgradeType.BulletSpeed))
                {
                    _thirdUpgradeButton.interactable = true;
                    _thirdButtonText.text = "Bullet Speed";
                }
                else
                {
                    _thirdUpgradeButton.interactable = false;
                    _thirdButtonText.text = "MAX";
                }

                SetActiveButtons(3);
                SetBackButtonActive(true);
                break;

            case UpgradeMenu.NewGun:
                GameObject[] guns = PrefabManager.Instance.GetGunsByLevel(WorldManager.Instance.HostPlayer.Level);
                if (guns.Length <= 0) return;

                _firstButtonImage.sprite = guns[0].GetComponent<Gun>().UISprite;
                _firstButtonText.text = "";
                _firstUpgradeButton.interactable = true;

                _secondButtonImage.sprite = guns[1].GetComponent<Gun>().UISprite;
                _secondButtonText.text = "";
                _secondUpgradeButton.interactable = true;

                SetActiveButtons(2);
                SetBackButtonActive(false);
                break;
        }
    }

    public void SetActiveButtons(int count)
    {
        GameObject buttonsParent = _firstUpgradeButton.transform.parent.gameObject;
        if (count == 0)
        {
            if (buttonsParent.activeSelf)
                buttonsParent.SetActive(false);
            return;
        }
        else
        {
            if (!buttonsParent.activeSelf)
                buttonsParent.SetActive(true);
        }

        switch (count)
        {
            case 1:
                if (!_firstUpgradeButton.gameObject.activeSelf)
                    _firstUpgradeButton.gameObject.SetActive(true);
                if (_secondUpgradeButton.gameObject.activeSelf)
                    _secondUpgradeButton.gameObject.SetActive(false);
                if (_thirdUpgradeButton.gameObject.activeSelf)
                    _thirdUpgradeButton.gameObject.SetActive(false);
                break;

            case 2:
                if (!_firstUpgradeButton.gameObject.activeSelf)
                    _firstUpgradeButton.gameObject.SetActive(true);
                if (!_secondUpgradeButton.gameObject.activeSelf)
                    _secondUpgradeButton.gameObject.SetActive(true);
                if (_thirdUpgradeButton.gameObject.activeSelf)
                    _thirdUpgradeButton.gameObject.SetActive(false);
                break;

            case 3:
                if (!_firstUpgradeButton.gameObject.activeSelf)
                    _firstUpgradeButton.gameObject.SetActive(true);
                if (!_secondUpgradeButton.gameObject.activeSelf)
                    _secondUpgradeButton.gameObject.SetActive(true);
                if (!_thirdUpgradeButton.gameObject.activeSelf)
                    _thirdUpgradeButton.gameObject.SetActive(true);
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

    private void SetBackButtonActive(bool active)
    {
        if (_backButton.gameObject.activeSelf != active)
            _backButton.gameObject.SetActive(active);
    }
}
