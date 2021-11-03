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

public class Alter : MonoBehaviour
{
    public new string name;
    public string age;
    public string gender;
    public Type type;
    public float damage;
    public float resilience;
}
