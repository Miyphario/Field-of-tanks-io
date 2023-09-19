using UnityEngine;

public interface IPoolable
{
    bool IsAlive { get; }
    GameObject gameObject { get; }
    void AddToPool();
}