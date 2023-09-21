using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Coroutine _enemiesSpawnRoutine;
    [SerializeField] private ObjectsPool _bulletsPool;
    public ObjectsPool BulletsPool => _bulletsPool;
    [SerializeField] private ObjectsPool _destructiblesPool;
    public ObjectsPool DestructiblesPool => _destructiblesPool;

    [Header("Bushes")]
    [SerializeField] private GameObject _bushesPrefab;
    [SerializeField] private Transform _bushesParent;
    [SerializeField] private float _maxBushes;

    private bool _notReadyToSpawn;
    public bool IsReady => !_notReadyToSpawn;
    private bool _isPlaying;
    private float _destroyTime = 0.05f;
    private int _destroyObjectsPerCycle = 15;
    public bool IsPlaying => _isPlaying;
    public event Action<bool> OnReadyToSpawn;
    public event Action OnGameStarted;
    public event Action OnGameEnded;
    private float _timeToSaveGame = -1f;
    private bool _gameSaving;

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
        GameManager.Instance.LoadGame();

        StartCoroutine(BushesSpawnIE());
        StartCoroutine(DestructibleSpawnIE());
        _enemiesSpawnRoutine = StartCoroutine(EnemySpawnIE());
    }
    public bool StartGame()
    {
        if (_isPlaying || _notReadyToSpawn) return false;

        _curTeamID = 0;
        if (HostPlayer == null)
        {
            HostPlayer = Instantiate(_playerPrefab).GetComponent<Player>();
            HostPlayer.OnDestroyed += () => 
            {
                _tanks.Remove(HostPlayer);
                _isPlaying = false;
                AdsManager.ShowAds();
                GameManager.Instance.SaveGame();
                Restart();
                OnGameEnded?.Invoke();
            };
        }

        HostPlayer.transform.SetPositionAndRotation(GetRandomSpawnPosition(_playerPrefab.transform.localScale.x),
                                                    GetRandomRotation());
        HostPlayer.Initialize(_curTeamID);
        _curTeamID++;
        _tanks.Add(HostPlayer);
        HostPlayer.gameObject.Toggle(true);
        
        OnPlayerCreated?.Invoke(HostPlayer);
        OnGameStarted?.Invoke();

        _enemiesSpawnRoutine = StartCoroutine(EnemySpawnIE());
        _isPlaying = true;
        return _isPlaying;
    }

    public void SaveGame(float delay)
    {
        _gameSaving = true;
        _timeToSaveGame = delay;
    }

    public void LoadGame(string json)
    {
        GameManager.Instance.LoadGame(json);
    }

    private void Update()
    {
        if (_gameSaving)
        {
            if (_timeToSaveGame > 0f)
            {
                _timeToSaveGame -= Time.deltaTime;
            }
            else
            {
                _gameSaving = false;
                GameManager.Instance.SaveGame();
            }
        }
    }

    public void Restart()
    {
        if (_notReadyToSpawn) return;

        if (!_notReadyToSpawn)
            OnReadyToSpawn?.Invoke(!_notReadyToSpawn);
        _notReadyToSpawn = true;
        StopAllCoroutines();

        StartCoroutine(DestroyAllIE(() =>
        {
            if (_notReadyToSpawn)
                OnReadyToSpawn?.Invoke(!_notReadyToSpawn);
            _notReadyToSpawn = false;
            StartCoroutine(BushesSpawnIE());
            StartCoroutine(DestructibleSpawnIE());
            _enemiesSpawnRoutine = StartCoroutine(EnemySpawnIE());
        }));
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

    private IEnumerator EnemySpawnIE()
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

                Enemy tank = EnemiesPool.GetFromPool(spawnPos, spawnRot).GetComponent<Enemy>();
                _tanks.Add(tank);
                int teamId = Gamemode switch
                {
                    Gamemode.Teammatch => Random.Range(0, 2),
                    _ => _curTeamID
                };

                int startLevel = 0;
                if (Vector2.Distance(spawnPos, GameManager.Instance.MainCamera.transform.position) >= 30)
                {
                    int maxLvl;
                    if (HostPlayer != null && _isPlaying)
                        maxLvl = HostPlayer.Level + 6;
                    else
                        maxLvl = 30;
                    startLevel = Random.Range(0, maxLvl);
                }
                if (tank.TeamID <= 0)
                    tank.OnDestroyed += () => _tanks.Remove(tank);

                tank.Initialize(teamId);
                _curTeamID++;
                tank.AddLevel(startLevel);

                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
            }
        }
    }

    private IEnumerator DestructibleSpawnIE()
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

    private IEnumerator DestroyDestructiblesIE()
    {
        while (_destructibles.Count > 0)
        {
            for (int i = 0; i < _destroyObjectsPerCycle; i++)
            {
                _destructibles[^1].TakeDamage(_destructibles[^1].Health);
                if (_destructibles.Count <= 0) break;
            }

            yield return new WaitForSeconds(_destroyTime);
        }
    }

    private IEnumerator BushesSpawnIE()
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
                go.Toggle(true);
                curBush++;

                if (curBush >= _maxBushes) yield break;
            }

            yield return new WaitForSeconds(_destroyTime);
        }
    }

    private IEnumerator DestroyBushesIE()
    {
        while (_bushesParent.childCount > 1)
        {
            for (int i = 0; i < _destroyObjectsPerCycle; i++)
            {
                Destroy(_bushesParent.GetChild(_bushesParent.childCount - 1).gameObject);
                if (_bushesParent.childCount <= 1) break;
            }

            yield return new WaitForSeconds(_destroyTime);
        }
    }

    private IEnumerator DestroyAllIE(Action actionAfterDestroy)
    {
        yield return EnemiesPool.ClearIE(_destroyObjectsPerCycle, _destroyTime);
        yield return BulletsPool.ClearIE(_destroyObjectsPerCycle, _destroyTime);
        yield return DestroyDestructiblesIE();
        yield return DestroyBushesIE();

        actionAfterDestroy?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, _worldSize);
    }
}
