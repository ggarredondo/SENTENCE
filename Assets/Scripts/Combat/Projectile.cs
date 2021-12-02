using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileMovement
{
    DIRECT,
    FALL,
    FALL_CURVED,
    APPARITION,
    ROTATION_HORIZONTAL,
}

public enum ProjectileType
{
    DROP,
    FACES,
    PUNCH,
    KICK
}

[System.Serializable]
public class Projectile
{
    public GameObject UI_object;
    public ProjectileMovement movement;
    public ProjectileType type;
    public float speed, length, interval;
}
