using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    public EnemyStats stats;
    SpriteRenderer sprite_renderer;
    public GameObject UI, ProjectilesPanel;
    public GameObject enemy_object;
    public GameObject player_object;
    public int current_projectile = 0;
    public List<Projectile> projectiles;
    public float depletion_transition_speed = 2.5f, depletion_transition_threshold = 0.005f, fade_speed = 0.65f,
        damage_threshold = 0.1f;

    private Image health_bar;
    private PlayerCombat player;
    private float timer_length, timer_interval, host_radius;
    private Vector3 target_health_scale, aux_pos;
    private List<GameObject> spawned_projectiles;
    private List<Vector3> projectile_direction;
    private List<bool> projectile_can_damage;
    private GameObject cut_animation, FadePanel, host;
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
        ProjectilesPanel = UI.transform.Find("CombatUI").Find("ProjectilesPanel").gameObject;
        host = UI.transform.Find("CombatUI").Find("Host").gameObject;
        spawned_projectiles = new List<GameObject>();
        projectile_direction = new List<Vector3>();
        projectile_can_damage = new List<bool>();
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

    public void Attack() {
        player.TakeDamage(stats.attack);
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

    private void SpawnProjectiles()
    {
        // Common spawn code
        spawned_projectiles.Add(Instantiate(projectiles[current_projectile].UI_object));
        spawned_projectiles[spawned_projectiles.Count - 1].transform.SetParent(ProjectilesPanel.transform, false);
        spawned_projectiles[spawned_projectiles.Count - 1].GetComponent<Hitbox>().SetEnemy(this);
        spawned_projectiles[spawned_projectiles.Count - 1].GetComponent<Hitbox>().SetIndex(spawned_projectiles.Count - 1);
        aux_pos = spawned_projectiles[spawned_projectiles.Count - 1].transform.localPosition;
        projectile_direction.Add((host.transform.localPosition - aux_pos).normalized);
        projectile_can_damage.Add(false);

        // Type specific code
        switch (projectiles[current_projectile].type) 
        {
            case ProjectileType.DROP:
                spawned_projectiles[spawned_projectiles.Count-1].transform.localPosition = 
                    new Vector3(aux_pos.x + Random.value*10f, aux_pos.y + Random.value * 10f, aux_pos.z);
                break;

            case ProjectileType.FACES:
                spawned_projectiles[spawned_projectiles.Count - 1].transform.localPosition =
                    new Vector3(aux_pos.x + Random.value * 138.3f, aux_pos.y, aux_pos.z);
                break;

            case ProjectileType.PUNCH:
                spawned_projectiles[spawned_projectiles.Count - 1].transform.localPosition =
                    new Vector3(aux_pos.x + Random.value * 10f, aux_pos.y + Random.value * 10f, aux_pos.z);
                break;
        }
    }

    public bool CanHit(int index) {
        return projectile_can_damage[index];
    }

    private void ThrowProjectiles()
    {
        switch (projectiles[current_projectile].movement)
        {
            case ProjectileMovement.DIRECT:
                for (int i = 0; i < spawned_projectiles.Count; ++i) {
                    spawned_projectiles[i].transform.localPosition += projectile_direction[i] * Time.deltaTime
                        * projectiles[current_projectile].speed;
                    projectile_can_damage[i] = true;
                }
                break;

            case ProjectileMovement.FALL:
                for (int i = 0; i < spawned_projectiles.Count; ++i) {
                    spawned_projectiles[i].transform.localPosition += Vector3.down * Time.deltaTime * projectiles[current_projectile].speed;
                    projectile_can_damage[i] = true;
                }
                break;
        }
    }

    private void ScriptAnimation()
    {
        if (player.current_state == TurnState.ATTACKING || player.current_state == TurnState.SELECTING)
            timer_length = Time.time + projectiles[current_projectile].length;

        switch (player.current_state)
        {
            case TurnState.AVOIDING:
                // Spawn a projectile every [interval] seconds
                if (timer_interval <= Time.time) {
                    SpawnProjectiles();
                    timer_interval = Time.time + projectiles[current_projectile].interval;
                }

                // Throw current projectiles
                ThrowProjectiles();

                // Finish Avoid state
                if (timer_length <= Time.time) {
                    player.current_state = TurnState.TRANSITION_TO_SELECT;
                    foreach (GameObject spawned_projectile in spawned_projectiles)
                        Destroy(spawned_projectile);
                    spawned_projectiles.Clear();
                    projectile_direction.Clear();
                    current_projectile = (current_projectile + 1) % projectiles.Count;
                }
                break;

            case TurnState.ATTACKING:
                AttackAnimation();
                if (Vector3.Distance(health_bar.transform.localScale, target_health_scale) <= depletion_transition_threshold)
                {
                    health_bar.transform.localScale = target_health_scale;
                    player.current_state = TurnState.TRANSITION_TO_AVOID;
                    cut_animation.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    cut_animation.SetActive(false);
                    timer_interval = 0f;
                }
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

    // Update is called once per frame
    void Update()
    {
        if (player.GetEnemy() == this) {
            UIStateManagement();
            ScriptAnimation();
        }
    }
}
