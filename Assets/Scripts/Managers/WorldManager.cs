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

    [Header("Pools")]
    [SerializeField] private ObjectsPool _enemiesPool;
    public ObjectsPool EnemiesPool => _enemiesPool;
    [SerializeField] private ObjectsPool _bulletsPool;
    public ObjectsPool BulletsPool => _bulletsPool;
    [SerializeField] private ObjectsPool _destructiblesPool;
    public ObjectsPool DestructiblesPool => _destructiblesPool;

    [Header("Bushes")]
    [SerializeField] private GameObject _bushesPrefab;
    [SerializeField] private Transform _bushesParent;
    [SerializeField] private float _maxBushes;

    public void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.OnGameRestarting += () => StopAllCoroutines();
    }

    private void Start()
    {
        HostPlayer = Instantiate(_playerPrefab, GetRandomSpawnPosition(1f), Quaternion.identity).GetComponent<Player>();
        HostPlayer.Initialize(0);
        _tanks.Add(HostPlayer);
        OnPlayerCreated?.Invoke(HostPlayer);

        StartCoroutine(SpawnBushesIE());
        StartCoroutine(EnemySpawningIE());
        StartCoroutine(DestructibleSpawningIE());
    }

    public Vector2 RandomPointAround(in Vector2 point, float distance, float objectSize)
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

    public void ClampMoveInput(in Vector2 position, ref Vector2 direction)
    {
        float objectSize = 1f;

        float maxX = _worldSize.x / 2f - objectSize;
        if ((position.x >= maxX && direction.x > 0f) || (position.x <= -maxX && direction.x < 0f)) direction.x = 0f;

        float maxY = _worldSize.y / 2f - objectSize;
        if ((position.y >= maxY && direction.y > 0f) || (position.y <= -maxY && direction.y < 0f)) direction.y = 0f;
    }

    public Vector2 GetMaxDistanceToCamera(float objectSize)
    {
        return new(_maxDistanceToCamera.x + objectSize, _maxDistanceToCamera.y + objectSize);
    }

    public bool IsFarFromCamera(in Vector2 position, float objectSize)
    {
        return IsFarFromCamera(position, objectSize, _maxDistanceToCamera);
    }

    public bool IsFarFromCamera(in Vector2 position, float objectSize, in Vector2 maxDistance)
    {
        Vector2 camPos = GameManager.Instance.MainCamera.transform.position;
        Vector2 dist = new(Mathf.Abs(position.x - camPos.x), Mathf.Abs(position.y - camPos.y));
        Vector2 maxDist = new(maxDistance.x + objectSize, maxDistance.y + objectSize);
        return dist.x > maxDist.x || dist.y > maxDist.y;
    }

    private Vector2 GetRandomSpawnPosition(float objectSize)
    {
        return new Vector2(Random.Range(-_worldSize.x / 2f + objectSize, _worldSize.x / 2f - objectSize),
                           Random.Range(-_worldSize.y / 2f + objectSize, _worldSize.y / 2f - objectSize));
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
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
                Quaternion spawnRot = GetRandomRotation();

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
                yield return new WaitForSeconds(4f);
                continue;
            }

            while (_destructibles.Count < _maxDestructibles)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject prefab = _destructiblePrefabs[Random.Range(0, _destructiblePrefabs.Count)];
                    Vector2 spawnPos = GetRandomSpawnPosition(prefab.transform.localScale.x);
                    Quaternion spawnRot = GetRandomRotation();

                    Destructible dest = Instantiate(prefab, spawnPos, spawnRot).GetComponent<Destructible>();
                    _destructibles.Add(dest);
                    dest.OnDestroyed += () => _destructibles.Remove(dest);

                    if (_destructibles.Count >= _maxDestructibles) break;
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator SpawnBushesIE()
    {
        int curBush = 0;
        while (curBush < _maxBushes)
        {
            for (int i = 0; i < 4; i++)
            {
                float size = Random.Range(1.75f, 2.2f);
                Vector2 spawnPos = GetRandomSpawnPosition(size);
                Quaternion spawnRot = GetRandomRotation();
                GameObject go = Instantiate(_bushesPrefab, spawnPos, spawnRot, _bushesParent);
                go.transform.localScale = new(size, size, size);
                go.SetActive(true);
                curBush++;

                if (curBush >= _maxBushes) yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, _worldSize);
    }
}
