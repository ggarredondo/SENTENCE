using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    EnemyCombat enemy;
    int index;

    public void SetEnemy(EnemyCombat enemy) {
        this.enemy = enemy;
    }

    public void SetIndex(int index) {
        this.index = index;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (enemy.CanHit(index))
            enemy.Attack();
    }
}
