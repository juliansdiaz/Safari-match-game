using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
}
