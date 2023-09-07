
public interface IPoolable
{
    bool IsAlive { get; }
    void AddToPool();
}