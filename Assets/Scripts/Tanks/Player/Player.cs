using System;
using sfloat = SafeFloat;
using sint = SafeInt;

public class Player : Tank
{
    public override float MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    public override float Health { get => _health; protected set => _health = value; }
    public override float Speed { get => _speed; protected set => _speed = value; }
    public override float Damage { get => _damage; protected set => _damage = value; }
    public override float TouchDamage { get => _touchDamage; protected set => _touchDamage = value; }
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
    private sfloat _touchDamage = Constants.DEFAULT_TOUCH_DAMAGE;

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

    public new void Initialize(int teamID)
    {
        base.Initialize(teamID);
        _healthbar.Enable();
        gameObject.EnableAll();
        Controller.Initialize();
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

    protected override void DestroySelf()
    {
        gameObject.Toggle(false);
        ResetToDefault();
    }

    public void SelectUpgrade(int upgrade)
    {
        if (UpgradeCount <= 0 || Controller.DisabledInput) return;

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
                        if (!CanUpgrade(UpgradeType.MaxHealth)) return;
                        Upgrade(UpgradeType.MaxHealth);
                        break;

                    case 2:
                        if (!CanUpgrade(UpgradeType.Speed)) return;
                        Upgrade(UpgradeType.Speed);
                        break;
                    
                    case 3:
                        if (!CanUpgrade(UpgradeType.TouchDamage)) return;
                        Upgrade(UpgradeType.TouchDamage);
                        break;
                }
                break;

            case UpgradeMenu.Gun:
                switch (upgrade)
                {
                    case 1:
                        if (!CanUpgrade(UpgradeType.Damage)) return;
                        Upgrade(UpgradeType.Damage);
                        break;

                    case 2:
                        if (!CanUpgrade(UpgradeType.FireRate)) return;
                        Upgrade(UpgradeType.FireRate);
                        break;

                    case 3:
                        if (!CanUpgrade(UpgradeType.BulletSpeed)) return;
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
        if (Controller.DisabledInput) return;

        if (_upgradeMenu == UpgradeMenu.Gun || _upgradeMenu == UpgradeMenu.Tank)
        {
            _upgradeMenu = UpgradeMenu.Base;
            OnMenuSelected?.Invoke(_upgradeMenu);
        }
    }
}
