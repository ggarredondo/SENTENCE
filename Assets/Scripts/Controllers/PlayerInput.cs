using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController controller;
    PlayerCombat player_combat;
    public GameObject UI;
    GameObject Menu, SystemMenu, OptionsMenu, ControlsMenu, DifficultyMenu;
    bool toggle_menu = true, toggle_system = false, toggle_options = false, toggle_difficulty = false, toggle_controls = true;

    public void Exit() {
        Application.Quit();
    }

    public void ToggleSystemMenu() {
        toggle_system = !toggle_system;
        toggle_options = false;
        toggle_controls = false;
    }

    public void ToggleOptionsMenu() {
        toggle_options = !toggle_options;
        toggle_system = false;
        toggle_controls = false;
    }

    public void ToggleDifficultyMenu() {
        toggle_difficulty = !toggle_difficulty;
    }

    public void ToggleControlsMenu() {
        toggle_controls = !toggle_controls;
        toggle_system = false;
        toggle_options = false;
    }

    public void ToggleFullscreen() {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
        else
            Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
    }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        player_combat = GetComponent<PlayerCombat>();
        Menu = UI.transform.Find("Menu").gameObject;
        OptionsMenu = Menu.transform.Find("OptionsMenu").gameObject;
        DifficultyMenu = Menu.transform.Find("OptionsMenu").Find("DifficultyMenu").gameObject;
        SystemMenu = UI.transform.Find("AlterInfo").Find("SystemMenu").gameObject;
        ControlsMenu = Menu.transform.Find("ControlsMenu").gameObject;
    }

    private void UIStateManagement() {
        if (Input.GetKeyDown(KeyCode.Escape))
            toggle_menu = !toggle_menu;
        toggle_menu = toggle_menu && player_combat.current_state == TurnState.WAITING;
        Menu.SetActive(toggle_menu);
        toggle_options = toggle_options && player_combat.current_state == TurnState.WAITING && toggle_menu;
        OptionsMenu.SetActive(toggle_options);
        toggle_system = toggle_system && player_combat.current_state == TurnState.WAITING && toggle_menu;
        SystemMenu.SetActive(toggle_system);
        toggle_controls = toggle_controls && player_combat.current_state == TurnState.WAITING && toggle_menu;
        ControlsMenu.SetActive(toggle_controls);
        toggle_difficulty = toggle_difficulty && toggle_options;
        DifficultyMenu.SetActive(toggle_difficulty);
    }

    private void Update()
    {
        UIStateManagement();

        if (player_combat.current_state == TurnState.WAITING)
        {
            if (Input.GetButtonUp("Up")) controller.MoveForward();
            if (Input.GetButtonUp("Down")) controller.MoveBackward();
            if (Input.GetButtonUp("Left")) controller.MoveLeft();
            if (Input.GetButtonUp("Right")) controller.MoveRight();
            if (Input.GetButtonUp("TurnLeft")) controller.RotateLeft();
            if (Input.GetButtonUp("TurnRight")) controller.RotateRight();
        }
    }
}
