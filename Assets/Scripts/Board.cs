using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Variables
    public int width;
    public int height;
    public float cameraSizeOffset;
    public float cameraVerticalOffset;
    public GameObject tileObject;

    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();
        SetCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {

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
            }
        }
    }
}
