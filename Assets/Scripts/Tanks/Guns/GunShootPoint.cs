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
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private AudioSource _audioSource;
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
        Bullet bul = WorldManager.Instance.BulletsPool.GetFromPool(_shootPoint.position, transform.rotation).GetComponent<Bullet>();
        bul.Initialize(Damage, BulletSpeed, _bulletSize, _gun.Owner);
        if (_audioSource != null)
        {
            if (!WorldManager.Instance.IsFarFromCamera(transform.position, transform.localScale.x * 5f))
            {
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.volume = Random.Range(0.7f, 0.8f);
                _audioSource.PlayOneShot(_audioSource.clip);
                PrefabManager.Instance.CreateParticles(ParticlesType.Shoot, _shootPoint.transform.position, transform.rotation);
            }
        }
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
