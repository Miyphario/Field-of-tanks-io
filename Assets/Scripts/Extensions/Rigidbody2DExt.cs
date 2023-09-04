using UnityEngine;

public static class Rigidbody2DExt
{
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 position, float radius, float upwardsModifier, ForceMode2D mode)
    {
        Vector3 direction = rb.position - position;
        float distance = direction.magnitude / radius;

        if (upwardsModifier == 0f)
        {
            direction /= distance;
        }
        else
        {
            direction.y += upwardsModifier;
            direction.Normalize();
        }

        rb.AddForce(Mathf.Lerp(0, explosionForce, 1 - distance) * direction, mode);
    }

    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 position, float radius)
    {
        AddExplosionForce(rb, explosionForce, position, radius, 0f, ForceMode2D.Force);
    }

    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 position, float radius, ForceMode2D mode)
    {
        AddExplosionForce(rb, explosionForce, position, radius, 0f, mode);
    }

    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 position, float radius, float upwardsModifier)
    {
        AddExplosionForce(rb, explosionForce, position, radius, upwardsModifier, ForceMode2D.Force);
    }
}
