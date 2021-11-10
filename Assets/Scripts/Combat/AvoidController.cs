using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidController : MonoBehaviour
{
    public GameObject player_object;
    public float speed = 1f;

    private PlayerCombat player_combat;
    private Vector3 mouse_position;

    private void Start()
    {
        player_combat = player_object.GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player_combat.current_state == TurnState.AVOIDING)
        {
            mouse_position = Input.mousePosition;
            transform.position = Vector2.Lerp(transform.position, mouse_position, Time.deltaTime * speed);
        }
    }
}
