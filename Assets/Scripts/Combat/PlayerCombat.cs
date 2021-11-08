using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;

    private bool is_fighting;

    // Start is called before the first frame update
    void Start()
    {
        is_fighting = false;
    }

    // Update is called once per frame
    void Update()
    {
        UI.transform.Find("CombatUI").gameObject.SetActive(is_fighting);
        if (Input.GetKeyDown(KeyCode.I)) // debug
            is_fighting = !is_fighting; // debug
    }
}
