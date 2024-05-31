using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private const int MAIN_MENU_SCENE = 0;
    private const int GAME_SCENE = 1;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(Instance);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }


    public void LoadGameWithMode(GameMode gameMode)
    {
        StartCoroutine(LoadGameWithModeCouroutine(gameMode));
    }


    private IEnumerator LoadGameWithModeCouroutine(GameMode gameMode)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GAME_SCENE);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameManager.Instance.NewGame(gameMode);
    }
}
