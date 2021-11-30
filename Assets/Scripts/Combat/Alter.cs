using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Protector,
    Caretaker,
    Persecutor,
    Gatekeeper,
    Child,
    Animal,
    Supernatural,
    Fragment
}

[System.Serializable]
public class Alter
{
    // variables
    public Sprite sprite;

    // info
    public string name;
    public string age;
    public string gender;

    // stats
    public Type type;
    public float attack; // to damage traumas
    public float resilience; // to resist emotional attacks

    // skills
    public List<MagicSkill> skills;
}
