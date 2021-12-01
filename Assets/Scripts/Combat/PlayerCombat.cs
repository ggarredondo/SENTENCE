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
    DEFENDING,
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
    public float rotation_speed = -200f, selecting_scale = 0.3f, transition_speed = 4f, transition_time = 1f, fade_speed = 0.65f,
        fade_threshold = 0.005f, depletion_threshold = 1f, attack_multiplier, max_attack_multiplier = 2f, defense_multiplier, 
        max_defense_multiplier = 2f, target_health;
    public int switch_max = 1;

    private GameObject AlterSystemUI, CombatUI, ActionMenu, MagicMenu, AvoidPanel, FadePanel, SelectionArrow, health_bar, mana_bar, shield;
    private Button MagicButton, SwitchButton;
    private List<Button> SkillButtons;
    private Image health_forebar, mana_forebar, host;
    private Color health_forebar_color, health_forebar_regenerating;
    private Text mana_text;
    private List<GameObject> AlterImages;
    private Vector3 host_initial_pos, avoid_initial_pos, avoid_initial_scale, avoid_select_scale, target_mana_scale;
    private float timer;
    private TransitionPhase current_phase = TransitionPhase.FIRST_PHASE;
    private int current_alter = 0, switch_number, switch_counter = 0, off_screen_alter;
    private bool ActivateAlterUI, ActivateActionMenu, defending = false, magic_interactable, toggle_magic_menu = false, regenerating;
    private Quaternion alterUI_target_angle;

    private void Start()
    {
        CombatUI = UI.transform.Find("CombatUI").gameObject;
        AlterSystemUI = CombatUI.transform.Find("AlterSystemUI").gameObject;
        ActionMenu = CombatUI.transform.Find("ActionMenu").gameObject;
        MagicMenu = ActionMenu.transform.Find("MagicMenu").gameObject;
        AvoidPanel = CombatUI.transform.Find("AvoidPanel").gameObject;
        FadePanel = UI.transform.Find("Fade").Find("FadePanel").gameObject;
        health_bar = CombatUI.transform.Find("PlayerHealthBar").gameObject;
        health_forebar = health_bar.transform.Find("ProgressForeground").GetComponent<Image>();
        health_forebar_color = health_forebar.color;
        health_forebar_regenerating = new Color(0.5f, 1f, 0f, 1f);
        mana_bar = CombatUI.transform.Find("PlayerManaBar").gameObject;
        mana_forebar = mana_bar.transform.Find("ProgressForeground").GetComponent<Image>();
        mana_text = mana_bar.transform.Find("ManaText").GetComponent<Text>();
        mana_text.text = stats.mana.ToString() + "\n/\n" + stats.max_mana.ToString();
        host = CombatUI.transform.Find("Host").GetComponent<Image>();
        host_initial_pos = host.transform.localPosition;
        avoid_initial_pos = AvoidPanel.transform.localPosition;
        avoid_initial_scale = AvoidPanel.transform.localScale;
        avoid_select_scale = new Vector3(selecting_scale, selecting_scale, 1f);
        off_screen_alter = 2;
        MagicButton = ActionMenu.transform.Find("MagicButton").GetComponent<Button>();
        SwitchButton = ActionMenu.transform.Find("SwitchButton").GetComponent<Button>();
        SkillButtons = new List<Button>();
        SkillButtons.Add(MagicMenu.transform.Find("Skill1Button").GetComponent<Button>());
        SkillButtons.Add(MagicMenu.transform.Find("Skill2Button").GetComponent<Button>());
        SkillButtons.Add(MagicMenu.transform.Find("Skill3Button").GetComponent<Button>());
        SkillButtons.Add(MagicMenu.transform.Find("Skill4Button").GetComponent<Button>());
        AlterImages = new List<GameObject>();
        AlterImages.Add(AlterSystemUI.transform.Find("Alter1").gameObject);
        AlterImages.Add(AlterSystemUI.transform.Find("Alter2").gameObject);
        AlterImages.Add(AlterSystemUI.transform.Find("Alter3").gameObject);
        AlterImages.Add(AlterSystemUI.transform.Find("Alter4").gameObject);
        SelectionArrow = CombatUI.transform.Find("SelectionArrow").gameObject;
        shield = host.transform.Find("DefendingShield").gameObject;
        shield.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    private void AlterUISpriteUpdate()
    {
        AlterImages[0].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[0].sprite;
        if (stats.system.Count >= 2)
            AlterImages[1].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[1].sprite;
        else
            AlterImages[1].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        if (stats.system.Count >= 3)
            AlterImages[2].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[2].sprite;
        else
            AlterImages[2].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        if (stats.system.Count >= 4)
            AlterImages[3].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().sprite = stats.system[3].sprite;
        else
            AlterImages[3].transform.Find("SpriteContainer").Find("Sprite").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    private void MagicMenuUpdate()
    {
        magic_interactable = false;
        for (short i = 0; i < SkillButtons.Count && !magic_interactable; ++i)
            magic_interactable = !(stats.system[current_alter].skills[i].name == "----");
        MagicButton.interactable = magic_interactable;
        for (short i = 0; i < SkillButtons.Count; ++i) {
            SkillButtons[i].transform.Find("SkillText").GetComponent<Text>().text = stats.system[current_alter].skills[i].name;
            if (stats.system[current_alter].skills[i].name != "----")
                SkillButtons[i].transform.Find("SkillCost").GetComponent<Text>().text = '\n' +
                    stats.system[current_alter].skills[i].cost.ToString();
            else
                SkillButtons[i].transform.Find("SkillCost").GetComponent<Text>().text = "";
        }
    }

    private void UIStateManagement()
    {
        CombatUI.SetActive(current_state != TurnState.WAITING && ActivateCombatUI);
        FadePanel.SetActive(current_state == TurnState.TRANSITION_TO_FIGHT || current_state == TurnState.TRANSITION_TO_ENEMYS_DEATH);
        ActivateAlterUI = current_state != TurnState.AVOIDING && current_state != TurnState.TRANSITION_TO_AVOID
            && current_state != TurnState.TRANSITION_TO_SELECT;
        AlterSystemUI.SetActive(ActivateAlterUI);
        SelectionArrow.SetActive(ActivateAlterUI);
        AlterImages[off_screen_alter].SetActive(current_state == TurnState.SWITCHING);
        ActivateActionMenu = current_state == TurnState.SELECTING;
        ActionMenu.SetActive(ActivateActionMenu);
        if (!ActivateActionMenu)
            toggle_magic_menu = false;
        mana_bar.SetActive(current_state == TurnState.SELECTING || current_state == TurnState.DEFENDING);
        health_bar.SetActive(current_state == TurnState.AVOIDING);
        SwitchButton.interactable = switch_counter < switch_max && stats.system.Count > 1;
        MagicMenu.SetActive(toggle_magic_menu);
        for (short i = 0; i < SkillButtons.Count; ++i)
            SkillButtons[i].interactable = !(stats.system[current_alter].skills[i].name == "----"
                || stats.system[current_alter].skills[i].name == "Switch" || stats.system[current_alter].skills[i].cost > stats.mana);

        // update health bar
        health_forebar.transform.localScale = new Vector3(stats.health / stats.max_health, 1f, 1f);
        regenerating = target_health > stats.health;
        if (target_health < 0f)
            target_health = 0f;
        else if (target_health > stats.max_health)
            target_health = stats.max_health;
        if (stats.health <= 0) // temp
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // temp
    }

    private void UpdateManaBar() {
        mana_forebar.transform.localScale = Vector3.Lerp(mana_forebar.transform.localScale, target_mana_scale,
                    Time.deltaTime * 2f);
        mana_text.text = (Mathf.Round(mana_forebar.transform.localScale.y * stats.max_mana)).ToString() + "\n/\n"
            + stats.max_mana.ToString();
        if (Vector3.Distance(mana_forebar.transform.localScale, target_mana_scale) <= fade_threshold)
        {
            mana_forebar.transform.localScale = target_mana_scale;
            mana_text.text = stats.mana.ToString() + "\n/\n" + stats.max_mana.ToString();
            current_state = TurnState.TRANSITION_TO_AVOID;
        }
    }

    private void FadeImage(Image img, Color new_color, float fade_speed, float threshold) {
        img.color = Color.Lerp(img.color, new_color, Time.deltaTime * fade_speed);
        if (Vector4.Distance(new Vector4(img.color.r, img.color.g, img.color.b, img.color.a),
            new Vector4(new_color.r, new_color.g, new_color.g, new_color.a)) <= threshold)
            img.color = new_color;
    }

    private void ScriptAnimation()
    {
        if (current_state == TurnState.SELECTING || current_state == TurnState.ATTACKING 
            || current_state == TurnState.TRANSITION_TO_ENEMYS_DEATH || current_state == TurnState.DEFENDING)
        {
            AvoidPanel.transform.Rotate(0f, 0f, Time.deltaTime * rotation_speed);
            timer = Time.time + transition_time;
        }

        if (defending)
            FadeImage(shield.GetComponent<Image>(), new Color(1f, 1f, 1f, 0.5f), transition_speed, fade_threshold);
        else
            FadeImage(shield.GetComponent<Image>(), new Color(1f, 1f, 1f, 0f), transition_speed, fade_threshold);

        if (regenerating)
            FadeImage(health_forebar, health_forebar_regenerating, transition_speed, fade_threshold);
        else
            FadeImage(health_forebar, health_forebar_color, transition_speed, fade_threshold);

        switch (current_state)
        {
            case TurnState.TRANSITION_TO_FIGHT:
                switch (current_phase)
                {
                    case TransitionPhase.FIRST_PHASE:
                        AlterUISpriteUpdate();
                        MagicMenuUpdate();
                        AvoidPanel.transform.localPosition = host_initial_pos;
                        AvoidPanel.transform.localScale = avoid_select_scale;
                        attack_multiplier = 1f;
                        defense_multiplier = 1f;
                        target_health = stats.health;
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
                stats.health = Mathf.Lerp(stats.health, target_health, Time.deltaTime * transition_speed);
                if (Mathf.Abs(target_health - stats.health) <= depletion_threshold)
                    stats.health = target_health;
                timer = Time.time + transition_time;
                break;

            case TurnState.TRANSITION_TO_SELECT:
                defending = false;

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
                    switch_counter = 0;
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

            case TurnState.DEFENDING:
                UpdateManaBar();
                break;

            case TurnState.SWITCHING:
                AvoidPanel.transform.Rotate(0f, 0f, Time.deltaTime * rotation_speed);

                AlterSystemUI.transform.localRotation = Quaternion.Lerp(AlterSystemUI.transform.localRotation, 
                    alterUI_target_angle, Time.deltaTime * transition_speed);
                foreach (GameObject AlterImage in AlterImages)
                    AlterImage.transform.localRotation = Quaternion.Euler(-AlterSystemUI.transform.localRotation.eulerAngles);
                if (timer <= Time.time)
                {
                    AlterSystemUI.transform.localRotation = alterUI_target_angle;
                    foreach (GameObject AlterImage in AlterImages)
                        AlterImage.transform.localRotation = Quaternion.Euler(0f, 0f, -alterUI_target_angle.eulerAngles.z);
                    off_screen_alter = (off_screen_alter + switch_number) % 4;
                    current_state = TurnState.SELECTING;
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
        enemy.TakeDamage(stats.system[current_alter].attack * attack_multiplier);
    }

    public void Magic() {
        toggle_magic_menu = !toggle_magic_menu;
    }

    private void Skill(MagicSkill mskill) {
        switch (mskill.name)
        {
            case "Heal":
                Debug.Log("Heal");
                break;
            case "Sharpen":
                Debug.Log("Sharpen");
                break;
            case "Harden":
                Debug.Log("Harden");
                break;
        }
    }

    public void Skill1() {
        Skill(stats.system[current_alter].skills[0]);
    }

    public void Skill2() {
        Skill(stats.system[current_alter].skills[1]);
    }

    public void Skill3() {
        Skill(stats.system[current_alter].skills[2]);
    }

    public void Skill4() {
        Skill(stats.system[current_alter].skills[3]);
    }

    public void Defend()
    {
        defending = true;
        stats.mana += stats.mana_regen;
        if (stats.mana > stats.max_mana)
            stats.mana = stats.max_mana;
        target_mana_scale = new Vector3(1f, stats.mana / stats.max_mana, 1f);
        current_state = TurnState.DEFENDING;
    }

    public void Switch() {
        ++switch_counter;
        for (switch_number = 1; AlterImages[(current_alter + switch_number) % 4].transform.Find("SpriteContainer")
            .Find("Sprite").GetComponent<Image>().color.a == 0f; ++switch_number)
        { }
        alterUI_target_angle = Quaternion.Euler(new Vector3(0f, 0f, 90f * switch_number) + AlterSystemUI.transform.localRotation.eulerAngles);
        current_alter = (current_alter + switch_number) % 4;
        MagicMenuUpdate();
        current_state = TurnState.SWITCHING;
    }

    public void TakeDamage(float damage)
    {
        target_health = stats.health - Mathf.Round(damage - damage * stats.system[current_alter].resilience * defense_multiplier * 0.01f 
            * System.Convert.ToSingle(defending));
    }

    // Update is called once per frame
    void Update()
    {
        UIStateManagement();
        ScriptAnimation();
    }
}
