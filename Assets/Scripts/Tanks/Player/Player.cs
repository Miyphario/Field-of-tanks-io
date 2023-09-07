using System;
using UnityEngine;
using sfloat = SafeFloat;
using sint = SafeInt;

public class Player : Tank
{
    public override float MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    public override float Health { get => _health; protected set => _health = value; }
    public override float Speed { get => _speed; protected set => _speed = value; }
    public override float Damage { get => _damage; protected set => _damage = value; }
    public override float FireRate { get => _fireRate; protected set => _fireRate = value; }
    public new PlayerController Controller => base.Controller as PlayerController;

    public override int XP { get => _xp; protected set => _xp = value; }
    public override int MaxXP { get => _maxXp; protected set => _maxXp = value; }
    public override int Level { get => _level; protected set => _level = value; }
    public override int Tier { get => _tier; protected set => _tier = value; }
    
    private sfloat _maxHealth = Constants.DEFAULT_MAX_HEALTH;
    private sfloat _health;
    private sfloat _speed = Constants.DEFAULT_MOVE_SPEED;
    private sfloat _damage = Constants.DEFAULT_DAMAGE;
    private sfloat _fireRate = Constants.DEFAULT_FIRE_RATE;

    private sint _xp;
    private sint _maxXp = Constants.DEFAULT_MAX_XP;
    private sint _level;
    private sint _tier;

    private bool _gunSelected = true;
    private UpgradeMenu _upgradeMenu;
    public event Action<UpgradeMenu> OnMenuSelected;

    protected override void Awake()
    {
        base.Awake();
        OnLevelUp += HandleLevelUp;
        OnTierUpdated += HandleTierUpdated;
    }

    private void HandleLevelUp(int level)
    {
        int lastTierLevel = PrefabManager.Instance.GetLastTierLevel(level);
        if (lastTierLevel > 0 && level > lastTierLevel && !_gunSelected)
        {
            SelectNewRandomGun();
            _upgradeMenu = UpgradeMenu.None;
            _gunSelected = true;
        }

        if (_upgradeMenu == UpgradeMenu.None)
        {
            _upgradeMenu = UpgradeMenu.Base;
            OnMenuSelected?.Invoke(_upgradeMenu);
        }
    }

    private void HandleTierUpdated(int tier)
    {
        _gunSelected = false;
        _upgradeMenu = UpgradeMenu.NewGun;
        OnMenuSelected?.Invoke(_upgradeMenu);
    }

    protected override void DestroyMe()
    {
        GameManager.Instance.RestartScene();
    }

    public void SelectUpgrade(int upgrade)
    {
        if (UpgradeCount <= 0) return;

        switch (_upgradeMenu)
        {
            case UpgradeMenu.Base:
                switch (upgrade)
                {
                    case 1:
                        _upgradeMenu = UpgradeMenu.Tank;
                        break;

                    case 2:
                        _upgradeMenu = UpgradeMenu.Gun;
                        break;
                }

                OnMenuSelected?.Invoke(_upgradeMenu);
                return;

            case UpgradeMenu.Tank:
                switch (upgrade)
                {
                    case 1:
                        Upgrade(UpgradeType.MaxHealth);
                        break;

                    case 2:
                        Upgrade(UpgradeType.Speed);
                        break;
                }
                break;

            case UpgradeMenu.Gun:
                switch (upgrade)
                {
                    case 1:
                        Upgrade(UpgradeType.Damage);
                        break;

                    case 2:
                        Upgrade(UpgradeType.FireRate);
                        break;

                    case 3:
                        Upgrade(UpgradeType.BulletSpeed);
                        break;
                }
                break;

            case UpgradeMenu.NewGun:
                SelectNewGun(upgrade - 1, Tier);
                _gunSelected = true;
                break;
        }

        if (UpgradeCount <= 0)
            _upgradeMenu = UpgradeMenu.None;
        else
            _upgradeMenu = UpgradeMenu.Base;

        OnMenuSelected?.Invoke(_upgradeMenu);
    }

    public void UpgradeMenuBack()
    {
        if (_upgradeMenu == UpgradeMenu.Gun || _upgradeMenu == UpgradeMenu.Tank)
        {
            _upgradeMenu = UpgradeMenu.Base;
            OnMenuSelected?.Invoke(_upgradeMenu);
        }
    }
}
