using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Variables
    public int xPos;
    public int yPos;
    public Board gameBoard;

    public void Setup(int xPos_, int yPos_, Board gameBoard_)
    {
        //Define x and y coordinates and Board reference
        xPos = xPos_;
        yPos = yPos_;
        gameBoard = gameBoard_;
    }

    public void OnMouseDown()
    {
        gameBoard.TileClicked(this);
    }

    public void OnMouseEnter()
    {
        gameBoard.TileMoved(this);
    }

    public void OnMouseUp()
    {
        gameBoard.TileReleased(this);
    }
}
