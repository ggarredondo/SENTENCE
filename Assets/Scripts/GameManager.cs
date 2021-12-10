using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject UI, player, enemy;
    public Vector3 easy_dif, normal_dif, hard_dif;
    Vector3 current_dif;
    Button easy_button, normal_button, hard_button;

    // Start is called before the first frame update
    private void Awake() {
        DontDestroyOnLoad(this);
        current_dif = normal_dif;
        easy_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("EasyButton").GetComponent<Button>();
        normal_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("NormalButton").GetComponent<Button>();
        hard_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("HardButton").GetComponent<Button>();
    }

    private void DifficultyUpdate() {
        if (player.GetComponent<PlayerCombat>().current_state == TurnState.TRANSITION_TO_FIGHT) {
            enemy.GetComponent<EnemyCombat>().stats.health = current_dif.x;
            enemy.GetComponent<EnemyCombat>().stats.max_health = current_dif.y;
            enemy.GetComponent<EnemyCombat>().stats.attack = current_dif.z;
        }
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
