using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetronimoData data;
    public Board board;
    public Vector2Int[] cells;
    public Vector2Int position;
    public bool freeze = false;
    int activeCellCount = -1;
    public bool BackDrop = false;
    public void Initialize(Board board, Tetronimo tetronimo)
    {
        //sets ref to board
        //blegh 
        this.board = board;

        //search for tetro data and assign waugh
        for (int i = 0; i < board.tetronimos.Length; i++)
        {
            if (board.tetronimos[i].tetronimo == tetronimo)
            {
                this.data = board.tetronimos[i];
                break;
            }
        }

        //creat a copy of the stupid cell locations
        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];


        //i hate life
        //set the starting posisition of these stupid ass pieces
        position = board.startPosition;

        activeCellCount = cells.Length;
    }

    private void Update()
    {
        //if the stupid lil dumb itdiot piece is frozen, do NOT process the update loop, and do not collect 200 dollars
        //Debug.Log(board.tetrisManager.gameOver);
        //if (board.tetrisManager.gameOver == null) return;
        if (board.tetrisManager.gameOver) return;
        if (BackDrop)
        {
            board.Clear(this);
            board.Set(this);
            BackDrop = false;
        }
        if (freeze) return;
        board.Clear(this);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.down);
            }
            // TBD for stupid debugging stupids
            //else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    Move(Vector2Int.up);
            //}

            //roationnnnnnn
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Rotate(1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Rotate(-1);
            }
        }

        board.Set(this);

        //TBDDDDDDDDD Stuprid fucking debug only - P is thestupid fukcing debug key
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }
        //only chekc the stupid board and spawn the stupid new piece AFTer teh stupid final piece has been frozen
        if (freeze)
        {
            board.CheckBoard();
            board.SpawnPiece();
        }
    }

    void Rotate(int direction)
    {
        //store those fucky lil cell locations, so we can do the funky lil thing of reverting it
        Vector2Int[] temporaryCells = new Vector2Int[cells.Length];

        for (int i = 0; i < cells.Length; i++) temporaryCells[i] = cells[i];

        ApplyRotation(direction);

        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(temporaryCells);
            }
            else
            {
                Debug.Log("Wall Kick succeeded");
            }
        }
    }
    bool TryWallKicks()
    {
        List<Vector2Int> wallKickOffsets = new List<Vector2Int>()
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down,
            new Vector2Int(-1, -1), //diagonal down-left bweugh
            new Vector2Int(1, -1) //diagonal down-right or whatever
        };

        //a dumb lil ad-hoc solution for the dumb lil I piece
        if (data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.Custom)
        {
            wallKickOffsets.Add(2 * Vector2Int.left);
            wallKickOffsets.Add(2 * Vector2Int.right);
        }
        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset)) return true;
        }

        return false;
    }
    void RevertRotation(Vector2Int[] TemporaryCells)
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = TemporaryCells[i];
    }

    void ApplyRotation(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90.0f * direction);

        bool isSpecial = data.tetronimo == Tetronimo.I || data.tetronimo == Tetronimo.O;
        for (int i = 0; i < cells.Length; i++)
        {
            //JUST the SILLY lil cell position
            Vector2Int cellPosition = cells[i];

            //cast it unto the Vector3 cuz they be working like that
            Vector3 cellPositionV3 = new Vector3(cellPosition.x, cellPosition.y);

            if (isSpecial)
            {
                cellPositionV3.x -= 0.5f;
                cellPositionV3.y -= 0.5f;
            }

            Vector3 result = rotation * cellPositionV3;

            //take the gosh darng result and apply to that gosh darng cell data
            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i] = new Vector2Int(
                    Mathf.RoundToInt(result.x),
                    Mathf.RoundToInt(result.y));
            }
        }
    }
    void HardDrop()
    {
        //keep moving down until the the beautiful thing called a translation is invalid
        while (Move(Vector2Int.down))
        {
            //do absolutely NOTHIN
        }

        freeze = true;
    }

    //move will return whether or not the silly lil translation is valid
    public bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position;
        newPosition += translation;

        bool isValid = board.IsPositionValid(this, newPosition);
        if (isValid) position = newPosition;

        return isValid;
    }

    public void ReduceActiveCount()
    {
        activeCellCount -= 1;
        Debug.Log($"Tetrinomo {data.tetronimo} active cell count = {activeCellCount} ");
        if (activeCellCount <= 0)
        {
            Debug.Log($"Tetronimo {data.tetronimo} destroyed");
            Destroy(gameObject);
        }
    }
}
