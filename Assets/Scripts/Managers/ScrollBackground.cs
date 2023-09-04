using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    private Renderer _renderer;
    private bool _initialized;
    private Vector2 _offset;

    public void Initialize()
    {
        _initialized = true;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        if (!_initialized) return;

        transform.position = WorldManager.Instance.HostPlayer.transform.position;

        Vector2 speed = WorldManager.Instance.HostPlayer.Controller.Velocity * Time.fixedDeltaTime;
        _offset += speed;
        _renderer.material.mainTextureOffset = _offset;
    }
}
