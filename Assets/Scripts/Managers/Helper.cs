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

    public static void DisableAllExcept(Transform parent, GameObject gameObjectExcept)
    {
        ToggleAllExcept(parent, gameObjectExcept, false);
    }

    public static void EnableAllExcept(Transform parent, GameObject gameObjectExcept)
    {
        ToggleAllExcept(parent, gameObjectExcept, true);
    }

    private static void ToggleAllExcept(Transform parent, GameObject gameObjectExcept, bool enable)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (child == gameObjectExcept) continue;

            if (child.activeSelf != enable)
                child.SetActive(enable);
        }
    }

    public static void DisableAll(Transform parent)
    {
        ToggleAll(parent, false);
    }

    public static void DisableAll(GameObject gameObject)
    {
        DisableAll(gameObject.transform);
    }

    public static void EnableAll(Transform parent)
    {
        ToggleAll(parent, true);
    }

    public static void EnableAll(GameObject gameObject)
    {
        EnableAll(gameObject.transform);
    }

    private static void ToggleAll(Transform parent, bool enable)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (child.activeSelf != enable)
                child.SetActive(enable);
        }
    }
}
