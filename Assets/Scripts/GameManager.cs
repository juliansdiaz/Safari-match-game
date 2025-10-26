using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float timeToMatch = 10f;
    public float currentTimeToMatch = 0;
    public int points = 0;
    public UnityEvent onPointsUpdated;
    public UnityEvent<GameState> onGameStateUpdated;

    public enum GameState
    {
        Idle,
        InGame,
        GameOver
    };

    public GameState gameState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(gameState == GameState.InGame)
        {
            currentTimeToMatch += Time.deltaTime;
            if(currentTimeToMatch > timeToMatch)
            {
                gameState = GameState.GameOver;
                onGameStateUpdated?.Invoke(gameState);
            }
        }
    }

    public void AddPoints(int newPoints)
    {
        points += newPoints;
        onPointsUpdated?.Invoke();
        currentTimeToMatch = 0;
    }

    public void RestartGame()
    {
        points = 0;
        gameState = GameState.InGame;
        onGameStateUpdated?.Invoke(gameState);
        currentTimeToMatch = 0;
    }
    
    public void ExitGame()
    {
        
    }
}
