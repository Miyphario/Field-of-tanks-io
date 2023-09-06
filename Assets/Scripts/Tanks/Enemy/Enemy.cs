using System.Collections;
using UnityEngine;

public class Enemy : Tank
{
    public new EnemyController Controller => base.Controller as EnemyController;

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
        Initialize(teamID);
        Controller.Initialize();
        AddLevel(startLevel);
        StartCoroutine(CheckDistanceIE());
    }

    protected override void DestroyMe()
    {
        StopAllCoroutines();
        WorldManager.Instance.EnemiesPool.AddToPool(gameObject);
        ResetToDefault();
    }

    private IEnumerator CheckDistanceIE()
    {
        while (true)
        {
            float dist = Vector2.Distance(transform.position, WorldManager.Instance.HostPlayer.transform.position);
            if (dist > HUDManager.Instance.HealthbarRenderDistance)
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

            /* if (CanCreateNewGun(level))
            {
                SelectNewRandomGun();
            }
            else
            {
                
            } */
        }
    }
}
