using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get;  private set; }

    public event EventHandler OnGameStateChange;

    [Tooltip("Timer count down before game starts")]
    public float startTimer = 3f;

    public enum GameMode
    {
        SinglePlayer,
        Multiplayer,
    }
    private GameMode gameMode;

    public enum GameState
    {
        Tutorial,
        StartTimer,
        Playing,
        GameOver
    }
    private GameState state;

    //Player scores
    private Dictionary<Player, int> playersScores = new Dictionary<Player, int>();


    void Awake()
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


    //TODO: eventually this will be called by mainMenu
    private void Start()
    {
        NewGame(GameMode.SinglePlayer);
    }


    private void Update()
    {
        switch (state)
        {
            case GameState.Tutorial:
                //Exit tutorial and start game when space bar is press down
                if (InputManager.Instance.SpaceBarDown())
                {
                    ChangeGameState(GameState.StartTimer);
                }
                break; 
            case GameState.StartTimer:
                //Timed entrance into the game
                startTimer -= Time.deltaTime;
                if (startTimer <= 0)
                {
                    ChangeGameState(GameState.Playing);
                }
                break; 
            case GameState.Playing:
                break; 
            case GameState.GameOver:
                break;
        }
    }


    private void ChangeGameState(GameState newGameState)
    {
        state = newGameState;
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }


    public void NewGame(GameMode gameMode)
    {
        //ToDo: eventually the game mode will affect the set up
        playersScores.Clear();
        GameObject[] players_GOs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player_GO in players_GOs)
        {
            Player player = player_GO.GetComponent<Player>();
            playersScores.Add(player, 0);
            player.PlayerReset();
        }

        ChangeGameState(GameState.Tutorial);
    }


    public void RetryGame()
    {
        //reset each player
        List<Player> players = new List<Player>(playersScores.Keys);
        foreach (Player player in players)
        {
            // reset score
            playersScores[player] = 0;
            //reset player
            player.PlayerReset();
        }

        startTimer = 3f;
        ChangeGameState(GameState.StartTimer);
    }


    public void UpdatePlayerScore(Player player, int score)
    {
        playersScores[player] += score;
    }

    #region Getter and setters

    public bool IsGameModeSinglePlayer()
    {
        return gameMode == GameMode.SinglePlayer;
    }

    public bool IsGameModeMultiplyPlayer()
    {
        return gameMode == GameMode.Multiplayer;
    }

    public bool IsTutorial()
    {
        return state == GameState.Tutorial;
    }

    public bool IsStartTimerActive()
    {
        return state == GameState.StartTimer;
    }

    public bool IsGamePlaying()
    {
        return state == GameState.Playing;
    }

   public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }

    public float GetStartTimer()
    {
        return startTimer;
    }

    public void PlayerHasFallen(Player player)
    {
        ChangeGameState(GameState.GameOver);
    }

    public Dictionary<Player, int> GetPlayerScores()
    {
        return playersScores;
    }

    #endregion
}
