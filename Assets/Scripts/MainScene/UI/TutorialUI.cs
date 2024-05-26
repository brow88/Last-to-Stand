using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;

        HandleTutorialState();
    }

    private void GameManager_OnGameStateChange(object sender, System.EventArgs e)
    {
        HandleTutorialState();
    }

    private void HandleTutorialState()
    {
        if (GameManager.Instance.IsTutorial())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
