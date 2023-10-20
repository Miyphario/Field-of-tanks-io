using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxInactiveObjects = 10;
    private Coroutine _destroyInactiveRoutine;
    private int _inactiveObjects;

    public void AddToPool(IPoolable obj)
    {
        obj.gameObject.Toggle(false);
        _inactiveObjects++;
    }

    public GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            IPoolable child = transform.GetChild(i).GetComponent<IPoolable>();
            if (child.gameObject.IsDestroyed()) continue;
            if (!child.IsAlive)
            {
                obj = child.gameObject;
                _inactiveObjects--;
                break;
            }
        }

        if (obj == null)
            obj = Instantiate(_prefab, transform);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.Toggle(true);

        return obj;
    }

    public void DestroyInactive()
    {
        if (_inactiveObjects >= _maxInactiveObjects)
            _destroyInactiveRoutine ??= StartCoroutine(DestroyInactiveIE());
    }

    public IEnumerator CleanupIE(int objectsPerCycle, float destroyTime)
    {
        int index = transform.childCount - 1;
        while (index >= 0)
        {
            for (int i = 0; i < objectsPerCycle; i++)
            {
                if (index < 0) break;

                if (index >= transform.childCount)
                {
                    index--;
                    continue;
                }

                Transform child = transform.GetChild(index);
                if (child == null)
                {
                    index--;
                    continue;
                }

                IPoolable obj = child.GetComponent<IPoolable>();
                obj.AddToPool();
                index--;
            }

            yield return new WaitForSeconds(destroyTime);
        }

        DestroyInactive();
    }

    private IEnumerator DestroyInactiveIE()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            IPoolable child = transform.GetChild(i).GetComponent<IPoolable>();
            if (!child.IsAlive)
            {
                _inactiveObjects--;
                child.Kill();
            }
        }
        
        yield return new WaitForSeconds(0.1f);
        _destroyInactiveRoutine = null;
    }
}
