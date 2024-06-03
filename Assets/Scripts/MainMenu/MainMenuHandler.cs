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
    //[SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject fadeOutPanel;

    [Header("Main Menu")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button leaderBoardButton;
    //[SerializeField] private Button optionsButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button quitButton;

    [Header("Leader Board")]
    [SerializeField] private Button leaderBoardReturnButton;

    //[Header("Options")]
    //[SerializeField] private Button optionsReturnButton;

    [Header("Credits")]
    [SerializeField] private Button creditsReturnButton;

    [Header("Fade Out")]
    private Image fadeOutImage;
    [SerializeField] private float fadeOutDuration = 1;
    [SerializeField] private float fadeOutImgAlpha = 100f;

    [Header("Mouse Cursor")]
    [SerializeField] private Texture2D cursorTexture;



    private void Awake()
    {
        fadeOutImage = fadeOutPanel.GetComponent<Image>();
    }


    private void Start()
    {
        SetMouseCursor();

        ButtonsListeners();

        MainMenu();

        StartCoroutine(FadeInOut());
        
        Time.timeScale = 1f; // in case we want to add pause menu in game and need to set timeScale to 0
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            gameObject.PlaySound(SoundManager.Instance.FindClip("bang"));
    }

    private void SetMouseCursor()
    {
        Cursor.SetCursor(cursorTexture, new Vector2(20,20), CursorMode.ForceSoftware);
    }

    #region Buttons

    private void ButtonsListeners()
    {
        //Ensures all the listeners are active in scene (the appropriate ones are turned off by StartingGamePanelsSetup() )
        mainMenuPanel.SetActive(true);
        leaderBoardPanel.SetActive(true);
        //optionsPanel.SetActive(true);
        creditPanel.SetActive(true);

        //Main menu
        singlePlayerButton.onClick.AddListener(StartSinglePlayerMode);
        multiplayerButton.onClick.AddListener(StartMultiPlayerMode);
        leaderBoardButton.onClick.AddListener(LeaderBoard);
        //optionsButton.onClick.AddListener(Options);
        creditButton.onClick.AddListener(Credits);
        quitButton.onClick.AddListener(QuitGame);

        //Leader board
        leaderBoardReturnButton.onClick.AddListener(MainMenu);

        //Options
        //optionsReturnButton.onClick.AddListener(MainMenu);

        //credits
        creditsReturnButton.onClick.AddListener(MainMenu);
    }


    private void StartSinglePlayerMode()
    {
        SceneTransitionManager.Instance.LoadGameWithMode(GameMode.SinglePlayer);
    }

    private void StartMultiPlayerMode()
    {
        SceneTransitionManager.Instance.LoadGameWithMode(GameMode.Multiplayer);
    }


    private void MainMenu()
    {
        mainMenuPanel.SetActive(true);
        leaderBoardPanel.SetActive(false);
        //optionsPanel.SetActive(false);
        creditPanel.SetActive(false);
    }


    private void LeaderBoard()
    {
        mainMenuPanel.SetActive(false);
        leaderBoardPanel.SetActive(true);
        //optionsPanel.SetActive(false);
        creditPanel.SetActive(false);

        leaderBoardPanel.GetComponent<LeaderBoard>().CreateLeaderBoard();
    }


    private void Options()
    {
        mainMenuPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        //optionsPanel.SetActive(true);
        creditPanel.SetActive(false);
    }


    private void Credits()
    {
        mainMenuPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        //optionsPanel.SetActive(false);
        creditPanel.SetActive(true);
    }


    private void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    #endregion


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
