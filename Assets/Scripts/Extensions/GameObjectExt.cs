using UnityEngine;

public static class GameObjectExt
{
    public static void Toggle(this GameObject gameObject, bool active)
    {
        if (gameObject.activeSelf == active) return;
        gameObject.SetActive(active);
    }

    public static void DisableAllExcept<T>(this GameObject gameObject, T componentExcept) where T : Component
    {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (child.TryGetComponent<T>(out var comp))
                if (comp == componentExcept) continue;
            
            child.gameObject.Toggle(false);
        }
    }

    public static void DisableAllExcept(this GameObject gameObject, GameObject gameObjectExcept)
    {
        ToggleAllExcept(gameObject, gameObjectExcept, false);
    }

    public static void EnableAllExcept(this GameObject gameObject, GameObject gameObjectExcept)
    {
        ToggleAllExcept(gameObject, gameObjectExcept, true);
    }

    public static void ToggleAllExcept(this GameObject gameObject, GameObject gameObjectExcept, bool enable)
    {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child == gameObjectExcept) continue;
            child.Toggle(enable);
        }
    }

    public static void DisableAll(this GameObject gameObject)
    {
        gameObject.ToggleAll(false);
    }

    public static void EnableAll(this GameObject gameObject)
    {
        gameObject.ToggleAll(true);
    }

    public static void ToggleAll(this GameObject gameObject, bool enable)
    {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            child.Toggle(enable);
        }
    }
}
