using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject OnePlayerControls;
    [SerializeField] private GameObject TwoPlayerControls;

    private void Start()
    {
        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;
    }

    private void GameManager_OnGameStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsTutorial())
        {

            if (GameManager.Instance.IsGameModeSinglePlayer())
            {
                Show(GameMode.SinglePlayer);
            }
            else
            {
                Show(GameMode.Multiplayer);
            }
        }
        else
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    private void Show(GameMode gameMode)
    {
        gameObject.SetActive(true);

        switch (gameMode)
        {
            case GameMode.SinglePlayer:
                OnePlayerControls.SetActive(true);
                TwoPlayerControls.SetActive(false);
                break;
            case GameMode.Multiplayer:
                OnePlayerControls.SetActive(false);
                TwoPlayerControls.SetActive(true);
                break;
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
