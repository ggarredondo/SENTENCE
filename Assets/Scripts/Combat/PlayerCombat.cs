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
    ATTACKING,
    TRANSITION_TO_FIGHT,
    TRANSITION_TO_SELECT,
    TRANSITION_TO_AVOID,
    TRANSITION_TO_PLAYERS_DEATH,
    TRANSITION_TO_ENEMYS_DEATH
}

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;
    EnemyCombat enemy;
    public TurnState current_state = TurnState.WAITING;
    public float rotation_speed = -200f, selecting_scale = 0.3f, transition_speed = 4f, transition_time = 1f;

    private GameObject AlterSystemUI, CombatUI, ActionMenu, AvoidPanel, health_bar, mana_bar;
    private Image health_bar_image, host;
    private Vector3 host_initial_pos, avoid_initial_pos, avoid_initial_scale, avoid_select_scale;
    private float timer;

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
        avoid_select_scale = new Vector3(selecting_scale, selecting_scale, 1f);
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING && current_state != TurnState.TRANSITION_TO_FIGHT);
        AlterSystemUI.SetActive(current_state == TurnState.SELECTING || current_state == TurnState.WAITING || 
            current_state == TurnState.ATTACKING);
        ActionMenu.SetActive(current_state == TurnState.SELECTING);
        health_bar.SetActive(current_state == TurnState.AVOIDING);
    }

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    public void Attack()
    {
        current_state = TurnState.ATTACKING;
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
                AvoidPanel.transform.Rotate(0f, 0f, Time.deltaTime * rotation_speed);
                timer = Time.time + transition_time;
                break;

            case TurnState.ATTACKING:
                AvoidPanel.transform.Rotate(0f, 0f, Time.deltaTime * rotation_speed);
                timer = Time.time + transition_time;
                break;

            case TurnState.AVOIDING:
                timer = Time.time + transition_time;
                break;

            case TurnState.TRANSITION_TO_FIGHT:
                current_state = TurnState.TRANSITION_TO_SELECT;
                break;

            case TurnState.TRANSITION_TO_SELECT:
                AvoidPanel.transform.localRotation = Quaternion.Lerp(AvoidPanel.transform.localRotation, Quaternion.Euler(0f, 0f, 180f),
                    Time.deltaTime * transition_speed);

                host.transform.localPosition = Vector2.Lerp(host.transform.localPosition, host_initial_pos,
                   Time.deltaTime * transition_speed);

                AvoidPanel.transform.localPosition = Vector2.Lerp(AvoidPanel.transform.localPosition, host_initial_pos,
                  Time.deltaTime * transition_speed);

                AvoidPanel.transform.localScale = Vector3.Lerp(AvoidPanel.transform.localScale, avoid_select_scale,
                  Time.deltaTime * transition_speed);

                if (timer <= Time.time)
                {
                    host.transform.localPosition = host_initial_pos;
                    AvoidPanel.transform.localPosition = host_initial_pos;
                    AvoidPanel.transform.localScale = avoid_select_scale;
                    current_state = TurnState.SELECTING;
                }
                break;

            case TurnState.TRANSITION_TO_AVOID:
                AvoidPanel.transform.localRotation = Quaternion.Lerp(AvoidPanel.transform.localRotation, Quaternion.Euler(0f, 0f, 0f),
                    Time.deltaTime * transition_speed);

                host.transform.localPosition = Vector2.Lerp(host.transform.localPosition, avoid_initial_pos,
                   Time.deltaTime * transition_speed);

                AvoidPanel.transform.localPosition = Vector2.Lerp(AvoidPanel.transform.localPosition, avoid_initial_pos,
                  Time.deltaTime * transition_speed);

                AvoidPanel.transform.localScale = Vector3.Lerp(AvoidPanel.transform.localScale, avoid_initial_scale,
                  Time.deltaTime * transition_speed);

                if (timer <= Time.time)
                {
                    host.transform.localPosition = avoid_initial_pos;
                    AvoidPanel.transform.localPosition = avoid_initial_pos;
                    AvoidPanel.transform.localScale = avoid_initial_scale;
                    AvoidPanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    current_state = TurnState.AVOIDING;
                }
                break;
        }
    }
}
