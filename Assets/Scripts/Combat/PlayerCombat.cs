using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private GameObject AlterSystemUI, CombatUI, ActionMenu, AvoidPanel, mana_bar;
    private Image health_bar, host;

    private void Start()
    {
        AlterSystemUI = UI.transform.Find("AlterSystemUI").gameObject;
        CombatUI = UI.transform.Find("CombatUI").gameObject;
        ActionMenu = CombatUI.transform.Find("ActionMenu").gameObject;
        AvoidPanel = CombatUI.transform.Find("AvoidPanel").gameObject;
        health_bar = AvoidPanel.transform.Find("PlayerHealthBar").Find("ProgressForeground").GetComponent<Image>();
        mana_bar = ActionMenu.transform.Find("PlayerManaBar").gameObject;
        mana_bar.transform.Find("ManaText").GetComponent<Text>().text = stats.max_mana.ToString() + "\n/\n" + stats.max_mana.ToString();
        host = CombatUI.transform.Find("Host").GetComponent<Image>();
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING);
        CombatUI.transform.Find("HostSafeZone").gameObject.SetActive(current_state != TurnState.AVOIDING);
        AlterSystemUI.SetActive(current_state == TurnState.SELECTING || current_state == TurnState.WAITING);
        ActionMenu.SetActive(current_state == TurnState.SELECTING);
        AvoidPanel.SetActive(current_state == TurnState.AVOIDING);
    }

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    public void Attack()
    {
        enemy.TakeDamage(stats.system[0].attack);
        current_state = TurnState.AVOIDING;
        host.transform.position = AvoidPanel.GetComponent<BoxCollider2D>().bounds.center;
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        health_bar.transform.localScale = new Vector3(stats.health / stats.max_health, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        UIStateManagement();
        switch (current_state)
        {
            case TurnState.AVOIDING:

            break;
        }
    }
}
