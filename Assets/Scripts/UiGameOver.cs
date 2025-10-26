using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UiGameOver : MonoBehaviour
{
    public int displayedPoints = 0;
    public TextMeshProUGUI pointsUI;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onGameStateUpdated.AddListener(GameStateUpdated);
    }

    void OnDestroy()
    {
        GameManager.Instance.onGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if(newState == GameManager.GameState.GameOver)
        {
            displayedPoints = 0;
            StartCoroutine(DisplayFinalScore());
        }
    }

    IEnumerator DisplayFinalScore()
    {
        while (displayedPoints < GameManager.Instance.points)
        {
            displayedPoints++;
            pointsUI.text = displayedPoints.ToString();
            yield return new WaitForFixedUpdate();
        }

        displayedPoints = GameManager.Instance.points;
        pointsUI.text = displayedPoints.ToString();

        yield return null;
    }

    public void PlayAgain()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void QuitGame()
    {
        GameManager.Instance.ExitGame();
    }
}
