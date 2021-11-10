using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player, independant_collider;
    EnemyCombat enemy_combat;

    private void Start()
    {
        enemy_combat = GetComponent<EnemyCombat>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player.GetComponent<PlayerCombat>().current_state == TurnState.WAITING) {
            transform.LookAt(player.transform.position);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            independant_collider.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            if (Vector3.Distance(player.transform.position, this.transform.position) <= 1f)
            {
                player.GetComponent<PlayerCombat>().current_state = TurnState.SELECTING;
                player.GetComponent<PlayerCombat>().SetEnemy(enemy_combat);
            }
        }
    }
}
