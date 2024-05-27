using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    public static Bartender Instance;

    [SerializeField] private float minIntervalServing = 5f;
    [SerializeField] private float maxIntervalServing = 10f;
    private float nextDrinkServingTime;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }
        CheckIfDrinkShouldBeServed();
    }

    public void CheckIfDrinkShouldBeServed()
    {
        //randomly serve a new glass every X Seconds
        if (Time.time >= nextDrinkServingTime)
        {
            ServeDrink();
            ScheduleNextDrink();
        }
    }

    public void ServeDrink()
    {
        Debug.Log(" Bartender is serving Drink");
    }

    private void ScheduleNextDrink()
    {
        float interval = UnityEngine.Random.Range(minIntervalServing, maxIntervalServing);
        nextDrinkServingTime = Time.time + interval;
    }

    public void NotifyThatPlayerDroppedGlass(Player player)
    {
        Debug.Log("Bartender is angry!");
    }
}
