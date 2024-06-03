using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverReasonTextMesh;

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

        GameManager.Instance.OnGameStateChange += Instance_OnGameStateChange;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        
        Hide();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= Instance_OnGameStateChange;
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void Instance_OnGameStateChange(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGameOver())
        {
            Hide();
        }
    }

    private void GameManager_OnGameOver(object sender, LoseCondition loseCondition)
    {
        //Show
        gameObject.SetActive(true);

        //Setups for Game over
        switch (loseCondition)
        {
            case LoseCondition.Player1FellOver:
                if (GameManager.Instance.IsGameModeSinglePlayer())
                {
                    gameOverReasonTextMesh.text = "Oops you fell over";
                    SinglePlayerGameOver();
                }
                else
                {
                    MultiplayerGameOver();
                }
                break;
            case LoseCondition.Player1PassedOut:
                if (GameManager.Instance.IsGameModeSinglePlayer())
                {
                    gameOverReasonTextMesh.text = "You passed out";
                    SinglePlayerGameOver();
                }
                else
                {
                    MultiplayerGameOver();
                }
                break;
            case LoseCondition.Player2FellOver:
                MultiplayerGameOver();
                break;
            case LoseCondition.Player2PassedOut:
                MultiplayerGameOver();
                break;
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

        KeyValuePair<Player, int>? highestScorePlayer = null;
        foreach (var kvp in GameManager.Instance.GetPlayerScores())
        {
            //Instantiate score text
            GameObject scoreText = Instantiate(scoreTextPrefab, scoreContainer);

            //Change the text for the score text
            scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + kvp.Value.ToString();

            // Find the player with the highest score
            if (highestScorePlayer == null || kvp.Value > highestScorePlayer.Value.Value)
            {
                highestScorePlayer = kvp;
            }
        }

        if (highestScorePlayer.Value.Key.IsPlayerOne)
        {
            gameOverReasonTextMesh.text = "Player 1 wins";
        }
        else
        {
            gameOverReasonTextMesh.text = "Player 2 wins";
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
