using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    WAITING,
    SELECTING,
    AVOIDING
}

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;
    EnemyCombat enemy;
    public TurnState current_state = TurnState.WAITING;

    private GameObject AlterSystemUI, CombatUI, ActionMenu, AvoidPanel;

    private void Start()
    {
        AlterSystemUI = UI.transform.Find("AlterSystemUI").gameObject;
        CombatUI = UI.transform.Find("CombatUI").gameObject;
        ActionMenu = CombatUI.transform.Find("ActionMenu").gameObject;
        AvoidPanel = CombatUI.transform.Find("AvoidPanel").gameObject;
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING);
        AlterSystemUI.SetActive(current_state == TurnState.SELECTING || current_state == TurnState.WAITING);
        ActionMenu.SetActive(current_state == TurnState.SELECTING);
        AvoidPanel.SetActive(current_state == TurnState.AVOIDING);
    }

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    // Update is called once per frame
    void Update()
    {
        UIStateManagement();
        switch (current_state)
        {
            case (TurnState.SELECTING):
                
            break;
            case (TurnState.AVOIDING):

            break;
        }
    }
}
