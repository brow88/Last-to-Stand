using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bartender : MonoBehaviour
{
    public static Bartender Instance;

    [SerializeField] private float minIntervalServing = 5f;
    [SerializeField] private float maxIntervalServing = 10f;
    private float nextDrinkServingTime;
    [SerializeField] private GameObject glass;

    

    [SerializeField] private float startMinGlassSpeed = 60f;
    [SerializeField] private float startMaxGlassSpeed = 90f;
    private Player lastPlayerServed;

    private List<Player> players;
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
        players ??= GameManager.Instance.GetPlayers;
        //randomly serve a new glass every X Seconds
        if (Time.time >= nextDrinkServingTime)
        {
            if (lastPlayerServed == null)
            {
                lastPlayerServed = players.First();
            }

            if (players.Count>1)
            {
                var serveToPlayer = players.FirstOrDefault(o=>o != lastPlayerServed);
                ServeDrink(serveToPlayer);
                lastPlayerServed = serveToPlayer;
            }
            else
            {
                ServeDrink(lastPlayerServed);
            }
           
            ScheduleNextDrink();
        }
    }

    public void ServeDrink(Player player)
    {
        Debug.Log(" Bartender is serving Drink");
        var newGlass =Instantiate(glass, GlassManager.Instance.GetPlayer1EndPos(),GlassManager.Instance.StartPositionPlayer1.rotation);
        var glassProperties = newGlass.GetComponent<SlidingGlass>();
        glassProperties.StartPosition = GlassManager.Instance.StartPositionPlayer1;
        glassProperties.EndPosition = GlassManager.Instance.EndPositionPlayer1;
        glassProperties.CatchPosition = GlassManager.Instance.CatchPositionPlayer1;
        glassProperties.Player = player;
        glassProperties.Speed = UnityEngine.Random.Range(startMinGlassSpeed, startMaxGlassSpeed);
        glassProperties.SetReady();
    }

    private void ScheduleNextDrink()
    {
        float interval = UnityEngine.Random.Range(minIntervalServing, maxIntervalServing);
        nextDrinkServingTime = Time.time + interval;
        startMinGlassSpeed++;
        startMaxGlassSpeed++;

        startMinGlassSpeed = Math.Min(startMinGlassSpeed, 100f);
        startMaxGlassSpeed = Math.Min(startMaxGlassSpeed, 150f);

    }

    public void NotifyThatPlayerDroppedGlass(Player player)
    {
        Debug.Log("Bartender is angry!");
        StartCoroutine(WalkOverAndSlap(3f,player));
    }

    private IEnumerator WalkOverAndSlap(float delay, Player player)
    {
        yield return new WaitForSeconds(delay);
        gameObject.PlaySound(SoundManager.Instance.FindClip("Slap"), 1f);
        player.Slapped();
    }
}
