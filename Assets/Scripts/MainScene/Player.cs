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
    [SerializeField] private float baseFallingForce;
    [SerializeField] private float stopDuration = 0.5f;  // Duration of the stop
    [SerializeField] private float minStopInterval = 1f; // Minimum time between stops
    [SerializeField] private float maxStopInterval = 3f; // Maximum time between stops

    public float leaning = 0;  //This is between -1 (left) and +1 (right)
    private bool playerStanding = true;

    public float baseSpeed = 0.2f;  // Base speed at which the fillAmount changes
    public float speedIncreasePerDrink = 0.05f;  // Speed increase per drink
    public int NumberOfdrinks = 0;

    private bool isStopping = false;
    private float nextStopTime;

    private void Start()
    {
        ScheduleNextStop();
    }

    private void FixedUpdate()
    {
        if (playerStanding && GameManager.Instance.IsGamePlaying())
        {
            HandlePlayerLeaningInput();

            if (!isStopping)
            {
                ApplySwayingPhysics();
            }

            // After player input and leaning Physics clamp the leaning value to stay within the bounds of -1 and 1
            leaning = Mathf.Clamp(leaning, -1f, 1f);

            leaningMeterUI.UpdateLeaningMeter(leaning);

            CheckIfPlayerHasFallen();
            CheckForStop();
        }
    }

    /// <summary>
    /// Handles player input for leaning.
    /// </summary>
    private void HandlePlayerLeaningInput()
    {
        if (InputManager.Instance.LeanRightInput())
        {
            leaning += playerLeaningInputMultipler * Time.deltaTime;
        }
        if (InputManager.Instance.LeanLeftInput())
        {
            leaning -= playerLeaningInputMultipler * Time.deltaTime;
        }
    }

    /// <summary>
    /// Simulate the player leaning more towards the current direction
    /// </summary>
    private void ApplySwayingPhysics()
    {
        //applied force can never be larger than player force
        float appliedFallingForce = Math.Min(baseFallingForce + (NumberOfdrinks * speedIncreasePerDrink), playerLeaningInputMultipler);
        

        // Check which way you are leaning
        if (leaning > 0)
        {
            // Leaning right so you lean more right
            leaning += appliedFallingForce * Time.deltaTime;
        }
        else
        {
            // Leaning left so lean more left
            leaning -= appliedFallingForce * Time.deltaTime;
        }
    }

    private void CheckIfPlayerHasFallen()
    {
        if (leaning >= 0.98f || leaning <= -0.98f)
        {
            playerStanding = false;
            GameManager.Instance.PlayerHasFallen(this);
        }
    }

    private void CheckForStop()
    {
        if (Time.time >= nextStopTime)
        {
            StartCoroutine(StopRoutine());
            ScheduleNextStop();
        }
    }

    private IEnumerator StopRoutine()
    {
        isStopping = true;
        yield return new WaitForSeconds(stopDuration);
        isStopping = false;
    }

    private void ScheduleNextStop()
    {
        float interval = UnityEngine.Random.Range(minStopInterval, maxStopInterval) / (NumberOfdrinks + 1);
        nextStopTime = Time.time + interval;
    }

    public void Drink()
    {
        NumberOfdrinks++;
    }
}
