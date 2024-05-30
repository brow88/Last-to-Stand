using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    public static Bartender Instance;

    [SerializeField] private float minIntervalServing = 5f;
    [SerializeField] private GameObject glass;
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
        var newGlass =Instantiate(glass, GlassManager.Instance.GetPlayer1EndPos(),GlassManager.Instance.StartPositionPlayer1.rotation);
        var glassProperties = newGlass.GetComponent<SlidingGlass>();
        glassProperties.StartPosition = GlassManager.Instance.StartPositionPlayer1;
        glassProperties.EndPosition = GlassManager.Instance.EndPositionPlayer1;
        glassProperties.CatchPosition = GlassManager.Instance.CatchPositionPlayer1;
        glassProperties.SetReady();
    }

    private void ScheduleNextDrink()
    {
        float interval = UnityEngine.Random.Range(minIntervalServing, maxIntervalServing);
        nextDrinkServingTime = Time.time + interval;
    }

    public void NotifyThatPlayerDroppedGlass(Player player)
    {
        Debug.Log("Bartender is angry!");
        StartCoroutine(WalkOverAndSlap(3f));
    }

    private IEnumerator WalkOverAndSlap(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.PlaySound(SoundManager.Instance.FindClip("Slap"), 1f);
    }
}
