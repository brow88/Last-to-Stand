using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject optionsPanel;
    [Header("Main Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [Header("Options")]
    [SerializeField] TextMeshProUGUI mainVolumeText;
    [SerializeField] public Slider mainVolumeSlider;
    [SerializeField] TextMeshProUGUI musicVolumeText;
    [SerializeField] public Slider musicVolumeSlider;
    [SerializeField] TextMeshProUGUI sfxVolumeText;
    [SerializeField] public Slider sfxVolumeSlider;
    [SerializeField] Button cancelButton;
    [SerializeField] Button confirmButton;
    [Header("Credits")]
    [SerializeField] Button creditsButton;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] TextMeshProUGUI creditsText;
    [SerializeField] Button exitCreditsButton;
    [SerializeField] float creditsTextStartPos;
    [SerializeField] float scrollSpeed = 10;
    [Header("Confirm Window")]
    [SerializeField] GameObject confirmWindow;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    private bool isOptionsPanelVisible = false;

    float startMainVolume;
    float startMusicVolume;
    float startSFXVolume;

    string mainVolumeTag = "mainVolume";
    string musicVolumeTag = "musicVolume";
    string sfxVolumeTag = "sfxVolume";

// AudioManager audioManager;

    void Start()
    {
        Time.timeScale = 1f;

     //   audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();

        optionsButton.onClick.AddListener(delegate { SwitchOptionsPanelVisible(); });
        startButton.onClick.AddListener(delegate { SceneManager.LoadScene(1);});
        quitButton.onClick.AddListener(delegate { ToggleExitWindow(); });

        cancelButton.onClick.AddListener(delegate { CancelChanges();});
        confirmButton.onClick.AddListener(delegate { ConfirmChanges(); });

        yesButton.onClick.AddListener(delegate { QuitGame(); });
        noButton.onClick.AddListener(delegate { ToggleExitWindow(); });

    //    mainVolumeSlider.onValueChanged.AddListener(delegate { ChangeAudioVolume(); });
    //    musicVolumeSlider.onValueChanged.AddListener(delegate { ChangeAudioVolume(); });
    //    sfxVolumeSlider.onValueChanged.AddListener(delegate { ChangeAudioVolume(); FMODEvents.Instance.OnButtonClick(); });

        creditsButton.onClick.AddListener(delegate {
            creditsScreen.SetActive(true);
            StartCoroutine(ScrollCredits());
            SwitchOptionsPanelVisible();
        });
        exitCreditsButton.onClick.AddListener(delegate { ExitCredits(); });

        mainVolumeSlider.value = PlayerPrefs.GetFloat(mainVolumeTag, 0.5f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(musicVolumeTag, 0.5f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxVolumeTag, 0.5f);

        //ChangeAudioVolume();

        optionsPanel.SetActive(false);
        creditsScreen.SetActive(false);
        confirmWindow.SetActive(false);
    }

    private void GetAudioVolume()
    {
        startMainVolume = mainVolumeSlider.value;
        startMusicVolume = musicVolumeSlider.value;
        startSFXVolume = sfxVolumeSlider.value;
    }
/*
    private void ChangeAudioVolume()
    {
        SetVolumeText();
        audioManager.masterVolume = mainVolumeSlider.value;
        audioManager.musicVolume = musicVolumeSlider.value;
        audioManager.sfxVolume = sfxVolumeSlider.value;
    }

    private void SetVolumeText()
    {
        mainVolumeText.text = "Main Volume " + (int)(mainVolumeSlider.value * 100);
        musicVolumeText.text = "Music " + (int)(musicVolumeSlider.value * 100);
        sfxVolumeText.text = "SFX " + (int)(sfxVolumeSlider.value * 100);
    }
*/
    private void SwitchOptionsPanelVisible()
    {
        isOptionsPanelVisible = !isOptionsPanelVisible;
        optionsPanel.SetActive(isOptionsPanelVisible);
        ToggleMainButtonsState(!isOptionsPanelVisible);
        GetAudioVolume();
        //FMODEvents.Instance.OnButtonClick();
    }

    private void ToggleMainButtonsState(bool state)
    {
        startButton.interactable = state;
        optionsButton.interactable = state;
        quitButton.interactable = state;
    }

    private void ConfirmChanges()
    {
        SwitchOptionsPanelVisible();
        PlayerPrefs.SetFloat(mainVolumeTag, mainVolumeSlider.value);
        PlayerPrefs.SetFloat(musicVolumeTag, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(sfxVolumeTag, sfxVolumeSlider.value);
    }

    private void CancelChanges()
    {
        mainVolumeSlider.value = startMainVolume;
        musicVolumeSlider.value = startMusicVolume;
        sfxVolumeSlider.value = startSFXVolume;
        SwitchOptionsPanelVisible();
    }

    private void ExitCredits()
    {
        StopAllCoroutines();
        creditsScreen.SetActive(false);
        creditsText.rectTransform.position = new Vector3(creditsText.rectTransform.position.x, creditsTextStartPos, 0);
    }

    IEnumerator ScrollCredits()
    {
        while (true)
        {
            float creditsYPos = creditsText.rectTransform.position.y;
            creditsYPos += Time.deltaTime * scrollSpeed;
            creditsText.rectTransform.position = new Vector3(creditsText.rectTransform.position.x, creditsYPos, 0);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ToggleExitWindow()
    {
        confirmWindow.SetActive(!confirmWindow.activeSelf);
        startButton.GetComponent<Button>().enabled = !startButton.GetComponent<Button>().enabled;
        optionsButton.GetComponent<Button>().enabled = !optionsButton.GetComponent<Button>().enabled;
        quitButton.GetComponent<Button>().enabled = !quitButton.GetComponent<Button>().enabled;
    }

    private void QuitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}
