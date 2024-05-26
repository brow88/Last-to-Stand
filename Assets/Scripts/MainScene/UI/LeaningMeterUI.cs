using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaningMeterUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;


    /// <summary>
    /// leaning range is between -1 and +1. 0 is perfectly balanced
    /// </summary>
    /// <param name="leaningAmount"></param>
    public void UpdateLeaningMeter(float leaningAmount)
    {
        if (leaningAmount>1 || leaningAmount <-1)
        {
            Debug.LogError("Leaning amount is " + leaningAmount + ". It should be between -1 and +1.");
        }

        fillImage.fillAmount = (leaningAmount + 1f) / 2f;
    }
}
