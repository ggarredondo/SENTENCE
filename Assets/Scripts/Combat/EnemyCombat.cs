using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    public EnemyStats stats;
    SpriteRenderer sprite_renderer;
    public GameObject UI;
    public GameObject enemy_object;
    public GameObject player_object;
    public float depletion_transition_speed = 2.5f, depletion_transition_threshold = 0.005f, fade_speed = 0.65f;

    private Image health_bar;
    private PlayerCombat player;
    private float timer;
    private Vector3 target_health_scale;
    private GameObject cut_animation, FadePanel;
    private TransitionPhase current_phase = TransitionPhase.FIRST_PHASE;

    // Start is called before the first frame update
    void Start() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        UI.transform.Find("CombatUI").Find("EnemySprite").GetComponent<Image>().sprite = sprite_renderer.sprite;
        health_bar = UI.transform.Find("CombatUI").Find("EnemyHealthBar").Find("ProgressForeground").GetComponent<Image>();
        FadePanel = UI.transform.Find("Fade").Find("FadePanel").gameObject;
        player = player_object.GetComponent<PlayerCombat>();
        cut_animation = UI.transform.Find("CombatUI").Find("Cut").gameObject;
        cut_animation.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        if (stats.health <= 0) {
            stats.health = 0f;
            player.current_state = TurnState.TRANSITION_TO_ENEMYS_DEATH;
        }
        target_health_scale = new Vector3(stats.health / stats.max_health, 1f, 1f);
    }

    public void ResetHealthBar() {
        health_bar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void UIStateManagement()
    {
        UI.transform.Find("CombatUI").Find("EnemyHealthBar").gameObject.SetActive(player.current_state == TurnState.ATTACKING ||
            player.current_state == TurnState.TRANSITION_TO_ENEMYS_DEATH);
    }

    private void AttackAnimation()
    {
        cut_animation.SetActive(true);
        if (cut_animation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            health_bar.transform.localScale = Vector3.Lerp(health_bar.transform.localScale, target_health_scale,
                Time.deltaTime * depletion_transition_speed);
        }
    }

    private void ScriptAnimation()
    {
        switch (player.current_state)
        {
            case TurnState.AVOIDING:
                if (timer <= Time.time) //temp
                    player.current_state = TurnState.TRANSITION_TO_SELECT; //temp
                break;

            case TurnState.ATTACKING:
                AttackAnimation();
                if (Vector3.Distance(health_bar.transform.localScale, target_health_scale) <= depletion_transition_threshold)
                {
                    health_bar.transform.localScale = target_health_scale;
                    player.current_state = TurnState.TRANSITION_TO_AVOID;
                    cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    cut_animation.SetActive(false);          
                }
                timer = Time.time + interval; //temp
                break;

            case TurnState.TRANSITION_TO_ENEMYS_DEATH:
                switch (current_phase)
                {
                    case TransitionPhase.FIRST_PHASE:
                        AttackAnimation();
                        if (Vector3.Distance(health_bar.transform.localScale, target_health_scale) <= depletion_transition_threshold)
                        {
                            health_bar.transform.localScale = target_health_scale;
                            current_phase = TransitionPhase.SECOND_PHASE;
                            cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            cut_animation.SetActive(false);
                        }
                        break;

                    case TransitionPhase.SECOND_PHASE:
                        FadePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, FadePanel.GetComponent<Image>().color.a
                            + Time.deltaTime * fade_speed);
                        if (FadePanel.GetComponent<Image>().color.a >= 1f) {
                            sprite_renderer.color = new Color(1f, 1f, 1f, 0f);
                            player.ActivateCombatUI = false;
                            current_phase = TransitionPhase.THIRD_PHASE;
                        }
                        break;

                    case TransitionPhase.THIRD_PHASE:
                        FadePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, FadePanel.GetComponent<Image>().color.a
                            - Time.deltaTime * fade_speed);
                        if (FadePanel.GetComponent<Image>().color.a <= 0f) {
                            player.current_state = TurnState.WAITING;
                            player.ResetAvoidPanelRotation();
                            enemy_object.SetActive(false);
                        }
                        break;
                }
                break;
        }
    }

    float interval = 4f;//temp
    // Update is called once per frame
    void Update()
    {
        if (player.GetEnemy() == this) {
            UIStateManagement();
            ScriptAnimation();
        }
    }
}
