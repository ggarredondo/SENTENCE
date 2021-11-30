using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float level, experience;
    public float health, max_health;
    public float mana, max_mana, mana_regen;
    public List<Alter> system;
}
