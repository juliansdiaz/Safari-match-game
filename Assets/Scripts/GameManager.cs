using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public int points = 0;
    public UnityEvent onPointsUpdated;

    public void AddPoints(int newPoints)
    {
        points += newPoints;
        onPointsUpdated?.Invoke(); 
    }
}
