using UnityEngine;

public static class Helper
{
    public static void DisableAllExcept<T>(Transform parent, T componentExcept) where T : Component
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.TryGetComponent<T>(out var comp))
                if (comp == componentExcept) continue;

            if (child.gameObject.activeSelf)
                child.gameObject.SetActive(false);
        }
    }

    public static void EnableAllExcept(Transform parent, GameObject gameObjectExcept)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (child == gameObjectExcept) continue;

            if (!child.activeSelf)
                child.SetActive(true);
        }
    }

    public static void EnableAll(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (!child.activeSelf)
                child.SetActive(true);
        }
    }

    public static void EnableAll(GameObject gameObject)
    {
        EnableAll(gameObject.transform);   
    }
}
