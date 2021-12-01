using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    DIRECT,
    FALL,
    CURVED_FALL,
    APPARITION
}

[System.Serializable]
public class Projectile
{
    public Sprite sprite;
    public float length, interval;
    public ProjectileType type;
}
