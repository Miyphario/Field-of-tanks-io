using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    public void AddToPool(GameObject gameObject)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject curObj = transform.GetChild(i).gameObject;
            IPoolable child = curObj.GetComponent<IPoolable>();
            if (!child.IsAlive)
            {
                obj = curObj;
                break;
            }
        }

        if (obj == null)
            obj = Instantiate(_prefab, transform);

        obj.transform.SetPositionAndRotation(position, rotation);
        if (!obj.activeSelf)
            obj.SetActive(true);

        return obj;
    }
}
