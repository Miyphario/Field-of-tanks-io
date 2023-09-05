using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _otherInfo;

    public void Initialize()
    {
        WorldManager.Instance.HostPlayer.OnLevelUp += HandlePlayerLevelUp;
        HandlePlayerLevelUp(WorldManager.Instance.HostPlayer.Level);

        WorldManager.Instance.HostPlayer.OnXpUpdated += HandlePlayerXpUpdated;
        HandlePlayerXpUpdated(WorldManager.Instance.HostPlayer.XP, WorldManager.Instance.HostPlayer.MaxXP);

        WorldManager.Instance.HostPlayer.OnUpgrade += HandlePlayerUpgrade;
        WorldManager.Instance.HostPlayer.OnTakeDamage += tank => HandlePlayerUpgrade();
        HandlePlayerUpgrade();
    }

    private void HandlePlayerUpgrade()
    {
        Player pl = WorldManager.Instance.HostPlayer;
        _otherInfo.text = "Health: " + pl.Health + "/" + pl.MaxHealth +
                          "\nDamage: " + pl.Damage + " | Speed: " + pl.Speed +
                          "\nBulSpd: " + pl.BulletSpeed + " | FireRate: " + pl.FireRate;
    }

    private void HandlePlayerXpUpdated(int xp, int maxXp)
    {
        _xpText.text = xp.ToString() + "/" + maxXp.ToString();
    }

    private void HandlePlayerLevelUp(int level)
    {
        _levelText.text = level.ToString();
    }
}
