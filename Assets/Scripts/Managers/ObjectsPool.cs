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
        AddToPool(obj.gameObject);
    }

    public void AddToPool(GameObject gameObject)
    {
        gameObject.Toggle(false);
        _inactiveObjects++;
        if (_inactiveObjects >= _maxInactiveObjects)
            _destroyInactiveRoutine ??= StartCoroutine(DestroyInactive());
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

    public IEnumerator ClearIE(int objectsPerCycle, float destroyTime)
    {
        int index = transform.childCount - 1;
        while (index >= 0)
        {
            for (int i = 0; i < objectsPerCycle; i++)
            {
                AddToPool(transform.GetChild(index).gameObject);
                index--;
                if (index < 0) break;
            }

            yield return new WaitForSeconds(destroyTime);
        }
    }

    private IEnumerator DestroyInactive()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            IPoolable child = transform.GetChild(i).GetComponent<IPoolable>();
            if (!child.IsAlive)
            {
                _inactiveObjects--;
                Destroy(child.gameObject);
            }
        }
        
        yield return new WaitForSeconds(0.1f);
        _destroyInactiveRoutine = null;
    }
}
