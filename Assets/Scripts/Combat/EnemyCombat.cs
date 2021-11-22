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

    private Image health_bar;
    private PlayerCombat player;

    // Start is called before the first frame update
    void Start() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        UI.transform.Find("CombatUI").Find("EnemySprite").GetComponent<Image>().sprite = sprite_renderer.sprite;
        health_bar = UI.transform.Find("CombatUI").Find("EnemySprite").Find("EnemyHealthBar").Find("ProgressForeground").GetComponent<Image>();
        player = player_object.GetComponent<PlayerCombat>();
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        health_bar.transform.localScale = new Vector3(stats.health / stats.max_health, 1f, 1f);
        if (stats.health <= 0) {
            player.current_state = TurnState.WAITING;
            enemy_object.SetActive(false);
        }
    }

    float interval = 4f, timer;//temp
    // Update is called once per frame
    void Update()
    {
        switch (player.current_state)
        {
            case TurnState.SELECTING: //temp
                timer = Time.time + interval; //temp
            break; //temp
            case TurnState.AVOIDING:
                if (timer <= Time.time) //temp
                    player.current_state = TurnState.TRANSITION_TO_SELECT; //temp
            break;
        }
    }
}
