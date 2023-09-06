using System.Collections;
using UnityEngine;

public class GunShootPoint : MonoBehaviour
{
    public float Damage => _bulletDamage + _gun.Owner.Damage;
    public float BulletSpeed => _bulletSpeed + _gun.Owner.BulletSpeed;
    public float FireRate => Mathf.Clamp(_gun.Owner.FireRate - _fireRate, 0.05f, 10f);

    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _bulletSize;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _shootPoint;
    private bool _canShoot = true;

    private Gun _gun;

    public void Initialize(Gun gun)
    {
        _gun = gun;
        _canShoot = true;
    }

    public void Shoot()
    {
        if (!_canShoot) return;
        StartCoroutine(ShootIE());
    }

    private void CreateBullet()
    {
        Bullet bul = WorldManager.Instance.BulletsPool.GetFromPool(_bullet, _shootPoint.position, transform.rotation).GetComponent<Bullet>();
        bul.Initialize(Damage, BulletSpeed, _bulletSize, _gun.Owner);
    }

    private IEnumerator ShootIE()
    {
        while (_gun.IsShooting)
        {
            CreateBullet();
            _canShoot = false;
            yield return new WaitForSeconds(FireRate);
            _canShoot = true;
        }
    }
}
