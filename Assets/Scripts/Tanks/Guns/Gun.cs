using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    private bool _isShooting;
    public bool IsShooting => _isShooting;

    private List<GunShootPoint> _shootPoints = new();

    [SerializeField] private Sprite _uiSprite;
    public Sprite UISprite => _uiSprite;

    private Tank _owner;
    public Tank Owner => _owner;

    public void Initialize(Tank owner)
    {
        _owner = owner;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).TryGetComponent(out GunShootPoint shootPoint))
            {
                _shootPoints.Add(shootPoint);
                _shootPoints[^1].Initialize(this);
            }
        }
    }

    public void ShootStart()
    {
        _isShooting = true;
        foreach (var point in _shootPoints)
        {
            point.Shoot();
        }
    }

    public void ShootEnd()
    {
        _isShooting = false;
    }

    public void Shoot()
    {
        ShootStart();
        _isShooting = false;
    }

    public void Disable()
    {
        ShootEnd();
        StopAllCoroutines();
        gameObject.Toggle(false);
    }

    public Gun Change(Gun gun)
    {
        Gun g = Instantiate(gun.gameObject, transform.parent).GetComponent<Gun>();
        Destroy(gameObject);
        return g;
    }
}
