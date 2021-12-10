using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public GameObject UI, player, enemy;
    public Vector3 easiest_dif, easy_dif, normal_dif, hard_dif, current_dif;
    Button easiest_button, easy_button, normal_button, hard_button;

    // Start is called before the first frame update
    private void Awake() {
        current_dif = normal_dif;
        easiest_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("EasiestButton").GetComponent<Button>();
        easy_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("EasyButton").GetComponent<Button>();
        normal_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("NormalButton").GetComponent<Button>();
        hard_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("HardButton").GetComponent<Button>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            instance.UI = this.UI;
            instance.player = this.player;
            instance.enemy = this.enemy;
            instance.easiest_button = this.easiest_button;
            instance.easiest_button.onClick.AddListener(instance.EasiestButton);
            instance.easy_button = this.easy_button;
            instance.easy_button.onClick.AddListener(instance.EasyButton);
            instance.normal_button = this.normal_button;
            instance.normal_button.onClick.AddListener(instance.NormalButton);
            instance.hard_button = this.hard_button;
            instance.hard_button.onClick.AddListener(instance.HardButton);
            Destroy(this.gameObject);
        }
    }

    private void DifficultyUpdate() {
        if (player)
        {
            if (player.GetComponent<PlayerCombat>().current_state == TurnState.TRANSITION_TO_FIGHT)
            {
                enemy.GetComponent<EnemyCombat>().stats.health = current_dif.x;
                enemy.GetComponent<EnemyCombat>().stats.max_health = current_dif.y;
                enemy.GetComponent<EnemyCombat>().stats.attack = current_dif.z;
            }
        }
    }

    public void EasiestButton() {
        current_dif = easiest_dif;
    }

    public void EasyButton() {
        current_dif = easy_dif;
    }

    public void NormalButton() {
        current_dif = normal_dif;
    }

    public void HardButton() {
        current_dif = hard_dif;
    }

    private void UIStateManagement() {
        easiest_button.interactable = !(current_dif == easiest_dif);
        easy_button.interactable = !(current_dif == easy_dif);
        normal_button.interactable = !(current_dif == normal_dif);
        hard_button.interactable = !(current_dif == hard_dif);
    }

    // Update is called once per frame
    void Update()
    {
        DifficultyUpdate();
        UIStateManagement();
    }
}
