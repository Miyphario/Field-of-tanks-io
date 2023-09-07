using UnityEngine;

public class Gun : MonoBehaviour
{
    
    private bool _isShooting;
    public bool IsShooting => _isShooting;

    [SerializeField] private GunShootPoint[] _shootPoints;

    [SerializeField, Header("UI")]
    private Sprite _uiSprite;
    public Sprite UISprite => _uiSprite;

    private Tank _owner;
    public Tank Owner => _owner;

    public void Initialize(Tank owner)
    {
        _owner = owner;
        foreach (var point in _shootPoints)
        {
            point.Initialize(this);
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
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public Gun Change(Gun gun)
    {
        Gun g = Instantiate(gun.gameObject, transform.parent).GetComponent<Gun>();
        Destroy(gameObject);
        return g;
    }
}
