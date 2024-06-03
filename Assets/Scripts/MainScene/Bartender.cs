using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    public static Bartender Instance;

    [SerializeField] private float minIntervalServing = 5f;
    [SerializeField] private float maxIntervalServing = 10f;
    private float nextDrinkServingTime;
    [SerializeField] private GameObject glass;

    [SerializeField] private Animator animator;
    private bool busy;    //checks if an action is already being done: serve drink, slap

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
        if (busy)
        {
            return;
        }

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
                StartCoroutine(ServeDrink(serveToPlayer));
                lastPlayerServed = serveToPlayer;
            }
            else
            {
                StartCoroutine(ServeDrink(lastPlayerServed));
            }
           
            ScheduleNextDrink();
        }
    }

    public IEnumerator ServeDrink(Player player)
    {
        while (busy)
        {
            yield return null;
        }

        Debug.Log(" Bartender is serving Drink");
        busy = true;

        //Pour drink and set which way to slide the drink for the animator
        animator.SetTrigger("PourDrink");     
        if (player.IsPlayerOne)
        {
            //Slide left
            animator.SetBool("SlideL", true);
        }
        else
        {
            //Slide right
            animator.SetBool("SlideL", false);
        }       
        yield return new WaitForSeconds(3f);    //poor drink 2.5 second + hand push 

        //Send the whiskey
        GlassManager glassManager = GlassManager.Instance;
        Transform startPosition;
        Transform endPosition;
        Transform catchPosition;

        if (player.IsPlayerOne)
        {
            startPosition = glassManager.StartPositionPlayer1;
            endPosition = glassManager.EndPositionPlayer1;
            catchPosition = glassManager.CatchPositionPlayer1;
        }
        else
        {
            startPosition = glassManager.StartPositionPlayer2;
            endPosition = glassManager.EndPositionPlayer2;
            catchPosition = glassManager.CatchPositionPlayer2;
        }

        var newGlass = Instantiate(glass, endPosition.position, startPosition.rotation);
        var glassProperties = newGlass.GetComponent<SlidingGlass>();
        glassProperties.StartPosition = startPosition;
        glassProperties.EndPosition = endPosition;
        glassProperties.CatchPosition = catchPosition;
        glassProperties.Player = player;
        glassProperties.Speed = UnityEngine.Random.Range(startMinGlassSpeed, startMaxGlassSpeed);
        glassProperties.SetReady();


        //Wait for hands to move back
        yield return new WaitForSeconds(1f);      //pass drink is 1.5 seconds but the hands moving back is about 
        busy = false;
    }

    private void ScheduleNextDrink()
    {
        float interval = UnityEngine.Random.Range(minIntervalServing, maxIntervalServing);
        nextDrinkServingTime = Time.time + interval;
        startMinGlassSpeed += 20;
        startMaxGlassSpeed += 20;

        startMinGlassSpeed = Math.Min(startMinGlassSpeed, 100f);
        startMaxGlassSpeed = Math.Min(startMaxGlassSpeed, 150f);

    }

    public void NotifyThatPlayerDroppedGlass(Player player)
    {
        Debug.Log("Bartender is angry!");
        StartCoroutine(WalkOverAndSlap(player));
    }

    private IEnumerator WalkOverAndSlap(Player player)
    {
        while (busy)
        {
            yield return null;
        }

        busy = true;

        //slap animation
        if (player.IsPlayerOne)
        {
            animator.SetTrigger("SlapL");
        }
        else
        {
            animator.SetTrigger("SlapR");
        }

        //walk to play and raise hand for slap
        yield return new WaitForSeconds(2.2f);

        //Slap
        gameObject.PlaySound(SoundManager.Instance.FindClip("Slap"), 1f);
        player.Slapped();

        //walk back
        yield return new WaitForSeconds(1.8f);
        
        busy = false;
    }
}
