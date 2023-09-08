using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : TankController
{
    private float _accuracy;

    private Tank _target;
    private float _distanceToTarget;
    public event Action<Tank> OnTargetChanged;
    private Coroutine _logicRoutine;

    private Vector2 _lookDirection;
    private Vector2 _moveDirection;
    private bool _isMoving;
    private float _minRotationSpeed = 100f;
    private float _maxRotationSpeed = 300f;

    private float RotationSpeed
    {
        get
        {
            if (_target == null)
                return 700f;
            else
            {
                float distancePercent = Mathf.Clamp(100f - (100f * _distanceToTarget / (_detectEnemyDistance - 1f)), 0f, 100f);
                return Mathf.Clamp(((_maxRotationSpeed - _minRotationSpeed) * distancePercent / 100f) + _minRotationSpeed, _minRotationSpeed, _maxRotationSpeed);
            }
        }
    }

    private VectorDirection _flexDirection;
    private float _flexTime;
    private bool _isFlexing;

    private float _detectEnemyDistance = 12f;
    private float _curDetectEnemyDistance = 12f;
    private float MaxEnemyDistance => _curDetectEnemyDistance + 3f;
    private Vector2 _moveDistance;
    private Vector2 _shootDistance;

    private AIState _state = AIState.Idle;
    private float _retreatHealthPercent = 20f;
    public bool IsRetreat
    {
        get
        {
            float curHealthPerc = Tank.Health / Tank.MaxHealth * 100f;
            return curHealthPerc <= _retreatHealthPercent;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        OnTargetChanged += tar =>
        {
            StartLogic();

            if (tar == null)
            {
                _curDetectEnemyDistance = _detectEnemyDistance;
                StopFlex();
            }
        };

        Tank.OnTakeDamage += tar =>
        {
            if (_target == null)
            {
                _curDetectEnemyDistance = DistanceToPoint(tar.transform.position) + 3f;
                _target = tar;
                OnTargetChanged?.Invoke(_target);
            }
        };

        Tank.OnDestroyed += () =>
        {
            DisabledInput = true;
            StopAllCoroutines();
            StopFlex();
            _moveDirection = Vector2.zero;
            _lookDirection = Vector2.zero;
            _rb.velocity = Vector2.zero;
            _target = null;
        };
    }

    public void Initialize()
    {
        _accuracy = Random.Range(0.1f, 3f);
        _minRotationSpeed = Random.Range(100f, 200f);
        _maxRotationSpeed = Random.Range(200f, 300f) + _minRotationSpeed;

        _retreatHealthPercent = Random.Range(10f, 35f);

        _detectEnemyDistance = Random.Range(11f, 18f);
        _curDetectEnemyDistance = _detectEnemyDistance;

        _moveDistance = new(Random.Range(4f, 12f), Random.Range(1.5f, 3f));
        _shootDistance = new(_moveDistance.x + 3f, _moveDistance.y + 3f);
        
        DisabledInput = false;

        StartCoroutine(SightIE());
        StartLogic();
    }

    private void StartLogic()
    {
        StopLogic();
        _logicRoutine = StartCoroutine(LogicIE());
    }

    private void StopLogic()
    {
        if (_logicRoutine == null) return;

        StopCoroutine(_logicRoutine);
        _logicRoutine = null;
    }

    private void Update()
    {
        if (DisabledInput) return;

        if (_lookDirection != Vector2.zero)
        {
            Quaternion toRot = Quaternion.LookRotation(Vector3.forward, _lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, RotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (DisabledInput) return;

        if (_isMoving || _isFlexing)
        {
            Vector2 direction = _moveDirection;

            if (_isFlexing)
            {
                switch (_flexDirection)
                {
                    case VectorDirection.UP:
                        direction += new Vector2(transform.up.x, transform.up.y);
                        break;

                    case VectorDirection.DOWN:
                        direction -= new Vector2(transform.up.x, transform.up.y);
                        break;

                    case VectorDirection.LEFT:
                        direction -= new Vector2(transform.right.x, transform.right.y);
                        break;

                    case VectorDirection.RIGHT:
                        direction += new Vector2(transform.right.x, transform.right.y);
                        break;

                }
            }

            direction = Vector2.ClampMagnitude(direction, 1f);
            direction = WorldManager.Instance.ClampMoveInput(_rb.position, direction);

            _rb.velocity = direction * Tank.Speed;
        }
    }

    private IEnumerator LogicIE()
    {
        float waitTime = 0.2f;
        Vector2 wanderingDistance = new(1, 1);

        while (true)
        {
            if (_target == null)
            {
                Destructible dest = FindDestructible();
                if (dest != null)
                {
                    _lookDirection = transform.position.DirectionToPoint(dest.transform.position);
                    _state = AIState.Wandering;
                    MoveToPosition(dest.transform.position, _moveDistance);

                    if (!IsFarFromPosition(dest.transform.position, _shootDistance))
                        Tank.Gun.Shoot();
                }
                else
                {
                    _state = AIState.Wandering;
                    yield return MoveToPositionIE(WorldManager.Instance.RandomPointAround(transform.position, 6f, 1f), wanderingDistance);
                    _state = AIState.Idle;
                    yield return new WaitForSeconds(Random.Range(0.8f, 2f));
                }
            }
            else
            {
                Vector2 spd = Vector2.zero;
                if (Vector2.Distance(_target.Controller.Velocity, Vector2.zero) > 0.1f)
                {
                    spd = _target.Controller.Velocity / (_target.Speed - _accuracy);
                }

                Vector2 pos = new Vector2(_target.transform.position.x, _target.transform.position.y) + spd;
                _lookDirection = transform.position.DirectionToPoint(pos);

                if (IsRetreat)
                {
                    _state = AIState.Retreat;
                    _moveDirection = _target.transform.position.DirectionToPoint(transform.position);

                    if (_distanceToTarget <= MaxEnemyDistance)
                        _isMoving = true;
                }
                else
                {
                    _state = AIState.ChaseToTarget;
                    MoveToPosition(_target.transform.position, _moveDistance);
                }

                if (!IsFarFromPosition(_target.transform.position, _shootDistance))
                    Tank.Gun.Shoot();

                if (!_isFlexing)
                {
                    StartFlex();
                }
                else
                {
                    if (_flexTime <= 0f)
                    {
                        StopFlex();
                    }
                    else
                    {
                        _flexTime -= waitTime;
                    }
                }
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator SightIE()
    {
        while (true)
        {
            if (_target != null)
            {
                _distanceToTarget = DistanceToPoint(_target.transform.position);
                if (_distanceToTarget > MaxEnemyDistance || _target.Health <= 0f || !_target.gameObject.activeSelf)
                {
                    _target = null;
                    OnTargetChanged?.Invoke(_target);
                }
            }

            float minDistance;
            if (_target == null)
                minDistance = _curDetectEnemyDistance;
            else
                minDistance = _curDetectEnemyDistance / 4f;

            float dist;
            Tank tar = _target;
            float curDist = _curDetectEnemyDistance;
            foreach (Tank tank in WorldManager.Instance.Tanks)
            {
                if (tank.TeamID == Tank.TeamID || tank == Tank || tank.Health <= 0f || !tank.gameObject.activeSelf) continue;
                if (tank == _target) continue;

                dist = DistanceToPoint(tank.transform.position);
                if (dist > _curDetectEnemyDistance) continue;

                if (_target != null)
                    if (dist > minDistance) continue;

                if (dist < curDist)
                {
                    curDist = dist;
                    tar = tank;
                }
            }

            if (_target != tar)
            {
                _target = tar;
                OnTargetChanged?.Invoke(_target);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private Destructible FindDestructible()
    {
        Destructible selectedDest = null;
        float curHealth = WorldManager.Instance.Destructibles[0].Health;
        foreach (Destructible dest in WorldManager.Instance.Destructibles)
        {
            float distance = DistanceToPoint(dest.transform.position);
            if (distance > _curDetectEnemyDistance) continue;

            if (selectedDest == null || dest.Health < curHealth)
            {
                curHealth = dest.Health;
                selectedDest = dest;
            }
        }

        return selectedDest;
    }

    private bool IsFarFromPosition(in Vector2 position, in Vector2 minDistance)
    {
        Vector2 dist = new(Mathf.Abs(position.x - transform.position.x), Mathf.Abs(position.y - transform.position.y));
        return dist.x > minDistance.x || dist.y > minDistance.y;
    }

    private void MoveToPosition(in Vector2 position, in Vector2 minDistance)
    {
        if (IsFarFromPosition(position, minDistance))
        {
            _moveDirection = transform.position.DirectionToPoint(position);
            _isMoving = true;
        }
        else
        {
            _moveDirection = Vector2.zero;
            _isMoving = false;
        }
    }

    private IEnumerator MoveToPositionIE(Vector2 position, Vector2 minDistance)
    {
        while (IsFarFromPosition(position, minDistance))
        {
            _isMoving = true;
            _moveDirection = transform.position.DirectionToPoint(position);
            _lookDirection = _moveDirection;
            yield return new WaitForSeconds(0.2f);
        }

        _isMoving = false;
        _moveDirection = Vector2.zero;
    }

    private float DistanceToPoint(Vector2 point)
    {
        return Vector2.Distance(transform.position, point);
    }

    private void StartFlex()
    {
        _isFlexing = true;
        _flexTime = Random.Range(0.8f, 2f);

        if (_state == AIState.ChaseToTarget)
        {
            float ch = Random.Range(0f, 1f);
            if (_flexDirection == VectorDirection.DOWN)
            {
                if (ch <= 0.4f)
                {
                    _flexDirection = VectorDirection.LEFT;
                }
                else if (ch <= 0.8f)
                {
                    _flexDirection = VectorDirection.RIGHT;
                }
                else if (ch <= 0.9f)
                {
                    _flexDirection = VectorDirection.UP;
                }
                else
                {
                    _flexDirection = VectorDirection.ZERO;
                }
            }
            else if (ch <= 0.64f)
            {
                if (ch <= 0.32f)
                {
                    _flexDirection = VectorDirection.LEFT;
                }
                else
                {
                    _flexDirection = VectorDirection.RIGHT;
                }
            }
            else
            {
                if (ch <= 0.8f)
                {
                    _flexDirection = VectorDirection.DOWN;
                }
                else if (ch <= 0.9f)
                {
                    _flexDirection = VectorDirection.UP;
                }
                else
                {
                    _flexDirection = VectorDirection.ZERO;
                }
            }
        }
        else if (_state == AIState.Retreat)
        {
            float ch = Random.Range(0f, 1f);
            if (ch <= 0.45f)
            {
                _flexDirection = VectorDirection.RIGHT;
            }
            else if (ch <= 0.9f)
            {
                _flexDirection = VectorDirection.LEFT;
            }
            else
            {
                _flexDirection = VectorDirection.ZERO;
            }
        }
    }

    private void StopFlex()
    {
        _isFlexing = false;
        _flexTime = 0f;
        _flexDirection = VectorDirection.ZERO;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        if (_target != null)
        {
            Vector2 spd = Vector2.zero;
            if (Vector2.Distance(_target.Controller.Velocity, Vector2.zero) > 0.1f)
            {
                spd = _target.Controller.Velocity / (_target.Speed - _accuracy);
            }

            Vector2 pos = new Vector2(_target.transform.position.x, _target.transform.position.y) + spd;
            Gizmos.DrawLine(transform.position, pos);
        }
        else
        {
            Gizmos.DrawRay(transform.position, _lookDirection * _shootDistance.x);
        }
    }
}
