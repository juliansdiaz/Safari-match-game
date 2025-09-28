using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Variables
    public int width;
    public int height;
    public float cameraSizeOffset;
    public float cameraVerticalOffset;
    public GameObject tileObject;
    public GameObject[] availablePieces;

    void Start()
    {
        SetupBoard();
        SetCameraPosition();
        SetupPieces();
    }

    void Update()
    {

    }

    private void SetupPieces()
    {
        //Draw board's rows
        for (int x = 0; x < width; x++)
        {
            //Draw board's columns
            for (int y = 0; y < height; y++)
            {
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)]; //Select random game piece to instantiate
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5f), Quaternion.identity); //Create new game piece instance 
                o.transform.parent = transform; //Set game piece instance as child of the board
                o.GetComponent<GamePiece>()?.SetGamePieces(x, y, this); //Set game piece coordinates in the game board 
            }
        }
    }

    private void SetCameraPosition()
    {
        //Set orthographic view
        float newPosX = (float)width / 2f;
        float newPosY = (float)height / 2f;

        Camera.main.transform.position = new Vector3 (newPosX - 0.5f, (newPosY - 0.5f + cameraVerticalOffset), -10);

        //Set camera margins 
        float hMargin = width + 1;
        float vMargin = (height / 2) + 1;

        Camera.main.orthographicSize = hMargin > vMargin ? hMargin + cameraSizeOffset : vMargin + cameraSizeOffset;
    }

    private void SetupBoard()
    {   
        //Draw board's rows
        for (int x = 0; x < width; x++)
        {
            //Draw board's columns
            for (int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5f), Quaternion.identity); //Create new tile instance 
                o.transform.parent = transform; //Set tile instance as child of the board
                o.GetComponent<Tile>()?.Setup(x, y, this); //Set tile coordinates in the game board 
            }
        }
    }
}
