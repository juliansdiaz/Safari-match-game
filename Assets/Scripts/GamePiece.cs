using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    //Variables
    public int xPosition;
    public int yPosition;
    public Board gBoard;

    //Game piece types
    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    }

    public type pieceType;

    public void SetGamePieces(int xPos_, int yPos_, Board gameBoard_)
    {
        xPosition = xPos_;
        yPosition = yPos_;
        gBoard = gameBoard_;
    }
}
