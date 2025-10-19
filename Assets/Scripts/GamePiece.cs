using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        transform.localScale = Vector3.one * 0.35f;
        transform.DOScale(Vector3.one, 0.35f);
    }

    public void Move(int xDestination, int yDestination)
    {
        transform.DOMove(new Vector3(xDestination, yDestination, -5f), 0.25f).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            xPosition = xDestination;
            yPosition = yDestination;
        };
    }

    public void RemovePiece(bool animated)
    {
        if (animated)
        {
            transform.DORotate(new Vector3(0,0,-120f), 0.12f);
            transform.DOScale(Vector3.one * 1.2f, 0.085f).onComplete = () =>
            {
                transform.DOScale(Vector3.zero, 0.1f).onComplete = () =>
                {
                    Destroy(gameObject);
                };
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
