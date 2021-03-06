using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light_variable;
    public PlayerCombat player;
    AudioSource sfx;

    public float minTime;
    public float maxTime;
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
        sfx = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        FlickeringLight();
        sfx.mute = player.current_state != TurnState.WAITING;
    }

    void FlickeringLight()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            light_variable.enabled = !light_variable.enabled;
            timer = Random.Range(minTime, maxTime);
        }
    }
}
