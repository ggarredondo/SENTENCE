using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    PlayerController controller;
    public bool is_fighting = false;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!is_fighting)
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
