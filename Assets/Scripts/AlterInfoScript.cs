using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterInfoScript : MonoBehaviour
{
    public PlayerCombat player_combat;
    GameObject SystemMenu, Info;
    Text NameSpace, AgeSpace, GenderSpace, TypeSpace, AttackSpace, ResilienceSpace;
    Image sprite;
    List<Alter> system;

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

    private void ShowInfo(string name) 
    {
        Info.SetActive(true);
        for (int i = 0; i < system.Count; ++i) { 
            if (system[i].name == name)
            {
                sprite.sprite = system[i].sprite;

                NameSpace.text = system[i].name;
                AgeSpace.text = system[i].age;
                GenderSpace.text = system[i].gender;

                TypeSpace.text = system[i].type.ToString();
                AttackSpace.text = system[i].attack.ToString();
                ResilienceSpace.text = system[i].resilience.ToString();
                break;
            }
        }
    }

    public void Alter1Button() { ShowInfo(system[0].name); }
    public void Alter2Button() { ShowInfo(system[1].name); }
    public void Alter3Button() { ShowInfo(system[2].name); }
    public void Alter4Button() { ShowInfo(system[3].name); }


    void Start() {
        SystemMenu = this.transform.Find("SystemMenu").gameObject;
        Info = SystemMenu.transform.Find("Info").gameObject;
        Info.SetActive(false);
        system = player_combat.stats.system;
        sprite = Info.transform.Find("Sprite").Find("Sprite").GetComponent<Image>();

        NameSpace = Info.transform.Find("Name").Find("Space").GetComponent<Text>();
        AgeSpace = Info.transform.Find("Age").Find("Space").GetComponent<Text>();
        GenderSpace = Info.transform.Find("Gender").Find("Space").GetComponent<Text>();

        TypeSpace = Info.transform.Find("Type").Find("Space").GetComponent<Text>();
        AttackSpace = Info.transform.Find("Attack").Find("Space").GetComponent<Text>();
        ResilienceSpace = Info.transform.Find("Resilience").Find("Space").GetComponent<Text>();

        UpdateSystemNames();
    }
}
