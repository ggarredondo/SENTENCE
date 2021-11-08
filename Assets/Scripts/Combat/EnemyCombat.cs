using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public EnemyStats stats;
    public SpriteRenderer sprite_renderer;
    public GameObject UI;
    public GameObject player;
    public TurnState current_state = TurnState.DEAD;

    // Start is called before the first frame update
    void Start() {
        UI.transform.Find("CombatUI").Find("EnemySprite").GetComponent<UnityEngine.UI.Image>().sprite = sprite_renderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
