using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get;  private set; }

    public event EventHandler OnGameStateChange;
    public event EventHandler<LoseCondition> OnGameOver;
    public event EventHandler OnScoreChange;

    [Tooltip("Timer count down before game starts")]
    public float startTimer = 3f;

    private GameMode gameMode;

    public enum GameState
    {
        Tutorial,
        StartTimer,
        Playing,
        GameOverTansitionTansition,
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
            case GameState.GameOverTansitionTansition:
                if (InputManager.Instance.SpaceBarDown())
                {
                    ChangeGameState(GameState.StartTimer);
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
        this.gameMode = gameMode;
        playersScores.Clear();

        GameObject[] players_GOs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player_GO in players_GOs)
        {
            Player player = player_GO.GetComponent<Player>();

            if (gameMode == GameMode.SinglePlayer)
            {
                if (player.IsPlayerOne)
                {
                    playersScores.Add(player, 0);
                    //player.PlayerReset();
                }
                else
                {
                    //Deactivate Player 2 if in single play mode
                    player.DeactivatePlayer();
                }
            }
            if (gameMode == GameMode.Multiplayer)
            {
                playersScores.Add(player, 0);
                //player.PlayerReset();
            }
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
        OnScoreChange?.Invoke(this, EventArgs.Empty);
        Debug.Log("Score changed");
    }

    public void TriggerGameOver(LoseCondition loseCondition)
    {
        // Alters multi-player score due to loss
        if (gameMode == GameMode.Multiplayer)
        {
            List<Player> playersToModify = new List<Player>();

            switch (loseCondition)
            {
                case LoseCondition.Player1FellOver:
                case LoseCondition.Player1PassedOut:
                    foreach (var kvp in playersScores)
                    {
                        if (kvp.Key.IsPlayerOne)
                        {
                            playersToModify.Add(kvp.Key);
                        }
                    }
                    break;
                case LoseCondition.Player2FellOver:
                case LoseCondition.Player2PassedOut:
                    foreach (var kvp in playersScores)
                    {
                        if (!kvp.Key.IsPlayerOne)
                        {
                            playersToModify.Add(kvp.Key);
                        }
                    }
                    break;

            }

            // Modify the scores outside of the enumeration
            foreach (var player in playersToModify)
            {
                playersScores[player] -= 20;
            }
        }

        //ToDo: transition fade+animation. have put transition in game state enum and space to go to game over

        OnGameOver?.Invoke(this, loseCondition);
        ChangeGameState(GameState.GameOver);
    }


    #region Getter and setters

    public List<Player> GetPlayers => playersScores.Keys.ToList();

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

    public Dictionary<Player, int> GetPlayerScores()
    {
        return playersScores;
    }

    #endregion
}
