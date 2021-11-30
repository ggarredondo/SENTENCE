using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light;

    public float minTime;
    public float maxTime;
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        FlickeringLight();   
    }

    void FlickeringLight()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            light.enabled = !light.enabled;
            timer = Random.Range(minTime, maxTime);
        }
    }
}
