using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    WAITING,
    SELECTING,
    AVOIDING,
    DEAD
}

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;
    public EnemyCombat enemy;
    public TurnState current_state = TurnState.DEAD;

    private bool was_fighting = true;

    void FightTransition() 
    {
        if (player.is_fighting != was_fighting) 
        {
            UI.transform.Find("CombatUI").gameObject.SetActive(player.is_fighting);
            if (player.is_fighting)
                current_state = TurnState.SELECTING;
            else
                current_state = TurnState.DEAD;
            was_fighting = player.is_fighting;
        }
    }

    // Update is called once per frame
    void Update()
    {
        FightTransition();
        switch (current_state)
        {

        }
    }
}
