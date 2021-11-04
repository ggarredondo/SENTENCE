using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public GameObject CombatUI;

    private bool is_fighting;

    // Start is called before the first frame update
    void Start()
    {
        is_fighting = false;
    }

    // Update is called once per frame
    void Update()
    {
        CombatUI.SetActive(is_fighting);
    }
}
