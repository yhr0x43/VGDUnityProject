using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BottleManager : MonoBehaviour
{
    private PlayerControl bottleCount;
    private PauseGame pauseGame;

    public GameObject victoryScreen;
    public Button restartButton;
    public Button quitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(RestartGame); 
        quitButton.onClick.AddListener(QuitGame); 
        pauseGame = GetComponent<PauseGame>();
        bottleCount = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(bottleCount.numBottles == 8)
        {
            victoryScreen.SetActive(true);
            Time.timeScale = 0f;
            pauseGame.isGamePaused = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bottle")
        {
            Destroy(other.gameObject);
            bottleCount.numBottles++;
            Debug.Log(bottleCount.numBottles);
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
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("StartMenu");

        while(!loadLevel.isDone)
        {
            yield return null;
        }
    }
}
