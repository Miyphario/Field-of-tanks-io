using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float Damage => _bulletDamage + _owner.Damage;
    public float BulletSpeed => _bulletSpeed + _owner.BulletSpeed;
    public float FireRate => Mathf.Clamp(_owner.FireRate - _fireRate, 0.05f, 10f);

    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _fireRate;
    private Coroutine _shootRoutine;
    private bool _isShooting;
    public bool IsShooting => _isShooting;
    private bool _canShoot = true;

    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform[] _shootPoints;

    private Tank _owner;

    private void Awake()
    {
        _owner = transform.root.GetComponent<Tank>();
    }

    public void ShootStart()
    {
        _isShooting = true;
        if (!_canShoot) return;
        _shootRoutine = StartCoroutine(ShootIE());
    }

    public void ShootEnd()
    {
        _isShooting = false;
        if (_shootRoutine == null || !_canShoot) return;
        StopCoroutine(_shootRoutine);
        _shootRoutine = null;
    }

    public void Shoot()
    {
        if (!_canShoot) return;
        ShootStart();
        _isShooting = false;
    }

    public void Disable()
    {
        ShootEnd();
        StopAllCoroutines();
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    private void CreateBullet()
    {
        foreach (var point in _shootPoints)
        {
            Bullet bul = Instantiate(_bullet, point.position, transform.rotation).GetComponent<Bullet>();
            bul.Initialize(Damage, BulletSpeed, _owner);
        }
    }

    private IEnumerator ShootIE()
    {
        while (_isShooting)
        {
            CreateBullet();
            _canShoot = false;
            yield return new WaitForSeconds(FireRate);
            _canShoot = true;
        }
    }
}
