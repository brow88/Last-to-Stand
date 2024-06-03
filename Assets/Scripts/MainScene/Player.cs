using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public bool IsPlayerOne;
    [SerializeField] private GameObject characterArt;
    private Animator animator;

    [Header("UI")]
    [SerializeField] private PlayerBarUI playerBarUI;
    [SerializeField] private LeaningMeterUI leaningMeterUI;
    [SerializeField] private QuickTimeUI quickTimeUI;

    public int NumberOfDrinks = 0;
    /// <summary>
    /// How drunk is the player from 0-100
    /// </summary>
    public int DrunkLevel = 0;

    #region Leaning / Balancing

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
    private bool isLeaningLeft;
    private DateTime lastLeanCheck;
    private float appliedLeaningForce =0f;
    #endregion

    #region QTE

    [Header("Quick Time Event")] [SerializeField]
    private float qteSpeed = 1f;

    [SerializeField] private float qteMinSpeed = 0.5f;
    [SerializeField] private float qteMaxSpeed = 0.9f;
    [SerializeField] private float qteHitWindow = 0.04f; // How close to the target the player needs to be
    [SerializeField] private float qteCloseHitWindow = 0.04f; // How close to the target the player needs to be to still be close
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

    [SerializeField] private int QteFailedRaisesDrunkBy = 20;
    [SerializeField] private int QteOkRaisesDrunkBy = 10;
    [SerializeField] private int QteerfectRaisesDrunkBy = 0;
     private int QteNumberOfTriesReset;

    private bool isQteAllowed = true;
    private bool isQteRecording = true;
    private DateTime startQteCollect;
    private DateTime startQteCooldown;
    private DateTime startQteEvent;

    private List<float> qteCollect;
    #endregion

    #region Drinking

    private bool hasDrink;

    #endregion


    private void Awake()
    {
        animator = characterArt.GetComponent<Animator>();

        if (quickTimeUI != null)
        {
            OnQTEUpdate += quickTimeUI.UpdateQTE;
            OnQTargetUpdate += quickTimeUI.SetQTargetPosition;
        }
    }


    private void Start()
    {
        ScheduleNextStop();
        QteNumberOfTriesReset = QteNumberOfTries;
        GameManager.Instance.OnScoreChange += GameManager_OnScoreChange;
        lastLeanCheck = DateTime.Now;
    }

    private void Update()
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

            if (DrunkLevel >= 100)
            {
                if (IsPlayerOne)
                {
                    GameManager.Instance.TriggerGameOver(LoseCondition.Player1PassedOut);
                }
                else
                {
                    GameManager.Instance.TriggerGameOver(LoseCondition.Player2PassedOut);
                }
                
            }
        }
    }

    private void FixedUpdate()
    {
        SwayAnimationWeights();
    }

    private void GameManager_OnScoreChange(object sender, EventArgs e)
    {
        if (IsPlayerOne || GameManager.Instance.IsGameModeMultiplyPlayer())
        {
            int score = GameManager.Instance.GetPlayerScores()[this];
            playerBarUI.UpdateScoreDisplay(score);      
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

    public void PlayerReset()
    {
        leaning = 0;
        playerStanding = true;
        leaningMeterUI.UpdateLeaningMeter(leaning);
        playerBarUI.ResetPlayerBarUI();
    }


    //Used by player 2 if in one player mode
    public void DeactivatePlayer()
    {
        playerBarUI.gameObject.SetActive(false);
        leaningMeterUI.gameObject.SetActive(false);
        quickTimeUI.gameObject.SetActive(false);

        //disables this script
        enabled = false;
    }


    /// <summary>
    /// Handles player input for leaning.
    /// </summary>
    private void HandlePlayerInput()
    {
        if (IsPlayerOne)
        {
            //Player One inputs
            if (InputManager.Instance.LeanRightPlayerOneInput())
            {
                leaning += playerLeaningInputMultipler * Time.deltaTime;
            }

            if (InputManager.Instance.LeanLeftPlayerOneInput())
            {
                leaning -= playerLeaningInputMultipler * Time.deltaTime;
            }

            if (InputManager.Instance.SpaceBarDown())
            {
                StartCoroutine(Drink());
            }
        }
        else
        {
            //Player two inputs
            if (InputManager.Instance.LeanRightPlayerTwoInput())
            {
                leaning += playerLeaningInputMultipler * Time.deltaTime;
            }

            if (InputManager.Instance.LeanLeftPlayerTwoInput())
            {
                leaning -= playerLeaningInputMultipler * Time.deltaTime;
            }

            if (InputManager.Instance.DrinkPlayerTwoInput())
            {
                StartCoroutine(Drink());
            }
        }

    }

    #region Balance / sway / Slap

    public void Slapped()
    {
        //If slapped the -1 score
        GameManager.Instance.UpdatePlayerScore(this, -1);

        var slapForce = Random.Range(0.25f,0.35f);
        var isLeft = Random.Range(0, 10) > 5;
        if (isLeft)
        {
            leaning -= slapForce;
        }
        else
        {
            leaning += slapForce;
        }    
    }

    /// <summary>
    /// Simulate the player leaning more towards the current direction
    /// </summary>
    private void ApplySwayingPhysics()
    {
        if ((DateTime.Now - lastLeanCheck).TotalSeconds>=1f)
        {
            var swapDirection = Random.Range(0, 10) == 5;
            if (swapDirection)
                isLeaningLeft = !isLeaningLeft;
            lastLeanCheck = DateTime.Now;
            //applied force can never be larger than player force
            var baseLineForce = Math.Min(baseFallingForce + (NumberOfDrinks * speedIncreasePerDrink), playerLeaningInputMultipler);
            float appliedFallingForce = Random.Range(baseLineForce * 0.5f, baseLineForce * 1.5f);
            appliedLeaningForce = appliedFallingForce;
        }
      
       
        // Check which way you are leaning
        if (!isLeaningLeft)
        {
            // Leaning right so you lean more right
            leaning += appliedLeaningForce * Time.deltaTime;
        }
        else
        {
            // Leaning left so lean more left
            leaning -= appliedLeaningForce * Time.deltaTime;
        }
    }

    private void SwayAnimationWeights()
    {
        // Check which way you are leaning
        if (leaning == 0)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
        }
        else if (leaning > 0)
        {
            // Leaning right so you lean more right in animation
            animator.SetLayerWeight(1, 0);  //left to 0
            animator.SetLayerWeight(2, leaning);   //right to leaning amount
        }
        else
        {
            // Leaning left so lean more left in animation
            animator.SetLayerWeight(1, -leaning);  //left by leaning amount (should be +ve so reverse sign)
            animator.SetLayerWeight(2, 0);   //right to leaning amount
        }
    }

    private void CheckIfPlayerHasFallen()
    {
        if (leaning >= 0.98f || leaning <= -0.98f)
        {
            gameObject.PlaySound(SoundManager.Instance.FindClip("fall"));
            playerStanding = false;
            if (IsPlayerOne)
            {
                GameManager.Instance.TriggerGameOver(LoseCondition.Player1FellOver);
            }
            else
            {
                GameManager.Instance.TriggerGameOver(LoseCondition.Player2FellOver);
            }          
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
        float interval = Random.Range(minStopInterval, maxStopInterval) / (NumberOfDrinks + 1);
        nextStopTime = Time.time + interval;
    }

    #endregion

    #region Drinking

    public void GetDrink()
    {
        hasDrink = true;
        animator.SetLayerWeight(3, 1);
    }

    public bool HasDrink 
    { 
        get 
        {   
            return hasDrink; 
        } 
    }

    public IEnumerator Drink()
    {
        if (!hasDrink) //todo maybe VO line "How am I suppose to drink without a drink"?
            yield break;

        //Animation
        animator.SetTrigger("Drink");

        //release glass back onto table after animation.
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(3, 0);

        NumberOfDrinks++;
        ChangedDrunkLevel(1);

        GameManager.Instance.UpdatePlayerScore(this, 5); //ToDo: score vary according to type of drink?!?

        if (NumberOfDrinks % 3 == 0)
        {
            quickTimeUI.gameObject.SetActive(true);
            StartQTE();
        }

        hasDrink = false;
    }

    public void ChangedDrunkLevel(int amount)
    {
        DrunkLevel += amount;
        playerBarUI.UpdateVomitMeter(DrunkLevel/100f);
    }

    #endregion

    #region QTE

    private void HandleQTE()
    {
        if ((DateTime.Now - startQteEvent).TotalSeconds > 10)
        {
            Vomit();
            EndQTE();
            return;
        }

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
        if (InputManager.Instance.QuickTimePlayerOneInput() && isQteAllowed)
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
                    Debug.Log($"QTE Success! (perfect)");
                    ChangedDrunkLevel(QteerfectRaisesDrunkBy);
                    EndQTE();
                    OnQTEHitOrMiss?.Invoke(true);
                }
                if (qteCollect.Any(distance => distance <= qteCloseHitWindow))
                {
                    // Player successfully hit the target
                    Debug.Log($"QTE Success! (ok)");
                    ChangedDrunkLevel(QteOkRaisesDrunkBy);
                    EndQTE();
                    OnQTEHitOrMiss?.Invoke(true);
                }
                else
                {
                    // Player missed the target
                    //todo this seems to trigger multiple times, better way to check?
                    Debug.Log($"QTE Fail!");
                    OnQTEHitOrMiss?.Invoke(false);
                    if (--QteNumberOfTries <=0)
                    {
                        //fail too many times, what now?
                        ChangedDrunkLevel(QteFailedRaisesDrunkBy);
                        EndQTE();
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

    public void Vomit()
    {
        var vomitForce = Random.Range(0.3f, 0.4f);
        var isLeft = Random.Range(0, 10) > 5;
        if (isLeft)
        {
            leaning -= (float)vomitForce;
        }
        else
        {
            leaning += (float)vomitForce;
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
        startQteEvent = DateTime.Now;
    }

    public void EndQTE()
    {
        isQTEActive = false;
        quickTimeUI.gameObject.SetActive(false);
        qteMinSpeed += 0.05f;
        qteMaxSpeed += 0.05f;
        qteMinSpeed = Math.Min(qteMinSpeed, 1.8f);
        qteMaxSpeed = Math.Min(qteMaxSpeed, 2.1f);

    }

    #endregion
}