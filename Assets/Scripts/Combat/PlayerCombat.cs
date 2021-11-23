using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum TurnState
{
    WAITING,
    SELECTING,
    AVOIDING,
    TRANSITION_TO_FIGHT,
    TRANSITION_TO_SELECT,
    TRANSITION_TO_AVOID,
    TRANSITION_TO_END,
}

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;
    EnemyCombat enemy;
    public TurnState current_state = TurnState.WAITING;
    public float rotation_speed = 1f;

    private GameObject AlterSystemUI, CombatUI, ActionMenu, AvoidPanel, health_bar, mana_bar;
    private Image health_bar_image, host;
    private Vector3 host_initial_pos, avoid_initial_pos, avoid_initial_scale, avoid_selecting_scale;

    private void Start()
    {
        AlterSystemUI = UI.transform.Find("AlterSystemUI").gameObject;
        CombatUI = UI.transform.Find("CombatUI").gameObject;
        ActionMenu = CombatUI.transform.Find("ActionMenu").gameObject;
        AvoidPanel = CombatUI.transform.Find("AvoidPanel").gameObject;
        health_bar = CombatUI.transform.Find("PlayerHealthBar").gameObject;
        health_bar_image = health_bar.transform.Find("ProgressForeground").GetComponent<Image>();
        mana_bar = ActionMenu.transform.Find("PlayerManaBar").gameObject;
        mana_bar.transform.Find("ManaText").GetComponent<Text>().text = stats.max_mana.ToString() + "\n/\n" + stats.max_mana.ToString();
        host = CombatUI.transform.Find("Host").GetComponent<Image>();
        host_initial_pos = host.transform.localPosition;
        avoid_initial_pos = AvoidPanel.transform.localPosition;
        avoid_initial_scale = AvoidPanel.transform.localScale;
        avoid_selecting_scale = new Vector3(0.3f, 0.3f, 1f);
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING && current_state != TurnState.TRANSITION_TO_FIGHT);
        AlterSystemUI.SetActive(current_state == TurnState.SELECTING || current_state == TurnState.WAITING);
        ActionMenu.SetActive(current_state == TurnState.SELECTING);
        health_bar.SetActive(current_state == TurnState.AVOIDING);
        if (current_state == TurnState.TRANSITION_TO_FIGHT)
            current_state = TurnState.TRANSITION_TO_SELECT;
        else if (current_state == TurnState.TRANSITION_TO_SELECT)
        {
            host.transform.localPosition = host_initial_pos;
            AvoidPanel.transform.localPosition = host_initial_pos;
            AvoidPanel.transform.localScale = avoid_selecting_scale;
            current_state = TurnState.SELECTING;
        }
        else if (current_state == TurnState.TRANSITION_TO_AVOID)
        {
            host.transform.localPosition = AvoidPanel.GetComponent<BoxCollider2D>().bounds.center;
            AvoidPanel.transform.localPosition = avoid_initial_pos;
            AvoidPanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            AvoidPanel.transform.localScale = avoid_initial_scale;
            current_state = TurnState.AVOIDING;
        }
    }

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    public void Attack()
    {
        current_state = TurnState.TRANSITION_TO_AVOID;
        enemy.TakeDamage(stats.system[0].attack);
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        health_bar_image.transform.localScale = new Vector3(stats.health / stats.max_health, 1f, 1f);
        if (stats.health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        UIStateManagement();
        switch (current_state)
        {
            case TurnState.SELECTING:
                AvoidPanel.transform.Rotate(0f, 0f, rotation_speed);
            break;
        }
    }
}
