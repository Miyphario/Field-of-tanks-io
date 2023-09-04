using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerStart;

    public Player HostPlayer { get; private set; }

    [SerializeField] private Vector2 _worldSize;
    public Vector2 WorldSize => _worldSize;
    [SerializeField] private GameObject _enemyPrefab;

    private List<Tank> _tanks = new();
    public IReadOnlyList<Tank> Tanks => _tanks;
    [SerializeField] private int _maxTanks = 39;

    private List<Destructible> _destructibles = new();
    public IReadOnlyList<Destructible> Destructibles => _destructibles;
    [SerializeField] private int _maxDestructibles = 30;
    [SerializeField] private List<GameObject> _destructiblePrefabs;

    private int _curTeamID = 0;
    public Gamemode Gamemode { get; private set; } = Gamemode.Deathmath;

    public void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        HostPlayer = Instantiate(_playerPrefab, GetRandomSpawnPosition(1f), Quaternion.identity).GetComponent<Player>();
        _tanks.Add(HostPlayer);

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
                Tank tank = Instantiate(_enemyPrefab, spawnPos, spawnRot).GetComponent<Tank>();
                _tanks.Add(tank);
                int teamId = Gamemode switch
                {
                    Gamemode.Teammatch => Random.Range(0, 2),
                    _ => _curTeamID
                };
                tank.Initialize(teamId);
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
