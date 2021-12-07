using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start() 
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
