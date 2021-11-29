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
    SWITCHING,
    TRANSITION_TO_FIGHT,
    TRANSITION_TO_SELECT,
    TRANSITION_TO_AVOID,
    TRANSITION_TO_PLAYERS_DEATH,
    TRANSITION_TO_ENEMYS_DEATH
}

public enum TransitionPhase
{
    FIRST_PHASE,
    SECOND_PHASE,
    THIRD_PHASE
}

public class PlayerCombat : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInput player;
    public GameObject UI;
    EnemyCombat enemy;
    public TurnState current_state = TurnState.WAITING;
    public bool ActivateCombatUI = false;
    public float rotation_speed = -200f, selecting_scale = 0.3f, transition_speed = 4f, transition_time = 1f, fade_speed = 0.65f;

    private GameObject AlterSystemUI, CombatUI, ActionMenu, AvoidPanel, FadePanel, health_bar, mana_bar;
    private Image health_bar_image, host;
    private Vector3 host_initial_pos, avoid_initial_pos, avoid_initial_scale, avoid_select_scale;
    private float timer;
    private TransitionPhase current_phase = TransitionPhase.FIRST_PHASE;
    private Alter current_alter;

    private void Start()
    {
        AlterSystemUI = UI.transform.Find("AlterSystemUI").gameObject;
        CombatUI = UI.transform.Find("CombatUI").gameObject;
        ActionMenu = CombatUI.transform.Find("ActionMenu").gameObject;
        AvoidPanel = CombatUI.transform.Find("AvoidPanel").gameObject;
        FadePanel = UI.transform.Find("Fade").Find("FadePanel").gameObject;
        health_bar = CombatUI.transform.Find("PlayerHealthBar").gameObject;
        health_bar_image = health_bar.transform.Find("ProgressForeground").GetComponent<Image>();
        mana_bar = ActionMenu.transform.Find("PlayerManaBar").gameObject;
        mana_bar.transform.Find("ManaText").GetComponent<Text>().text = stats.max_mana.ToString() + "\n/\n" + stats.max_mana.ToString();
        host = CombatUI.transform.Find("Host").GetComponent<Image>();
        host_initial_pos = host.transform.localPosition;
        avoid_initial_pos = AvoidPanel.transform.localPosition;
        avoid_initial_scale = AvoidPanel.transform.localScale;
        avoid_select_scale = new Vector3(selecting_scale, selecting_scale, 1f);
        current_alter = stats.system[0];
    }

    private void AlterUISpriteUpdate()
    {
        AlterSystemUI.transform.Find("Alter1").Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[0].sprite;
        if (stats.system[1] != null)
            AlterSystemUI.transform.Find("Alter2").Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[1].sprite;
        if (stats.system[2] != null)
            AlterSystemUI.transform.Find("Alter3").Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[2].sprite;
        if (stats.system[3] != null)
            AlterSystemUI.transform.Find("Alter4").Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[3].sprite;
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING && ActivateCombatUI);
        FadePanel.SetActive(current_state == TurnState.TRANSITION_TO_FIGHT || current_state == TurnState.TRANSITION_TO_ENEMYS_DEATH);
        AlterSystemUI.SetActive(current_state != TurnState.AVOIDING && current_state != TurnState.TRANSITION_TO_AVOID
            && current_state != TurnState.TRANSITION_TO_SELECT && ActivateCombatUI);
        AlterSystemUI.transform.Find("Alter4").gameObject.SetActive(current_state == TurnState.SWITCHING);
        ActionMenu.SetActive(current_state == TurnState.SELECTING);
        health_bar.SetActive(current_state == TurnState.AVOIDING);
    }

    private void ScriptAnimation()
    {
        if (current_state == TurnState.SELECTING || current_state == TurnState.ATTACKING 
            || current_state == TurnState.TRANSITION_TO_ENEMYS_DEATH)
        {
            AvoidPanel.transform.Rotate(0f, 0f, Time.deltaTime * rotation_speed);
            timer = Time.time + transition_time;
        }

        switch (current_state)
        {
            case TurnState.TRANSITION_TO_FIGHT:
                switch (current_phase)
                {
                    case TransitionPhase.FIRST_PHASE:
                        AvoidPanel.transform.localPosition = host_initial_pos;
                        AvoidPanel.transform.localScale = avoid_select_scale;
                        current_phase = TransitionPhase.SECOND_PHASE;
                        break;

                    case TransitionPhase.SECOND_PHASE:
                        FadePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, FadePanel.GetComponent<Image>().color.a 
                            + Time.deltaTime * fade_speed);
                        if (FadePanel.GetComponent<Image>().color.a >= 1f) {
                            ActivateCombatUI = true;
                            current_phase = TransitionPhase.THIRD_PHASE;
                        }
                        break;

                    case TransitionPhase.THIRD_PHASE:
                        FadePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, FadePanel.GetComponent<Image>().color.a 
                            - Time.deltaTime * fade_speed);
                        if (FadePanel.GetComponent<Image>().color.a <= 0f) {
                            current_state = TurnState.SELECTING;
                            current_phase = TransitionPhase.FIRST_PHASE; // reset
                        }
                        break;
                }
                break;

            case TurnState.AVOIDING:
                timer = Time.time + transition_time;
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

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    public EnemyCombat GetEnemy() {
        return enemy;
    }

    public void ResetAvoidPanelRotation() {
        AvoidPanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void Attack()
    {
        current_state = TurnState.ATTACKING;
        enemy.TakeDamage(current_alter.attack);
    }

    public void TakeDamage(float damage)
    {
        stats.health -= Mathf.Round(damage - damage * current_alter.resilience * 0.01f);
        health_bar_image.transform.localScale = new Vector3(stats.health / stats.max_health, 1f, 1f);
        if (stats.health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        UIStateManagement();
        ScriptAnimation();
    }
}
