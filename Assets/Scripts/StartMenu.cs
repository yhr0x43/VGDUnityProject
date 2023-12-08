using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void StartGame()
    {
        StartCoroutine(LoadForestLevel());
    }

    IEnumerator LoadForestLevel()
    {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("ForestLevel");

        while(!loadLevel.isDone)
        {
            yield return null;
        }
    }
}
