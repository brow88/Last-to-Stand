using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("single player")]
    [SerializeField] private GameObject leaderBoard;

    [Header("multi-player")]
    [SerializeField] private Transform scoreContainer;
    [SerializeField] private GameObject scoreTextPrefab;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneTransitionManager.Instance.LoadMainMenu();
        });

        retryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RetryGame();
        });

        GameManager.Instance.OnGameStateChange += GameManager_OnGameStateChange;

        Hide();
    }

    private void GameManager_OnGameStateChange(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
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

        //Setups for Game over
        if (GameManager.Instance.IsGameModeSinglePlayer())
        {
            SinglePlayerGameOver();
        }
        else if (GameManager.Instance.IsGameModeMultiplyPlayer())
        {
            MultiplayerGameOver();
        }
    }

    private void SinglePlayerGameOver()
    {
        //setup right UI
        scoreContainer.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(true);

        int score = 0;
        foreach (var kvp in GameManager.Instance.GetPlayerScores())
        {
            score = kvp.Value;
        }

        leaderBoard.GetComponent<LeaderBoard>().CreateLeaderBoardWithNewEntry(new LeaderBoardEntry {Score = score});
    }

    private void MultiplayerGameOver()
    {
        //set up right UI
        scoreContainer.gameObject.SetActive(true);
        leaderBoard.gameObject.SetActive(false);

        //Empties any score results
        foreach (Transform child in scoreContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in GameManager.Instance.GetPlayerScores())
        {
            //Instantiate score text
            GameObject scoreText = Instantiate(scoreTextPrefab, scoreContainer);

            //Change the text for the score text
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + kvp.Value.ToString();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
