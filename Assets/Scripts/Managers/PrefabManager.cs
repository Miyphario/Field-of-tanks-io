using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [SerializeField, Header("Guns")] private GameObject[] _tier0;
    public GameObject[] Tier0 => _tier0;
    [SerializeField] private GameObject[] _tier1;
    public GameObject[] Tier1 => _tier1;
    [SerializeField] private GameObject[] _tier2;
    public GameObject[] Tier2 => _tier2;
    [SerializeField] private GameObject[] _tier3;
    public GameObject[] Tier3 => _tier3;
    [SerializeField] private GameObject[] _tier4;
    public GameObject[] Tier4 => _tier4;

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
}
