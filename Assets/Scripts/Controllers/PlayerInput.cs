using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController controller;
    PlayerCombat player_combat;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        player_combat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
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
