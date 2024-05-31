using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaningMeterUI : MonoBehaviour
{
    [SerializeField] private int maxRangeAngle;
    [SerializeField] private GameObject arrow;


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

        float zRotation = -leaningAmount * maxRangeAngle;
        arrow.transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
