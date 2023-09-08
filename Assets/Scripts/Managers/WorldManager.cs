using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    [SerializeField] private Vector2 _maxDistanceToCamera;

    [SerializeField] private GameObject _playerPrefab;
    public event Action<Player> OnPlayerCreated;

    public Player HostPlayer { get; private set; }

    [SerializeField] private Vector2 _worldSize;
    public Vector2 WorldSize => _worldSize;

    private List<Tank> _tanks = new();
    public IReadOnlyList<Tank> Tanks => _tanks;
    [SerializeField] private int _maxTanks = 39;

    private List<Destructible> _destructibles = new();
    public IReadOnlyList<Destructible> Destructibles => _destructibles;
    [SerializeField] private int _maxDestructibles = 30;
    [SerializeField] private List<GameObject> _destructiblePrefabs;

    private int _curTeamID = 0;
    public Gamemode Gamemode { get; private set; } = Gamemode.Deathmath;

    [SerializeField, Header("Pools")]
    private ObjectsPool _enemiesPool;
    public ObjectsPool EnemiesPool => _enemiesPool;
    [SerializeField] private ObjectsPool _bulletsPool;
    public ObjectsPool BulletsPool => _bulletsPool;
    [SerializeField] private ObjectsPool _destructiblesPool;
    public ObjectsPool DestructiblesPool => _destructiblesPool;

    public void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        HostPlayer = Instantiate(_playerPrefab, GetRandomSpawnPosition(1f), Quaternion.identity).GetComponent<Player>();
        HostPlayer.Initialize(0);
        _tanks.Add(HostPlayer);
        OnPlayerCreated?.Invoke(HostPlayer);

        StartCoroutine(EnemySpawningIE());
        StartCoroutine(DestructibleSpawningIE());
    }

    //public void StartHost()
    //{
    //    NetworkManager.Singleton.StartHost();

    //    HostPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
    //}

    //public void StartClient()
    //{
    //    NetworkManager.Singleton.StartClient();

    //    HostPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
    //}

    //public void StartServer()
    //{
    //    NetworkManager.Singleton.StartServer();
    //}

    public Vector2 RandomPointAround(Vector2 point, float distance, float objectSize)
    {
        Vector2 pos = new(
            Random.Range(-distance, distance),
            Random.Range(-distance, distance));
        pos += point;

        float maxX = _worldSize.x / 2f - objectSize;
        if (pos.x > maxX) pos.x = maxX;
        else if (pos.x < -maxX) pos.x = -maxX;

        float maxY = _worldSize.y / 2f - objectSize;
        if (pos.y > maxY) pos.y = maxY;
        else if (pos.y < -maxY) pos.y = -maxY;

        return pos;
    }

    public Vector2 ClampMoveInput(Vector2 position, Vector2 direction)
    {
        float objectSize = 1f;

        float maxX = _worldSize.x / 2f - objectSize;
        if ((position.x >= maxX && direction.x > 0f) || (position.x <= -maxX && direction.x < 0f)) direction.x = 0f;

        float maxY = _worldSize.y / 2f - objectSize;
        if ((position.y >= maxY && direction.y > 0f) || (position.y <= -maxY && direction.y < 0f)) direction.y = 0f;

        return direction;
    }

    public Vector2 GetMaxDistanceToCamera(float objectSize)
    {
        return new(_maxDistanceToCamera.x + objectSize, _maxDistanceToCamera.y + objectSize);
    }

    public bool IsFarFromCamera(Vector2 position, float objectSize)
    {
        Vector2 camPos = GameManager.Instance.MainCamera.transform.position;
        Vector2 dist = new(Mathf.Abs(position.x - camPos.x), Mathf.Abs(position.y - camPos.y));
        Vector2 maxDist = GetMaxDistanceToCamera(objectSize);
        return dist.x > maxDist.x || dist.y > maxDist.y;
    }

    private Vector2 GetRandomSpawnPosition(float objectSize)
    {
        return new Vector2(Random.Range(-_worldSize.x / 2f + objectSize, _worldSize.x / 2f - objectSize),
                           Random.Range(-_worldSize.y / 2f + objectSize, _worldSize.y / 2f - objectSize));
    }

    private IEnumerator EnemySpawningIE()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (_tanks.Count >= _maxTanks)
            {
                yield return new WaitForSeconds(3f);
                continue;
            }

            for (int i = _tanks.Count; i < _maxTanks; i++)
            {
                Vector2 spawnPos = GetRandomSpawnPosition(1f);
                Quaternion spawnRot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                _curTeamID++;
                Enemy tank = EnemiesPool.GetFromPool(spawnPos, spawnRot).GetComponent<Enemy>();
                _tanks.Add(tank);
                int teamId = Gamemode switch
                {
                    Gamemode.Teammatch => Random.Range(0, 2),
                    _ => _curTeamID
                };

                int startLevel = 0;
                if (Vector2.Distance(spawnPos, HostPlayer.transform.position) >= 30)
                {
                    startLevel = Random.Range(0, HostPlayer.Level + 6);
                }
                tank.Initialize(teamId);
                tank.AddLevel(startLevel);
                tank.OnDestroyed += () => _tanks.Remove(tank);

                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
            }
        }
    }

    private IEnumerator DestructibleSpawningIE()
    {
        while (true)
        {
            if (_destructibles.Count >= _maxDestructibles)
            {
                yield return new WaitForSeconds(3f);
                continue;
            }

            for (int i = _destructibles.Count; i < _maxDestructibles; i++)
            {
                Vector2 spawnPos = GetRandomSpawnPosition(0.8f);
                Quaternion spawnRot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                GameObject prefab = _destructiblePrefabs[Random.Range(0, _destructiblePrefabs.Count)];
                Destructible dest = Instantiate(prefab, spawnPos, spawnRot).GetComponent<Destructible>();
                _destructibles.Add(dest);
                dest.OnDestroyed += () => _destructibles.Remove(dest);

                yield return new WaitForSeconds(Random.Range(1f, 1.5f));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, _worldSize);
    }
}
