using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTextMesh;
    [SerializeField] private Image vomitFillImage;

    private void Start()
    {
        ResetPlayerBarUI();
    }

    public void ResetPlayerBarUI()
    {
        scoreTextMesh.text = "Score: 0";
        vomitFillImage.fillAmount = 0;
    }

    public void UpdateScoreDisplay(int score)
    {
        scoreTextMesh.text = "Score: " + score.ToString();
    }

    public void UpdateVomitMeter(float NormalizedVomitValue)
    {
        vomitFillImage.fillAmount = NormalizedVomitValue;
    }
}
