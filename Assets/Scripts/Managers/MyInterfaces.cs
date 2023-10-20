using System;
using UnityEngine;

public interface IPoolable
{
    bool IsAlive { get; }
    GameObject gameObject { get; }
    event Action OnAddedToPool;
    void AddToPool();
    void Kill();
}