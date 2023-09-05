using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tank : MonoBehaviour
{
    public virtual float MaxHealth { get; protected set; } = 100f;
    public virtual float Health { get; protected set; }
    public event Action<Tank> OnTakeDamage;

    public virtual float Speed { get; protected set; } = 6f;
    public virtual float BulletSpeed { get; protected set; } = 1f;
    public virtual float Damage { get; protected set; } = 10f;
    public virtual float FireRate { get; protected set; } = 1f;

    public TankController Controller { get; protected set; }

    [SerializeField] protected Gun _gun;
    public Gun Gun => _gun;

    private int _teamID;
    public int TeamID => _teamID;
    public event Action OnDestroyed;

    public virtual int XP { get; protected set; }
    public virtual int MaxXP { get; protected set; } = Constants.DEFAULT_MAX_XP;
    public virtual int Level { get; protected set; }
    public int RewardXP => Level * 3;
    private int _upgradeCount;
    public int UpgradeCount => _upgradeCount;
    public event Action<int> OnLevelUp;

    protected BarUI _healthbar;

    protected virtual void Awake()
    {
        Controller = GetComponent<TankController>();
        Health = MaxHealth;
    }

    protected virtual void Start()
    {
        _healthbar = HUDManager.Instance.CreateHealthbar();
        bool enableBar = this switch
        {
            Player => true,
            _ => false
        };
        _healthbar.Initialize(gameObject, 0f, MaxHealth, enableBar);
    }

    public void Initialize(int teamID)
    {
        _teamID = teamID;
    }

    public bool TakeDamage(float damage, Tank attacker)
    {
        if (Health - damage > 0f)
        {
            Health = Mathf.Clamp(Health - damage, 0f, MaxHealth);
            _healthbar.SetValue(Health);
            if (attacker != null)
                OnTakeDamage?.Invoke(attacker);
        }
        else
        {
            Health = 0f;
            OnDestroyed?.Invoke();
            _healthbar.Dispose();
            if (attacker != null)
                attacker.TakeDamage(-(MaxHealth / 3.5f));
            DestroyMe();
            return true;
        }

        return false;
    }

    public bool TakeDamage(float damage)
    {
        return TakeDamage(damage, null);
    }

    protected virtual void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void AddXP(int xp)
    {
        int curXp = XP + xp - MaxXP;
        if (curXp >= 0)
        {
            XP = 0;
            MaxXP += Constants.MAX_XP_MULTIPLIER;
            Level++;
            OnLevelUp?.Invoke(Level);
            _upgradeCount++;
            TakeDamage(-MaxHealth);

            if (curXp > 0)
                AddXP(curXp);
        }
        else
        {
            XP += xp;
        }
    }

    public void Upgrade(UpgradeType upgradeType)
    {
        if (_upgradeCount <= 0) return;

        switch (upgradeType)
        {
            case UpgradeType.Damage:
                Damage += Constants.DAMAGE_PER_LEVEL;
                break;

            case UpgradeType.FireRate:
                FireRate -= Constants.FIRE_RATE_PER_LEVEL;
                break;

            case UpgradeType.BulletSpeed:
                BulletSpeed += Constants.BULLET_SPEED_PER_LEVEL;
                break;

            case UpgradeType.MaxHealth:
                float healthPerc = Health / MaxHealth;
                MaxHealth += Constants.MAX_HEALTH_PER_LEVEL;
                Health = healthPerc * MaxHealth;
                _healthbar.SetMaxValue(MaxHealth);
                _healthbar.SetValue(Health);
                break;

            case UpgradeType.Speed:
                Speed += Constants.SPEED_PER_LEVEL;
                break;
        }

        _upgradeCount--;
    }

    public void Upgrade(Gun newGun)
    {
        if (_upgradeCount <= 0) return;

        bool isShooting = _gun.IsShooting;
        Gun g = Instantiate(newGun.gameObject, transform).GetComponent<Gun>();
        Destroy(_gun.gameObject);
        _gun = g;
        _upgradeCount--;

        if (isShooting)
            _gun.ShootStart();
    }

    public void SelectNewGun(int gunIndex, int level)
    {
        GameObject[] guns = PrefabManager.Instance.GetGunsByLevel(level);
        if (guns.Length <= 0) return;

        Upgrade(guns[gunIndex].GetComponent<Gun>());
    }

    public void SelectNewRandomGun()
    {
        SelectNewRandomGun(Level);
    }

    public void SelectNewRandomGun(int level)
    {
        GameObject[] guns = PrefabManager.Instance.GetGunsByLevel(level);
        if (guns.Length <= 0) return;

        int index = Random.Range(0, guns.Length);
        SelectNewGun(index, level);
    }

    public static bool CanCreateNewGun(int level)
    {
        if (level <= 0) return false;
        return level % 3 == 0;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Health = MaxHealth;
    }
#endif
}
