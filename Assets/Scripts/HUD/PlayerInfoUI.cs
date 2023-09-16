using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _otherInfo;

    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

    public void Initialize()
    {
        WorldManager.Instance.OnPlayerCreated += player =>
        {
            player.OnUpgrade += HandlePlayerUpgrade;
            player.OnLevelUp += HandlePlayerLevelUp;
            player.OnXpUpdated += HandlePlayerXpUpdated;
            player.OnTakeDamage += tank => HandlePlayerUpgrade();

            HandlePlayerUpgrade();
            HandlePlayerLevelUp(player.Level);
            HandlePlayerXpUpdated(player.XP, player.MaxXP);
        };
    }

    private void HandlePlayerUpgrade()
    {
        Player pl = WorldManager.Instance.HostPlayer;
        _otherInfo.text = "HP: " + pl.Health + "/" + pl.MaxHealth +
                          "\nDmg: " + pl.Damage + " | Touch: " + pl.TouchDamage + 
                          "\nSpd: " + pl.Speed + " | Bull: " + pl.BulletSpeed +
                          "\nFrRate: " + pl.FireRate;
    }

    private void HandlePlayerXpUpdated(int xp, int maxXp)
    {
        _xpText.text = "XP: " + xp + "/" + maxXp;
    }

    private void HandlePlayerLevelUp(int level)
    {
        _levelText.text = "Level: " + level;
    }
}
