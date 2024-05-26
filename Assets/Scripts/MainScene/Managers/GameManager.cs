using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get;  private set; }

    public event EventHandler OnGameStateChange;

    public enum GameState
    {
        Tutorial,
        StartTimer,
        Playing,
        GameOver
    }
    private GameState state;

    private float startTimer = 3f;

    private int playerScore = 0;


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
                    state = GameState.StartTimer;
                    OnGameStateChange?.Invoke(this, EventArgs.Empty);
                }
                break; 
            case GameState.StartTimer:
                startTimer -= Time.deltaTime;
                if (startTimer <= 0)
                {
                    state = GameState.Playing;
                    OnGameStateChange?.Invoke(this, EventArgs.Empty);
                }
                break; 
            case GameState.Playing:
                break; 
            case GameState.GameOver:
                break;
        }
    }

    public bool IsTutorial()
    {
        return state == GameState.Tutorial;
    }

    public bool IsStartTimerActive()
    {
        return state == GameState.StartTimer;
    }

    public float GetStartTimer()
    {
        return startTimer;
    }

    public bool IsGamePlaying()
    {
        return state == GameState.Playing;
    }

    public void PlayerHasFallen(Player player)
    {
        state = GameState.GameOver;
        OnGameStateChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }
}
