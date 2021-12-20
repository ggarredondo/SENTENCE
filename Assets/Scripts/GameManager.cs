using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public GameObject UI, player, enemy;
    public AlterInfoScript alter_info;
    public Vector3 easiest_dif, easy_dif, normal_dif, hard_dif, current_dif;
    public List<Alter> new_alters;
    public int fst_threshold = 1, snd_threshold = 2, thrd_threshold = 3;
    public AudioSource music;
    public List<AudioSource> sfx;
    public Slider MusicSlider, SFXSlider;
    float music_volume = 0.2f, sfx_volume = 1f;
    PlayerCombat player_combat;
    Button easiest_button, easy_button, normal_button, hard_button;
    string path;
    int last_counter = 0, death_counter = 0;

    private void Load() {
        if (File.Exists(path)) {
            StreamReader reader = new StreamReader(path);
            int.TryParse(reader.ReadToEnd(), out death_counter);
        }
    }

    private void Save() {
        StreamWriter writer = new StreamWriter(path);
        writer.Write(instance.death_counter);
        writer.Close();
    }

    private void UpdateAllSound() {
        music.volume = music_volume;
        MusicSlider.value = music_volume;
        UpdateSFX();
        SFXSlider.value = sfx_volume;
    }

    private void UpdateSFX() {
        foreach (AudioSource a in sfx)
            a.volume = sfx_volume;
    }

    private void Awake() {
        path = Application.persistentDataPath + "/DIDRPG.dat";
        current_dif = normal_dif;
        easiest_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("EasiestButton").GetComponent<Button>();
        easy_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("EasyButton").GetComponent<Button>();
        normal_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("NormalButton").GetComponent<Button>();
        hard_button = UI.transform.Find("Menu").Find("OptionsMenu").Find("DifficultyMenu").Find("HardButton").GetComponent<Button>();
        player_combat = player.GetComponent<PlayerCombat>();

        if (instance == null)
        {
            instance = this;
            player_combat.stats.system[0].name = System.Environment.UserName;
            UpdateAllSound();
            DontDestroyOnLoad(this.gameObject);
            Load();
        }
        else
        {
            instance.death_counter++;
            Save();
            instance.UI = this.UI;
            instance.alter_info = this.alter_info;
            instance.player = this.player;
            instance.player.GetComponent<PlayerInput>().InitialShowControls(false);
            instance.player_combat = this.player.GetComponent<PlayerCombat>();
            instance.enemy = this.enemy;
            instance.easiest_button = this.easiest_button;
            instance.easiest_button.onClick.AddListener(instance.EasiestButton);
            instance.easy_button = this.easy_button;
            instance.easy_button.onClick.AddListener(instance.EasyButton);
            instance.normal_button = this.normal_button;
            instance.normal_button.onClick.AddListener(instance.NormalButton);
            instance.hard_button = this.hard_button;
            instance.hard_button.onClick.AddListener(instance.HardButton);
            instance.music = this.music;
            instance.sfx = this.sfx;
            instance.MusicSlider = this.MusicSlider;
            instance.MusicSlider.onValueChanged.AddListener(instance.MusicVolume);
            instance.SFXSlider = this.SFXSlider;
            instance.SFXSlider.onValueChanged.AddListener(instance.SFXVolume);
            instance.UpdateAllSound();
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

    public void MusicVolume(float new_volume) {
        music_volume = new_volume;
        music.volume = music_volume;
    }

    public void SFXVolume(float new_volume) {
        sfx_volume = new_volume;
        UpdateSFX();
    }

    private void UIStateManagement() {
        easiest_button.interactable = !(current_dif == easiest_dif);
        easy_button.interactable = !(current_dif == easy_dif);
        normal_button.interactable = !(current_dif == normal_dif);
        hard_button.interactable = !(current_dif == hard_dif);
    }

    private void NextAlter() { player_combat.stats.system.Add(new_alters[player_combat.stats.system.Count - 1]); }

    private void Progress()
    {
        if (last_counter != death_counter)
        {
            if (death_counter >= fst_threshold && death_counter < snd_threshold)
                NextAlter();
            else if (death_counter >= snd_threshold && death_counter < thrd_threshold) {
                NextAlter();
                NextAlter();
            }
            else if (death_counter >= thrd_threshold) {
                NextAlter();
                NextAlter();
                NextAlter();
            }
            alter_info.UpdateSystemNames();
            last_counter = death_counter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DifficultyUpdate();
        UIStateManagement();
        Progress();
    }
}
