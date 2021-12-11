using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterInfoScript : MonoBehaviour
{
    public PlayerCombat player_combat;
    GameObject SystemMenu, Info;

    private void UpdateSystemNames()
    {
        SystemMenu.transform.Find("Alter1").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[0].name;
        if (player_combat.stats.system.Count > 1)
            SystemMenu.transform.Find("Alter2").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[1].name;
        if (player_combat.stats.system.Count > 2)
            SystemMenu.transform.Find("Alter3").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[2].name;
        if (player_combat.stats.system.Count > 3)
            SystemMenu.transform.Find("Alter4").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[3].name;
    }

    // Start is called before the first frame update
    void Start() {
        SystemMenu = this.transform.Find("SystemMenu").gameObject;
        Info = this.transform.Find("Info").gameObject;
        Info.SetActive(false);
        UpdateSystemNames();
    }
}
