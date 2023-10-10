using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Tank, IPoolable
{
    public new EnemyController Controller => base.Controller as EnemyController;

    private bool _isAlive = true;

    public event Action OnAddedToPool;

    public bool IsAlive => _isAlive;

    protected override void Awake()
    {
        base.Awake();
        OnLevelUp += HandleLevelUp;
        OnTierUpdated += HandleTierUpdated;
        OnInvisible += HandleInvisible;
    }

    protected override void Start()
    {
        base.Start();
    }

    public new void Initialize(int teamID)
    {
        _isAlive = true;
        base.Initialize(teamID);
        Controller.Initialize();
        StartCoroutine(CheckDistanceIE());
    }

    protected override void DestroySelf() => AddToPool();

    private IEnumerator CheckDistanceIE()
    {
        while (true)
        {
            if (!IsInvisible)
            {
                if (WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x))
                {
                    _healthbar.Disable();
                }
                else
                {
                    _healthbar.Enable();
                }
            }

            yield return new WaitForSeconds(Constants.HEALTHBAR_RENDER_TIME);
        }
    }

    private void HandleTierUpdated(int tier)
    {
        SelectNewRandomGun();
    }

    private void HandleLevelUp(int level)
    {
        UpgradeType[] otherUpgrades = new UpgradeType[]
        {
            UpgradeType.Speed,
            UpgradeType.BulletSpeed
        };

        while (UpgradeCount > 0)
        {
            float rand = Random.Range(0f, 100f);
            switch (rand)
            {
                case var _ when rand <= 15f:
                    Upgrade(UpgradeType.Damage);
                    break;
                
                case var _ when rand <= 30f:
                    Upgrade(UpgradeType.TouchDamage);
                    break;

                case var _ when rand <= 45f:
                    Upgrade(UpgradeType.MaxHealth);
                    break;

                case var _ when rand <= 60f:
                    Upgrade(UpgradeType.FireRate);
                    break;

                default:
                    int r = Random.Range(0, otherUpgrades.Length);
                    Upgrade(otherUpgrades[r]);
                    break;
            }
        }
    }

    public void AddToPool()
    {
        WorldManager.Instance.EnemiesPool.AddToPool(this);
        gameObject.EnableAll();
        ResetToDefault();
        _isAlive = false;
        OnAddedToPool?.Invoke();
    }

    private void HandleInvisible(bool invisible)
    {
        if (invisible)
        {
            _healthbar.Disable();
        }
        else
        {
            if (!WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x))
            {
                _healthbar.Enable();
            }
        }
    }
}
