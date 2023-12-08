using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BottleManager : MonoBehaviour
{
    private int numBottles;
    private PauseGame pauseGame;

    public GameObject victoryScreen;
    public Button restartButton;
    public Button quitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        numBottles = 0;  
        restartButton.onClick.AddListener(RestartGame); 
        quitButton.onClick.AddListener(QuitGame); 
        pauseGame = GetComponent<PauseGame>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 60 * Time.deltaTime, 0);

        if(numBottles == 8)
        {
            victoryScreen.SetActive(true);
            Time.timeScale = 0f;
            pauseGame.isGamePaused = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameObject.FindWithTag("Player"))
        {
            numBottles++;
        }
    }

    void RestartGame()
    {
        RestartEverything();
    }

    void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator RestartEverything()
    {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("Start Menu");

        while(!loadLevel.isDone)
        {
            yield return null;
        }
    }
}
