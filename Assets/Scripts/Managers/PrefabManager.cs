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
        if (level <= 0 || level % 3 != 0 ||
            _gunTier.Length < level / 3) return -1;

        return level / 3;
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
}
