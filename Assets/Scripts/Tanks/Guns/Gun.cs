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

    private void Awake()
    {
        _owner = transform.root.GetComponent<Tank>();
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
}
