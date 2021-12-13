using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlterInfoScript : MonoBehaviour
{
    public PlayerCombat player_combat;
    GameObject SystemMenu, Info;
    Transform Magic1, Magic2, Magic3, Magic4;
    Text NameSpace, AgeSpace, GenderSpace, TypeSpace, AttackSpace, ResilienceSpace, QuoteSpace;
    Image sprite;
    Button Alter1, Alter2, Alter3, Alter4;
    List<Alter> system;

    public void UpdateSystemNames()
    {
        SystemMenu.transform.Find("Alter1").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[0].name;
        if (player_combat.stats.system.Count > 1)
            SystemMenu.transform.Find("Alter2").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[1].name;
        if (player_combat.stats.system.Count > 2)
            SystemMenu.transform.Find("Alter3").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[2].name;
        if (player_combat.stats.system.Count > 3)
            SystemMenu.transform.Find("Alter4").Find("AlterName").GetComponent<Text>().text = player_combat.stats.system[3].name;
        UIStateManagement();
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

                QuoteSpace.text = system[i].quote;

                Magic1.Find("Text").GetComponent<Text>().text = system[i].skills[0].name;
                Magic1.Find("Desc").GetComponent<Text>().text = system[i].skills[0].desc;

                Magic2.Find("Text").GetComponent<Text>().text = system[i].skills[1].name;
                Magic2.Find("Desc").GetComponent<Text>().text = system[i].skills[1].desc;

                Magic3.Find("Text").GetComponent<Text>().text = system[i].skills[2].name;
                Magic3.Find("Desc").GetComponent<Text>().text = system[i].skills[2].desc;

                Magic4.Find("Text").GetComponent<Text>().text = system[i].skills[3].name;
                Magic4.Find("Desc").GetComponent<Text>().text = system[i].skills[3].desc;

                break;
            }
        }
    }

    public void Alter1Button() { ShowInfo(system[0].name); }
    public void Alter2Button() { if (player_combat.stats.system.Count > 1) ShowInfo(system[1].name); }
    public void Alter3Button() { if (player_combat.stats.system.Count > 2) ShowInfo(system[2].name); }
    public void Alter4Button() { if (player_combat.stats.system.Count > 3) ShowInfo(system[3].name); }

    private void UIStateManagement() {
        Alter1.interactable = Alter1.transform.Find("AlterName").GetComponent<Text>().text != "----";
        Alter2.interactable = Alter2.transform.Find("AlterName").GetComponent<Text>().text != "----";
        Alter3.interactable = Alter3.transform.Find("AlterName").GetComponent<Text>().text != "----";
        Alter4.interactable = Alter4.transform.Find("AlterName").GetComponent<Text>().text != "----";
    }

    void Start() {
        SystemMenu = this.transform.Find("SystemMenu").gameObject;
        Alter1 = SystemMenu.transform.Find("Alter1").GetComponent<Button>();
        Alter2 = SystemMenu.transform.Find("Alter2").GetComponent<Button>();
        Alter3 = SystemMenu.transform.Find("Alter3").GetComponent<Button>();
        Alter4 = SystemMenu.transform.Find("Alter4").GetComponent<Button>();

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

        QuoteSpace = Info.transform.Find("Quote").Find("Space").GetComponent<Text>();

        Magic1 = Info.transform.Find("Magic").Find("Magic1");
        Magic2 = Info.transform.Find("Magic").Find("Magic2");
        Magic3 = Info.transform.Find("Magic").Find("Magic3");
        Magic4 = Info.transform.Find("Magic").Find("Magic4");

        UpdateSystemNames();
    }
}
