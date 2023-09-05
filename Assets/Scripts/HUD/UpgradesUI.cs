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
        WorldManager.Instance.HostPlayer.OnMenuSelected += HandlePlayerMenuSelected;

        _firstButtonText = _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
        _secondButtonText = _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
        _thirdButtonText = _thirdUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();

        _firstUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(1));
        _secondUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(2));
        _thirdUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(3));

        SetActiveButtons(0);
    }

    private void HandlePlayerMenuSelected(UpgradeMenu menu)
    {
        switch (menu)
        {
            case UpgradeMenu.None:
                SetActiveButtons(0);
                break;

            case UpgradeMenu.Base:
                _firstButtonImage.sprite = _tankSprite;
                _firstButtonText.text = "Tank";

                _secondButtonImage.sprite = _gunSprite;
                _secondButtonText.text = "Gun";

                SetActiveButtons(2);
                break;

            case UpgradeMenu.Tank:
                _firstButtonImage.sprite = _healthSprite;
                _firstButtonText.text = "Health";

                _secondButtonImage.sprite = _speedSprite;
                _secondButtonText.text = "Speed";

                SetActiveButtons(2);
                break;

            case UpgradeMenu.Gun:
                _firstButtonImage.sprite = _damageSprite;
                _firstButtonText.text = "Damage";

                _secondButtonImage.sprite = _fireRateSprite;
                _secondButtonText.text = "Fire Rate";

                _thirdButtonImage.sprite = _bulletSpeedSprite;
                _thirdButtonText.text = "Bullet Speed";

                SetActiveButtons(3);
                break;

            case UpgradeMenu.NewGun:
                GameObject[] guns = PrefabManager.Instance.GetGunsByLevel(WorldManager.Instance.HostPlayer.Level);
                if (guns.Length <= 0) return;

                _firstButtonImage.sprite = guns[0].GetComponent<Gun>().UISprite;
                _firstButtonText.text = "";

                _secondButtonImage.sprite = guns[1].GetComponent<Gun>().UISprite;
                _secondButtonText.text = "";

                SetActiveButtons(2);
                break;
        }
    }

    public void SetActiveButtons(int count)
    {
        switch (count)
        {
            case 0:
                if (_firstUpgradeButton.gameObject.activeSelf)
                    _firstUpgradeButton.gameObject.SetActive(false);
                if (_secondUpgradeButton.gameObject.activeSelf)
                    _secondUpgradeButton.gameObject.SetActive(false);
                if (_thirdUpgradeButton.gameObject.activeSelf)
                    _thirdUpgradeButton.gameObject.SetActive(false);
                break;

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
}
