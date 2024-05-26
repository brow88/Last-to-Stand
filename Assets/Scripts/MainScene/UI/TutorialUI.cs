using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    private void GameManager_OnGameStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsTutorial())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
