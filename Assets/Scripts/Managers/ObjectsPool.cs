using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    public void AddToPool(GameObject gameObject)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        
        gameObject.transform.SetParent(transform);
    }

    public GameObject GetFromPool(GameObject originalObject, Vector3 newPosition, Quaternion newRotation, Transform newParent)
    {
        GameObject obj = null;
        for (int i = transform.childCount - 1; i >= 0;)
        {
            obj = transform.GetChild(i).gameObject;
            break;
        }

        if (obj == null)
            obj = Instantiate(originalObject);

        obj.transform.SetParent(newParent);
        obj.transform.SetPositionAndRotation(newPosition, newRotation);
        if (!obj.activeSelf)
            obj.SetActive(true);

        return obj;
    }

    public GameObject GetFromPool(GameObject originalObject, Vector3 newPosition, Quaternion newRotation)
    {
        return GetFromPool(originalObject, newPosition, newRotation, null);
    }
}
