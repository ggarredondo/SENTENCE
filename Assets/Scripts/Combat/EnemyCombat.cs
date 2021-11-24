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
    public float depletion_transition_speed = 2.5f, depletion_transition_threshold = 0.005f;

    private Image health_bar;
    private PlayerCombat player;
    private float timer;
    private Vector3 target_health_scale;
    private GameObject cut_animation;

    // Start is called before the first frame update
    void Start() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        UI.transform.Find("CombatUI").Find("EnemySprite").GetComponent<Image>().sprite = sprite_renderer.sprite;
        health_bar = UI.transform.Find("CombatUI").Find("EnemyHealthBar").Find("ProgressForeground").GetComponent<Image>();
        player = player_object.GetComponent<PlayerCombat>();
        cut_animation = UI.transform.Find("CombatUI").Find("Cut").gameObject;
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        target_health_scale = new Vector3(stats.health / stats.max_health, 1f, 1f);
        if (stats.health <= 0) { //temp
            player.current_state = TurnState.WAITING; //temp
            enemy_object.SetActive(false); //temp
        } //temp
    }

    float interval = 4f;//temp
    // Update is called once per frame
    void Update()
    {
        UI.transform.Find("CombatUI").Find("EnemyHealthBar").gameObject.SetActive(player.current_state == TurnState.ATTACKING);
        cut_animation.SetActive(player.current_state == TurnState.ATTACKING);

        switch (player.current_state)
        {
            case TurnState.AVOIDING:
                if (timer <= Time.time) //temp
                    player.current_state = TurnState.TRANSITION_TO_SELECT; //temp
                break;

            case TurnState.ATTACKING:
                if (cut_animation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End")) {
                    cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                    health_bar.transform.localScale = Vector3.Lerp(health_bar.transform.localScale, target_health_scale,
                        Time.deltaTime * depletion_transition_speed);
                }

                if (Vector3.Distance(health_bar.transform.localScale, target_health_scale) <= depletion_transition_threshold) {
                    health_bar.transform.localScale = target_health_scale;
                    player.current_state = TurnState.TRANSITION_TO_AVOID;
                    cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    timer = Time.time + interval; //temp
                }
                break;
        }
    }
}
