using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] LeaningMeterUI leaningMeterUI;

    [Header("Leaning Variable")]
    [SerializeField] private float playerLeaningInputMultipler;
    [SerializeField] private float appliedFallingForce;
    
    public float leaning = 0;  //This is between -1 (left) and +1 (right)
    private bool playerStanding = true;


    private void FixedUpdate()
    {
        if (playerStanding && GameManager.Instance.IsGamePlaying())
        {
            HandlePlayerLeaningInput();

            ApplySwayingPhysics();

            //After player input and leaning Physics clamp the leaning value to stay within the bounds of -1 and 1
            leaning = Mathf.Clamp(leaning, -1f, 1f);

            leaningMeterUI.UpdateLeaningMeter(leaning);

            CheckIfPlayerHasFallen();
        }
    }


    /// <summary>
    /// Handles player input for leaning.
    /// </summary>
    private void HandlePlayerLeaningInput()
    {
        if (InputManager.Instance.LeanRightInput())
        {
            leaning += playerLeaningInputMultipler; 
        }
        if (InputManager.Instance.LeanLeftInput())
        {
            leaning -= playerLeaningInputMultipler;
        }
    }


    /// <summary>
    /// Simulate the player leaning more towards the current direction
    /// </summary>
    private void ApplySwayingPhysics()
    {
        //Check which way you are leaning
        if (leaning > 0)
        {
            //leaning right so you lean more right
            leaning += appliedFallingForce;
        }
        else
        {
            //leaning left so lean more left
            leaning -= appliedFallingForce;
        }
    }


    private void CheckIfPlayerHasFallen()
    {
        if (leaning >= 1f || leaning <= -1f)
        {
            playerStanding = false;
            GameManager.Instance.PlayerHasFallen(this);
        }
    }


    public void Drink()
    {

    }


}
