using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public AudioClip moveSFX;
    public AudioClip missSFX;
    public AudioClip matchSFX;
    public AudioClip gameOverSFX;
    public AudioSource SFXSource;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onPointsUpdated.AddListener(PointsUpdated);
        GameManager.Instance.onGameStateUpdated.AddListener(GameStateUpdated);
    }

    void OnDestroy()
    {
        GameManager.Instance.onPointsUpdated.RemoveListener(PointsUpdated);
        GameManager.Instance.onGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if(newState == GameManager.GameState.GameOver)
        {
            SFXSource.PlayOneShot(gameOverSFX);
        }
        else if(newState == GameManager.GameState.InGame)
        {
            SFXSource.PlayOneShot(matchSFX);
        }
    }

    private void PointsUpdated()
    {
        SFXSource.PlayOneShot(matchSFX);
    }

    public void PieceMoved()
    {
        SFXSource.PlayOneShot(moveSFX);
    }

    public void MatchMissed()
    {
        SFXSource.PlayOneShot(missSFX);
    }
}
