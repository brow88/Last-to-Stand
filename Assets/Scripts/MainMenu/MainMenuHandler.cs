using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject leaderBoardPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject fadeOutPanel;

    [Header("Main Menu")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaderBoardButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Leader Board")]
    [SerializeField] private Button leaderBoardReturnButton;

    [Header("Options")]
    [SerializeField] private Button optionsReturnButton;

    [Header("Fade Out")]
    private Image fadeOutImage;
    [SerializeField] private float fadeOutDuration = 1;
    [SerializeField] private float fadeOutImgAlpha = 100f;



    private void Awake()
    {
        fadeOutImage = fadeOutPanel.GetComponent<Image>();
    }


    private void Start()
    {
        ButtonsListeners();

        StartingGamePanelsSetup();

        StartCoroutine(FadeInOut());
        
        Time.timeScale = 1f; // in case we want to add pause menu in game and need to set timeScale to 0
    }

    #region Buttons

    private void ButtonsListeners()
    {
        //Ensures all the listeners are active in scene (the appropriate ones are turned off by StartingGamePanelsSetup() )
        mainMenuPanel.SetActive(true);
        leaderBoardPanel.SetActive(true);
        optionsPanel.SetActive(true);

        //Main menu
        startButton.onClick.AddListener(()=>
        {
            Debug.Log("Start");
            StartGame();
        });
        leaderBoardButton.onClick.AddListener(LeaderBoard);
        optionsButton.onClick.AddListener(Options);
        quitButton.onClick.AddListener(QuitGame);

        //Leader board
        leaderBoardReturnButton.onClick.AddListener(MainMenu);

        //Options
        optionsReturnButton.onClick.AddListener(MainMenu);
    }


    private void StartGame()
    {
        SceneTransitionManager.Instance.LoadGameWithMode(GameMode.SinglePlayer);
    }


    private void MainMenu()
    {
        mainMenuPanel.SetActive(true);
        leaderBoardPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }


    private void LeaderBoard()
    {
        mainMenuPanel.SetActive(false);
        leaderBoardPanel.SetActive(true);
        optionsPanel.SetActive(false);

        leaderBoardPanel.GetComponent<LeaderBoard>().CreateLeaderBoard();
    }


    private void Options()
    {
        mainMenuPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }


    private void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    #endregion


    private void StartingGamePanelsSetup()
    {
        mainMenuPanel.SetActive(true);
        leaderBoardPanel.SetActive(false);
        optionsPanel.SetActive(false);
        fadeOutPanel.SetActive(true);
    }


    private IEnumerator FadeInOut()
    {
        for (int i = 100; i > 0; i--)
        {
            yield return new WaitForSeconds(fadeOutDuration * 0.05f);
            fadeOutImgAlpha = i / 100f;
            fadeOutImage.color = new Color(0f, 0f, 0f, fadeOutImgAlpha);
        }
    }

}
