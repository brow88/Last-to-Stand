using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Transform scoreContainer;
    [SerializeField] private GameObject scoreTextPrefab;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            //ToDo
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

        //Show player score
        foreach(Transform child in scoreContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(var kvp in GameManager.Instance.GetPlayerScores())
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
