using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Variables
    public float timeBetweenPieces = 0.05f;
    public int width;
    public int height;
    public float cameraSizeOffset;
    public float cameraVerticalOffset;
    public GameObject tileObject;
    public GameObject[] availablePieces;
    public int pointsPerMatch;
    Tile[,] boardTiles;
    GamePiece[,] boardPieces;
    Tile startTile;
    Tile endTile;
    bool arePiecesSwapping = false;

    void Start()
    {
        boardTiles = new Tile[width, height];
        boardPieces = new GamePiece[width, height];

        SetupBoard();
        SetCameraPosition();

        if (GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            StartCoroutine(SetupPieces());
        }
        GameManager.Instance.onGameStateUpdated.AddListener(OnGameStateUpdated);
    }

    void OnDestroy()
    {
        GameManager.Instance.onGameStateUpdated.RemoveListener(OnGameStateUpdated);
    }

    private void OnGameStateUpdated(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.InGame)
        {
            StartCoroutine(SetupPieces());
        }
        else if(newState == GameManager.GameState.GameOver)
        {
            ClearAllPieces();
        }
    }

    private IEnumerator SetupPieces()
    {
        int maxIterations = 50;
        int currentIteration = 0;

        //Draw board's rows
        for (int x = 0; x < width; x++)
        {
            //Draw board's columns
            for (int y = 0; y < height; y++)
            {
                yield return new WaitForSeconds(timeBetweenPieces);
                if (boardPieces[x, y] == null)
                {
                    currentIteration = 0;
                    var newPiece = CreateGamePiece(x, y);
                    while (HasPreviousMatches(x, y))
                    {
                        ClearPieceAt(x, y);
                        newPiece = CreateGamePiece(x, y);
                        currentIteration++;
                        if (currentIteration > maxIterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
        yield return null;
    }

    private void ClearPieceAt(int x, int y)
    {
        var pieceToClear = boardPieces[x, y];
        pieceToClear.RemovePiece(true);
        boardPieces[x, y] = null;
    }

    void ClearAllPieces()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                ClearPieceAt(x, y);
            }
        }
    }

    private GamePiece CreateGamePiece(int x, int y)
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)]; //Select random game piece to instantiate
        var o = Instantiate(selectedPiece, new Vector3(x, y + 1, -5f), Quaternion.identity); //Create new game piece instance 
        o.transform.parent = transform; //Set game piece instance as child of the board
        boardPieces[x, y] = o.GetComponent<GamePiece>(); //save GamePiece reference into boardPieces array
        boardPieces[x, y]?.SetGamePieces(x, y, this); //Set game piece coordinates in the game board
        boardPieces[x, y].Move(x, y);
        return boardPieces[x, y];
    }

    private void SetCameraPosition()
    {
        //Set orthographic view
        float newPosX = (float)width / 2f;
        float newPosY = (float)height / 2f;

        Camera.main.transform.position = new Vector3(newPosX - 0.5f, (newPosY - 0.5f + cameraVerticalOffset), -10);

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
                boardTiles[x, y] = o.GetComponent<Tile>(); //Save Tile reference in boardTiles array
                boardTiles[x, y]?.Setup(x, y, this); //Set tile coordinates in the game board 
            }
        }
    }

    private IEnumerator SwapTiles()
    {
        arePiecesSwapping = true;

        //Create piece references according to their position in board
        var startPiece = boardPieces[startTile.xPos, startTile.yPos];
        var endPiece = boardPieces[endTile.xPos, endTile.yPos];

        //Swap the pieces by changing their position values
        startPiece.Move(endTile.xPos, endTile.yPos);
        endPiece.Move(startTile.xPos, startTile.yPos);

        //Update piece coordinates in the grid
        boardPieces[startTile.xPos, startTile.yPos] = endPiece;
        boardPieces[endTile.xPos, endTile.yPos] = startPiece;

        yield return new WaitForSeconds(0.6f);

        var startMatches = GetMatchByPiece(startTile.xPos, startTile.yPos, 3);
        var endMatches = GetMatchByPiece(endTile.xPos, endTile.yPos, 3);
        var allMatches = startMatches.Union(endMatches).ToList();

        if (allMatches.Count == 0)
        {
            startPiece.Move(startTile.xPos, startTile.yPos);
            endPiece.Move(endTile.xPos, endTile.yPos);
            boardPieces[startTile.xPos, startTile.yPos] = startPiece;
            boardPieces[endTile.xPos, endTile.yPos] = endPiece;
        }
        else
        {
            ClearPieces(allMatches);
            GrantPoints(allMatches);
        }

        startTile = null;
        endTile = null;
        arePiecesSwapping = false;

        yield return null;
    }

    private void ClearPieces(List<GamePiece> piecesToClear)
    {
        piecesToClear.ForEach(piece =>
        {
            ClearPieceAt(piece.xPosition, piece.yPosition);
        });

        List<int> columns = GetColumns(piecesToClear);
        List<GamePiece> collapsedPieces = CollapseColumns(columns, 0.3f);
        FindMatchesRecursively(collapsedPieces);
    }

    private void FindMatchesRecursively(List<GamePiece> collapsedPieces)
    {
        StartCoroutine(FindComboMatches(collapsedPieces));
    }

    IEnumerator FindComboMatches(List<GamePiece> collapsedPieces)
    {
        yield return new WaitForSeconds(1.0f);
        List<GamePiece> newMatches = new List<GamePiece>();
        collapsedPieces.ForEach(piece =>
        {
            var matches = GetMatchByPiece(piece.xPosition, piece.yPosition, 3);
            if (matches != null)
            {
                newMatches = newMatches.Union(matches).ToList();
                ClearPieces(matches);
                GrantPoints(matches);
            }
        });
        if (newMatches.Count > 0)
        {
            var newCollapsedPieces = CollapseColumns(GetColumns(newMatches), 0.3f);
            FindMatchesRecursively(newCollapsedPieces);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SetupPieces());
            arePiecesSwapping = false;
        }
        yield return null;
    }

    private List<int> GetColumns(List<GamePiece> piecesToClear)
    {
        var result = new List<int>();
        piecesToClear.ForEach(piece =>
        {
            if (!result.Contains(piece.xPosition))
            {
                result.Add(piece.xPosition);
            }
        });
        return result;
    }

    private List<GamePiece> CollapseColumns(List<int> columns, float timeToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            for (int y = 0; y < height; y++)
            {
                if (boardPieces[column, y] == null)
                {
                    for (int yplus = y + 1; yplus < height; yplus++)
                    {
                        if (boardPieces[column, yplus] != null)
                        {
                            boardPieces[column, yplus].Move(column, y);
                            boardPieces[column, y] = boardPieces[column, yplus];
                            if (!movingPieces.Contains(boardPieces[column, y]))
                            {
                                movingPieces.Add(boardPieces[column, y]);
                            }
                            boardPieces[column, yplus] = null;
                            break;
                        }
                    }
                }
            }
        }
        return movingPieces;
    }

    public void TileClicked(Tile clickedTile_)
    {
        if (!arePiecesSwapping && GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            startTile = clickedTile_;
        }
    }

    public void TileMoved(Tile clickedTile_)
    {
        if (!arePiecesSwapping && GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            endTile = clickedTile_;
        }
    }

    public void TileReleased(Tile clickedTile_)
    {
        if (!arePiecesSwapping && GameManager.Instance.gameState == GameManager.GameState.InGame)
        {
            if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))
            {
                StartCoroutine(SwapTiles());
            }
        }

    }

    public bool IsCloseTo(Tile start_, Tile end_)
    {
        if (Math.Abs((start_.xPos - end_.xPos)) == 1 && start_.yPos == end_.yPos)
        {
            return true;
        }
        if (Math.Abs((start_.yPos - end_.yPos)) == 1 && start_.xPos == end_.xPos)
        {
            return true;
        }
        return false;
    }

    bool HasPreviousMatches(int posX, int posY)
    {
        var downMatch = GetMatchByDirection(posX, posY, new Vector2(0, -1), 2);
        var leftMatch = GetMatchByDirection(posX, posY, new Vector2(-1, 0), 2);

        if (downMatch == null) downMatch = new List<GamePiece>();
        if (leftMatch == null) leftMatch = new List<GamePiece>();

        return downMatch.Count > 0 || leftMatch.Count > 0;
    }

    public List<GamePiece> GetMatchByDirection(int xPosition, int yPosition, Vector2 direction, int minPieces = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece initialPiece = boardPieces[xPosition, yPosition];
        matches.Add(initialPiece);

        int nextX;
        int nextY;
        int maxVal = width > height ? width : height;

        for (int i = 1; i < maxVal; i++)
        {
            nextX = xPosition + ((int)direction.x * i);
            nextY = yPosition + ((int)direction.y * i);
            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {
                var nextPiece = boardPieces[nextX, nextY];
                if (nextPiece != null && nextPiece.pieceType == initialPiece.pieceType)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }
        if (matches.Count >= minPieces)
        {
            return matches;
        }
        return null;
    }

    public List<GamePiece> GetMatchByPiece(int xPosition_, int yPosition_, int minPieces_ = 3)
    {
        var upMatch = GetMatchByDirection(xPosition_, yPosition_, new Vector2(0, 1), 2);
        var downMatch = GetMatchByDirection(xPosition_, yPosition_, new Vector2(0, -1), 2);
        var rightMatch = GetMatchByDirection(xPosition_, yPosition_, new Vector2(1, 0), 2);
        var leftMatch = GetMatchByDirection(xPosition_, yPosition_, new Vector2(-1, 0), 2);

        if (upMatch == null) upMatch = new List<GamePiece>();
        if (downMatch == null) downMatch = new List<GamePiece>();
        if (rightMatch == null) rightMatch = new List<GamePiece>();
        if (leftMatch == null) leftMatch = new List<GamePiece>();

        var verticalMatch = upMatch.Union(downMatch).ToList();
        var horizontalMatch = leftMatch.Union(rightMatch).ToList();

        var foundMatches = new List<GamePiece>();
        if (verticalMatch.Count >= minPieces_)
        {
            foundMatches = foundMatches.Union(verticalMatch).ToList();
        }
        if (horizontalMatch.Count >= minPieces_)
        {
            foundMatches = foundMatches.Union(horizontalMatch).ToList();
        }

        return foundMatches;
    }

    public void GrantPoints(List<GamePiece> allMatches)
    {
        GameManager.Instance.AddPoints(allMatches.Count * pointsPerMatch);
    }
}
