using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    EnemyCombat enemy;

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        enemy.Attack();
    }
}
