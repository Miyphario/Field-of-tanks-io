using UnityEngine;

public static class GameObjectExt
{
    public static void Toggle(this GameObject gameObject, bool active)
    {
        if (gameObject.activeSelf == active) return;
        gameObject.SetActive(active);
    }
}
