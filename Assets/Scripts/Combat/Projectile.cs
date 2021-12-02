using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    DIRECT,
    FALL,
    FALL_CURVED,
    APPARITION,
    ROTATION_HORIZONTAL,
}

[System.Serializable]
public class Projectile
{
    public GameObject UI_object;
    public ProjectileType type;
    public float speed, length, interval;
}
