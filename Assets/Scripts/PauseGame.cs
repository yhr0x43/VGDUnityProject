using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static bool isGamePaused;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
    }

    public static void PauseToggle()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public static void Pause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public static void Unpause()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
}
