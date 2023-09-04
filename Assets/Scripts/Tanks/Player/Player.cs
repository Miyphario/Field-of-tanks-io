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

    private sfloat _maxHealth = 100f;
    private sfloat _health;
    private sfloat _speed = 6f;
    private sfloat _damage = 10f;
    private sfloat _fireRate = 1f;

    private sint _xp;
    private sint _maxXp = Constants.DEFAULT_MAX_XP;
    private sint _level = 1;

    private UpgradeMenu _upgradeMenu;
    public event Action<UpgradeMenu> OnMenuSelected;

    protected override void Awake()
    {
        base.Awake();

        OnLevelUp += Player_OnLevelUp;
    }

    private void Player_OnLevelUp(int level)
    {
        Debug.Log("Cur Level: " + Level);

        UpgradeMenu lastMenu = _upgradeMenu;

        if (CanCreateNewGun(Level - 1))
        {
            SelectNewRandomGun(Level - 1);
        }

        if (CanCreateNewGun(Level))
        {
            _upgradeMenu = UpgradeMenu.NewGun;
        }
        else
        {
            _upgradeMenu = UpgradeMenu.Base;
        }
        
        

        if (_upgradeMenu != lastMenu)
            OnMenuSelected?.Invoke(_upgradeMenu);
    }

    protected override void DestroyMe()
    {
        //GameManager.Instance.RestartScene();
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
                SelectNewGun(upgrade - 1, Level);
                break;
        }

        if (UpgradeCount <= 0)
            _upgradeMenu = UpgradeMenu.None;
        else
            _upgradeMenu = UpgradeMenu.Base;

        OnMenuSelected?.Invoke(_upgradeMenu);
    }
}
