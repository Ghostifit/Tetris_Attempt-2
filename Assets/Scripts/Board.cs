using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetrisManager tetrisManager;
    public TetronimoData[] tetronimos;
    //contains a beautiful lil reference to the "piece" prefab
    public Piece piecePrefab;
    public Tilemap tilemap;
    public Vector2Int boardSize;
    public Vector2Int startPosition;
    //maps the stupid tilemap postion to the stupid piece gameobject 
    //raaaaaaaaaaaaaaah
    public bool firstRun = true;
    public float dropInterval = 0.5f;
    float DropTime = 0.0f;
    List<Tetronimo> pieceOrder = new List<Tetronimo>()
    {   Tetronimo.Z, 
        Tetronimo.T,
        Tetronimo.Custom,
        Tetronimo.I,
        Tetronimo.Custom,
        Tetronimo.Custom,
        Tetronimo.O,
        Tetronimo.J,
        Tetronimo.S,
        Tetronimo.T,
        Tetronimo.T,
        Tetronimo.L,
        Tetronimo.Custom,
        Tetronimo.Custom
    };



    Dictionary<Vector3Int, Piece> pieces = new Dictionary<Vector3Int, Piece>();

    Piece activePiece;

    int left
    {
        get
        { return -boardSize.x / 2; }
    }
    int right
    {
        get
        { return boardSize.x / 2; }
    }
    int top
    {
        get
        { return boardSize.y / 2; }
    }
    int bottom
    {
        get
        { return -boardSize.y / 2; }
    }

    private void Update()
    {
        if (tetrisManager.gameOver) return;

        DropTime += Time.deltaTime;

        if (DropTime >= dropInterval)
        {
            DropTime = 0.0f;

            Clear(activePiece);
            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            //if the darn move fails then that means that the rootin tootin pieces is BLOCKED
            //like my ex wife
            //I don't have an ex wife
            //its also ends the placement
            if (!moveResult)
            {
                activePiece.freeze = true;
                CheckBoard();
                SpawnPiece();
            }
        }
    }

    public void SpawnPiece()
    {
        activePiece = Instantiate(piecePrefab);
        //spawns a random tetromnio or smthng 

        
        if (pieceOrder.Count > 0)
        {
            Tetronimo t = pieceOrder[0];
            activePiece.Initialize(this, t);
            pieceOrder.RemoveAt(0);
        }
        else if (pieceOrder.Count == 0)
        {

            if(pieces.Count == 0)
            {
                tetrisManager.SetGameOver(true, true);
            }
            else
            {
                tetrisManager.SetGameOver(true, false);
            }
            Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);
            activePiece.Initialize(this, t);
        }
        else
        {
            Tetronimo t = (Tetronimo)Random.Range(0, tetronimos.Length);
            activePiece.Initialize(this, t);
        }



        CheckEndGame();
        Set(activePiece);
    }


    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            //if there is so silly valid place for the new silly piece makes it a game overrrrr
            tetrisManager.SetGameOver(true, false);
        }
    }

    public void UpdateGameOver()
    {
        //either started a new silly game
        //or reset the gosh darn game
        if (!tetrisManager.gameOver)
        {
            ResetBoard();
        }
    }

    void ResetBoard()
    {
       
        if (firstRun)
        {
            firstRun = false;
        }
        else
        {
            Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsSortMode.None);
            foreach (Piece piece in foundPieces) Destroy(piece.gameObject);

            activePiece = null;
            tilemap.ClearAllTiles();
            pieces.Clear();
        }
        

        SpawnPiece();
    }
    void SetTile(Vector3Int cellPosition, Piece piece)
    {
        if (piece == null)
        {
            tilemap.SetTile(cellPosition, null);
            
            pieces.Remove(cellPosition);
        }
        else
        {
            tilemap.SetTile(cellPosition, piece.data.tile);
            //this stupid line is creating a stupid assoctiation
            //with the stupid cell postion and the dumb game object
            pieces[cellPosition] = piece;
        }
    }
    //set will coliur the stupid pieces
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
           SetTile(cellPosition, piece);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            //dumb bounds chekc agshfgdgdgfgs
            if (cellPosition.x < left || cellPosition.x >= right ||
                cellPosition.y < bottom || cellPosition.y >= top) return false;

            //checks if this stu[pid position is fuckin occupied in tile map
            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    public void CheckBoard()
    {
        List<int> destroyedLines = new List<int>();
        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }

        //Debug.Log(destroyedLines);

        int rowsShiftedDown = 0;
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);

            //at the end of every stupid loop we shift the stupid rows down by 1 more
            //fuck my stupid baka life
            rowsShiftedDown++;
        }

        //finally ready to set that funky wunky lil gosh darn score
        int score = tetrisManager.calculateScore(destroyedLines.Count);
        
        tetrisManager.ChangeScore(score);



        //Debug.Log($"Lines Destroyed: {destroyedLines.Count}");

        //the studpid shift down goes here 
        //foreach (int clearedRow in destroyedLines)
        //{
        //    ShiftRowsDown(clearedRow);
        //}
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);

                if (pieces.ContainsKey(cellPosition))
                {
                    //store the stupid piece temproitly
                    Piece currentPiece = pieces[cellPosition];

                    //clear the FWICKING position it FWICKING is in
                    SetTile(cellPosition, null);

                    //move the dumb tile down
                    cellPosition.y -= 1;
                    SetTile(cellPosition, currentPiece);
                }
            }
        }
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);

            if (!tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    void DestroyLine(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            if (pieces.ContainsKey(cellPosition))
            {
                Piece piece = pieces[cellPosition];
                piece.ReduceActiveCount();
                SetTile(cellPosition, null);
            }
            }
        }
}
