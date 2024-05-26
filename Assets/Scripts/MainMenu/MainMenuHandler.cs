using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject buttonsPanel;
    [SerializeField] GameObject optionsPanel;

    [SerializeField] Button startButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;

    [SerializeField] GameObject fadeOutPanel;
    [Header("Options")]
    [SerializeField] Button returnButton;

    Image fadeOutImage;
    public float fadeOutDuration = 1;
    public float fadeOutImgAlpha = 100f;

    void Start()
    {
        fadeOutPanel.SetActive(true);
        optionsPanel.SetActive(false);
        fadeOutImage = fadeOutPanel.GetComponent<Image>();

        Time.timeScale = 1f; // in case we want to add pause menu in game and need to set timeScale to 0

        StartCoroutine(FadeInOut());

        startButton.onClick.AddListener(delegate { StartGame(); });
        optionsButton.onClick.AddListener(delegate { Options(); });
        quitButton.onClick.AddListener(delegate { QuitGame(); });
        returnButton.onClick.AddListener(delegate
        {
            optionsPanel.SetActive(false);
            buttonsPanel.SetActive(true);
        });

        buttonsPanel.SetActive(true);
    }

    void StartGame()
    {
        SceneManager.LoadScene(1);
               
    }

    IEnumerator FadeInOut()
    {
        for (int i = 100; i>0; i--)
        {
            yield return new WaitForSeconds(fadeOutDuration*0.05f);
            fadeOutImgAlpha = i / 100f;
            fadeOutImage.color = new Color(0f, 0f, 0f, fadeOutImgAlpha);
        }
    }

    void Options()
    {
        buttonsPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}
