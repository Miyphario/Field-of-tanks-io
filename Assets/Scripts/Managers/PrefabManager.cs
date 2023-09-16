using System;
using UnityEngine;

[Serializable]
public class GunTier
{
    [SerializeField] private GameObject[] _guns;
    public GameObject[] Guns => _guns;
}

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [SerializeField] private GunTier[] _gunTier;
    public Gun DefaultGun => _gunTier[0].Guns[0].GetComponent<Gun>();

    [Header ("Particles")]
    [SerializeField] private GameObject _tankExplodeEffect;
    [SerializeField] private GameObject _shootEffect;

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public int GetCurrentTier(int level)
    {
        if (level <= 0 || level % 3 != 0) return -1;

        float tier = level / 3f;
        float curTier = (float)(tier - Math.Truncate(tier));
        if (curTier != 0f) return -1;
        if (_gunTier.Length < tier) return -1;

        return Mathf.FloorToInt(tier);
    }

    public int GetLastTier(int level)
    {
        float tier = level / 3f;
        return Mathf.Clamp((int)tier, 0, _gunTier.Length - 1);
    }

    public int GetLastTierLevel(int level)
    {
        int tier = GetLastTier(level);
        return tier * 3;
    }

    public GameObject[] GetGunsByLevel(int level)
    {
        if (level <= 0 || level % 3 != 0) return default;

        int index = GetCurrentTier(level);
        if (index >= 0)
            return _gunTier[index].Guns;
        else
            return default;
    }

    public GameObject[] GetGunsByTier(int tier)
    {
        if (tier >= _gunTier.Length || tier < 0) return default;

        return _gunTier[tier].Guns;
    }

    public void CreateParticles(ParticlesType particles, Vector2 position, Quaternion rotation, Transform parent)
    {
        GameObject part = null;
        switch (particles)
        {
            case ParticlesType.TankExplode:
                part = _tankExplodeEffect;
                break;

            case ParticlesType.Shoot:
                part = _shootEffect;
                break;
        }

        if (part == null) return;
        Instantiate(part, position, rotation, parent);
    }

    public void CreateParticles(ParticlesType particles, Vector2 position, Quaternion rotation)
    {
        CreateParticles(particles, position, rotation, null);
    }
}
