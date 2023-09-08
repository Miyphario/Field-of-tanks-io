using System.Collections;
using UnityEngine;

public class Enemy : Tank, IPoolable
{
    public new EnemyController Controller => base.Controller as EnemyController;

    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

    protected override void Awake()
    {
        base.Awake();
        OnLevelUp += HandleLevelUp;
        OnTierUpdated += HandleTierUpdated;
    }

    protected override void Start()
    {
        base.Start();
    }

    public void Initialize(int teamID, int startLevel)
    {
        _isAlive = true;
        Initialize(teamID);
        Controller.Initialize();
        AddLevel(startLevel);
        StartCoroutine(CheckDistanceIE());
    }

    protected override void DestroyMe() => AddToPool();

    private IEnumerator CheckDistanceIE()
    {
        while (true)
        {
            if (WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x))
            {
                _healthbar.Disable();
            }
            else
            {
                _healthbar.Enable();
            }

            yield return new WaitForSeconds(HUDManager.HEALTHBAR_RENDER_TIME);
        }
    }

    private void HandleTierUpdated(int tier)
    {
        SelectNewRandomGun();
    }

    private void HandleLevelUp(int level)
    {
        while (UpgradeCount > 0)
        {
            float rand = Random.Range(0f, 100f);
            switch (rand)
            {
                case var _ when rand <= 60f:
                    Upgrade(UpgradeType.Speed);
                    break;

                case var _ when rand <= 35f:
                    Upgrade(UpgradeType.BulletSpeed);
                    break;

                case var _ when rand <= 20f:
                    Upgrade(UpgradeType.FireRate);
                    break;

                default:
                    Upgrade(UpgradeType.Damage);
                    break;
            }
        }
    }

    public void AddToPool()
    {
        StopAllCoroutines();
        WorldManager.Instance.EnemiesPool.AddToPool(gameObject);
        ResetToDefault();
        _isAlive = false;
    }
}
