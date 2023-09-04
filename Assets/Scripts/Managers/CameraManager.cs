using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float _smoothSpeed = 5f;

    private bool _initialized;

    public void Initialize()
    {
        transform.position = WorldManager.Instance.HostPlayer.transform.position;
        _initialized = true;
    }

    private void FixedUpdate()
    {
        if (!_initialized) return;
        if (WorldManager.Instance.HostPlayer == null) return;

        transform.position = WorldManager.Instance.HostPlayer.transform.position; //Vector2.Lerp(transform.position, WorldManager.Instance.HostPlayer.transform.position, Time.deltaTime * _smoothSpeed);
    }
}
