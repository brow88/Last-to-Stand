using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("UI")] [SerializeField] LeaningMeterUI leaningMeterUI;
    [SerializeField] QuickTimeUI quickTimeUI;

    public int NumberOfDrinks = 0;

    #region Leanig / Blancing

    [Header("Leaning Variable")] [SerializeField]
    private float playerLeaningInputMultipler;

    [SerializeField] private float baseFallingForce;
    [SerializeField] private float stopDuration = 0.5f; // Duration of the stop
    [SerializeField] private float minStopInterval = 1f; // Minimum time between stops
    [SerializeField] private float maxStopInterval = 3f; // Maximum time between stops

    public float leaning = 0; //This is between -1 (left) and +1 (right)
    public bool playerStanding = true;

    public float baseSpeed = 0.2f; // Base speed at which the fillAmount changes
    public float speedIncreasePerDrink = 0.05f; // Speed increase per drink
    private bool isStopping = false;
    private float nextStopTime;

    #endregion

    #region QTE

    [Header("Quick Time Event")] [SerializeField]
    private float qteSpeed = 1f;

    [SerializeField] private float qteMinSpeed = 0.5f;
    [SerializeField] private float qteMaxSpeed = 2f;
    [SerializeField] private float qteHitWindow = 0.04f; // How close to the target the player needs to be
    private bool isQTEActive = false;
    private float qteMarkerPosition = 0f;
    private bool qteMovingRight = true;
    private float qteTargetPosition = 0.5f;
    public event Action<float> OnQTargetUpdate; // Event to set target in UI
    public event Action<float> OnQTEUpdate; // Event to update UI
    public event Action<bool> OnQTEHitOrMiss;
    [SerializeField] private float QteCollectTimeInSec = 0.1f;
    [SerializeField] private float QteCooldowTimeInSec = 0.2f;
    [SerializeField] private int QteNumberOfTries = 3;
     private int QteNumberOfTriesReset;

    private bool isQteAllowed = true;
    private bool isQteRecording = true;
    private DateTime startQteCollect;
    private DateTime startQteCooldown;
    private List<float> qteCollect;
    #endregion

    #region Drinking

    private bool hasDrink;

    #endregion


    private void Start()
    {
        ScheduleNextStop();
        //todo debug
        StartQTE();
        QteNumberOfTriesReset = QteNumberOfTries;
    }

    private void Awake()
    {
        if (quickTimeUI != null)
        {
            OnQTEUpdate += quickTimeUI.UpdateQTE;
            OnQTargetUpdate += quickTimeUI.SetQTargetPosition;
        }
    }

    private void OnDestroy()
    {
        if (quickTimeUI != null)
        {
            OnQTEUpdate -= quickTimeUI.UpdateQTE;
            OnQTargetUpdate -= quickTimeUI.SetQTargetPosition;
        }
    }

    private void FixedUpdate()
    {
        if (playerStanding && GameManager.Instance.IsGamePlaying())
        {
            HandlePlayerInput();

            if (!isStopping)
            {
                ApplySwayingPhysics();
            }

            // After player input and leaning Physics clamp the leaning value to stay within the bounds of -1 and 1
            leaning = Mathf.Clamp(leaning, -1f, 1f);

            leaningMeterUI.UpdateLeaningMeter(leaning);

            CheckIfPlayerHasFallen();
            CheckForStop();

            if (isQTEActive)
            {
                HandleQTE();
            }
        }
    }

    public void PlayerReset()
    {
        leaning = 0;
        playerStanding = true;
        leaningMeterUI.UpdateLeaningMeter(leaning);
    }

    /// <summary>
    /// Handles player input for leaning.
    /// </summary>
    private void HandlePlayerInput()
    {
        if (InputManager.Instance.LeanRightInput())
        {
            leaning += playerLeaningInputMultipler * Time.deltaTime;
        }

        if (InputManager.Instance.LeanLeftInput())
        {
            leaning -= playerLeaningInputMultipler * Time.deltaTime;
        }

        if (InputManager.Instance.SpaceBarDown())
        {
            Drink();
        }
    }

    #region Balance / sway

    /// <summary>
    /// Simulate the player leaning more towards the current direction
    /// </summary>
    private void ApplySwayingPhysics()
    {
        //applied force can never be larger than player force
        float appliedFallingForce = Math.Min(baseFallingForce + (NumberOfDrinks * speedIncreasePerDrink),
            playerLeaningInputMultipler);

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
        float interval = UnityEngine.Random.Range(minStopInterval, maxStopInterval) / (NumberOfDrinks + 1);
        nextStopTime = Time.time + interval;
    }

    #endregion

    #region Drinking

    public void GotDrink()
    {
        hasDrink = true;
    }

    public void Drink()
    {
        if (!hasDrink) //todo maybe VO line "How am I suppose to drink without a drink"?
            return;

        NumberOfDrinks++;
        GameManager.Instance.UpdatePlayerScore(this, 1); //ToDo: score vary according to type of drink?!?
    }

    #endregion




    #region QTE

    private void HandleQTE()
    {
        // Move the QTE marker
        if (qteMovingRight)
        {
            qteMarkerPosition += qteSpeed * Time.deltaTime;
            if (qteMarkerPosition >= 1f)
            {
                qteMarkerPosition = 1f;
                qteMovingRight = false;
            }
        }
        else
        {
            qteMarkerPosition -= qteSpeed * Time.deltaTime;
            if (qteMarkerPosition <= 0f)
            {
                qteMarkerPosition = 0f;
                qteMovingRight = true;
            }
        }

        // Send QTE update to the UI
        OnQTEUpdate?.Invoke(qteMarkerPosition);

        // Check for player input
        if (InputManager.Instance.QuickTimeInput() && isQteAllowed)
        {
            if (!isQteRecording)
            {
                startQteCollect = DateTime.Now;
                qteCollect.Clear();
                isQteRecording = true;
            }

            if ((DateTime.Now - startQteCollect).TotalSeconds >= QteCollectTimeInSec)
            {
                //todo check results
                if (qteCollect.Any(distance => distance <= qteHitWindow))
                {
                    // Player successfully hit the target
                    Debug.Log($"QTE Success!");
                    EndQTE();
                    OnQTEHitOrMiss?.Invoke(true);
                }
                else
                {
                    // Player missed the target
                    //todo this seems to trigger mutliple times, better way to check?
                    Debug.Log($"QTE Fail!");
                    OnQTEHitOrMiss?.Invoke(false);
                    if (--QteNumberOfTries <=0)
                    {
                        //fail too many times, what now?

                    }
                }
                isQteRecording = false;
                isQteAllowed = false;
                startQteCooldown = DateTime.Now;

            }
            var distance = Mathf.Abs(qteMarkerPosition - qteTargetPosition);
            qteCollect.Add(distance);
        }
        else
        {
            if (!isQteAllowed && (DateTime.Now - startQteCooldown).TotalSeconds >= QteCooldowTimeInSec)
            {
                isQteAllowed = true;
            }
            isQteRecording = false;
        }
    }

    public void StartQTE()
    {
        qteCollect = new List<float>();
        QteNumberOfTries = QteNumberOfTriesReset;
        isQteAllowed = true;
        isQTEActive = true;
        qteMarkerPosition = 0f;
        qteMovingRight = true;
        qteSpeed = UnityEngine.Random.Range(qteMinSpeed, qteMaxSpeed);
        qteTargetPosition = UnityEngine.Random.Range(0.1f, 0.9f); // Random target position within the bar
        OnQTargetUpdate?.Invoke(qteTargetPosition);
    }

    public void EndQTE()
    {
        isQTEActive = false;
    }

    #endregion
}