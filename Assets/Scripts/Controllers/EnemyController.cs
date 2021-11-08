using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player, independant_collider;
    public EnemyCombat enemy;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player.GetComponent<PlayerInput>().is_fighting) {
            transform.LookAt(player.transform.position);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            independant_collider.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            if (Vector3.Distance(player.transform.position, this.transform.position) <= 1f)
                player.GetComponent<PlayerInput>().is_fighting = true;
        }
    }
}
