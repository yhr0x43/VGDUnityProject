using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameObject.FindWithTag("Player") && SceneManager.GetActiveScene().name == "ForestLevel")
        {
            StartCoroutine(LoadMountainLevel());
        }
    }

    IEnumerator LoadMountainLevel()
    {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("MountainLevel");

        while(!loadLevel.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadPortCityLevel()
    {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("MountainLevel");

        while(!loadLevel.isDone)
        {
            yield return null;
        }
    }
}
