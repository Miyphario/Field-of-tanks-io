using System.Collections;
using UnityEngine;

public class Enemy : Tank
{
    protected override void Awake()
    {
        base.Awake();

        OnLevelUp += LevelUp;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckDistanceIE());
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

    private void LevelUp(int level)
    {
        if (CanCreateNewGun(level))
        {
            SelectNewRandomGun();
        }
        else
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
}
