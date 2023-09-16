
using System;

public enum Gamemode
{
    Deathmath,
    Teammatch
}

public enum UpgradeType
{
    Damage,
    BulletSpeed,
    Speed,
    FireRate,
    MaxHealth,
    TouchDamage
}

public enum AIState
{
    Idle,
    Wandering,
    ChaseToTarget,
    Retreat
}

public enum VectorDirection
{
    ZERO, UP, DOWN, LEFT, RIGHT
}

public enum UpgradeMenu
{
    None, Base, Tank, Gun, NewGun
}

public enum ParticlesType
{
    TankExplode, Shoot
}

[Serializable]
public enum UISound
{
    Button, Exit
}