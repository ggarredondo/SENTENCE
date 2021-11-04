using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
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
    // info
    public string name;
    public string age;
    public string gender;

    // stats
    public Type type;
    public float attack;
    public float resilience;
}
