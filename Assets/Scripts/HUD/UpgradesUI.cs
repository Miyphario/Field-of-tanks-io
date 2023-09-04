using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private Button _firstUpgradeButton;
    [SerializeField] private Button _secondUpgradeButton;
    [SerializeField] private Button _thirdUpgradeButton;

    public void Initialize()
    {
        WorldManager.Instance.HostPlayer.OnLevelUp += Player_OnLevelUp;
        WorldManager.Instance.HostPlayer.OnMenuSelected += Player_OnMenuSelected;

        _firstUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(1));
        _secondUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(2));
        _thirdUpgradeButton.onClick.AddListener(() => WorldManager.Instance.HostPlayer.SelectUpgrade(3));

        SetActiveButtons(0);
    }

    private void Player_OnMenuSelected(UpgradeMenu menu)
    {
        switch (menu)
        {
            case UpgradeMenu.None:
                SetActiveButtons(0);
                break;

            case UpgradeMenu.Base:
                _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tank";
                _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Gun";

                SetActiveButtons(2);
                break;

            case UpgradeMenu.Tank:
                _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max Health";
                _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Speed";

                SetActiveButtons(2);
                break;

            case UpgradeMenu.Gun:
                _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Damage";
                _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Fire Rate";
                _thirdUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Bullet Speed";

                SetActiveButtons(3);
                break;

            case UpgradeMenu.NewGun:
                _firstUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Dual Gun";
                _secondUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Null";

                SetActiveButtons(2);
                break;
        }
    }

    private void Player_OnLevelUp(int level)
    {

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
    }
}
