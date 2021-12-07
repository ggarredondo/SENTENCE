using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController controller;
    PlayerCombat player_combat;
    public GameObject UI, Menu, OptionsMenu;
    bool toggle_menu = false, toggle_options = false;

    public void Exit() {
        Application.Quit();
    }

    public void ToggleOptionsMenu() {
        toggle_options = !toggle_options;
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
    }

    private void UIStateManagement() {
        if (Input.GetKeyDown(KeyCode.Escape))
            toggle_menu = !toggle_menu;
        toggle_menu = toggle_menu && player_combat.current_state == TurnState.WAITING;
        Menu.SetActive(toggle_menu);
        toggle_options = toggle_options && player_combat.current_state == TurnState.WAITING;
        OptionsMenu.SetActive(toggle_options);
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
