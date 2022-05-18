using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private int width, height;

    #region Graphical Elements

    [SerializeField] private Tile tilePrefab;
    #region WhitePieces
    [SerializeField] private Piece W_Pawn;
    [SerializeField] private Piece W_Bishop;
    [SerializeField] private Piece W_Knight;
    [SerializeField] private Piece W_Queen;
    [SerializeField] private Piece W_King;
    [SerializeField] private Piece W_Rook;
    #endregion
    #region BlackPieces
    [SerializeField] private Piece B_Pawn;
    [SerializeField] private Piece B_Bishop;
    [SerializeField] private Piece B_Knight;
    [SerializeField] private Piece B_Queen;
    [SerializeField] private Piece B_King;
    [SerializeField] private Piece B_Rook;
    #endregion
    [SerializeField] public Piece Empty;

    #endregion

    [SerializeField] private Transform cam;

    public Piece[,] pieces = new Piece[8, 8];
    public Piece[,] storedState = new Piece[8, 8];

    private Dictionary<Vector3, Tile> tiles;
    #endregion


    private void Start()
    {
        GenerateGrid();
        GeneratePieces();
    }
    

    #region Methods
    
    //Generates tiles
    void GenerateGrid()
    {
        tiles = new Dictionary<Vector3, Tile>();

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0.01f), Quaternion.identity);
                spawnedTile.name = $"Tile {x + 1} {y + 1}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.colored(isOffset);

                tiles[new Vector3(x, y)] = spawnedTile;
            }
        }
        cam.transform.position = new Vector3((float)width / 2 -0.5f, (float)height / 2 - 0.5f, -10);
    }
    
    //Returns the tile at pos
    public Tile GetTileAtPosition(Vector3 pos)
    {
        if(tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    //Instantiates pieces and stores them in "pieces" array
    void GeneratePieces()
    {
        for (int x = 0; x < width; x++)
        {
            var spawnedPieceW = Instantiate(W_Pawn, new Vector3(x, 1), Quaternion.identity);
            var spawnedPieceB = Instantiate(B_Pawn, new Vector3(x, 6), Quaternion.identity);
            pieces[x, 1] = spawnedPieceW;
            pieces[x, 6] = spawnedPieceB;
        }
        for(int i = 0; i < 8; i++)
        {
            for (int j = 2; j < 6; j++)
            {
                var spawnedEmpty = Instantiate(Empty, new Vector3(i, j), Quaternion.identity);
                pieces[i, j] = spawnedEmpty;
            }
        }

        pieces[0, 0] = Instantiate(W_Rook, new Vector3(0, 0), Quaternion.identity);
        pieces[7, 0] = Instantiate(W_Rook, new Vector3(7, 0), Quaternion.identity);
        pieces[1, 0] = Instantiate(W_Knight, new Vector3(1, 0), Quaternion.identity);
        pieces[6, 0] = Instantiate(W_Knight, new Vector3(6, 0), Quaternion.identity);
        pieces[2, 0] = Instantiate(W_Bishop, new Vector3(2, 0), Quaternion.identity);
        pieces[5, 0] = Instantiate(W_Bishop, new Vector3(5, 0), Quaternion.identity);
        pieces[3, 0] = Instantiate(W_Queen, new Vector3(3, 0), Quaternion.identity);
        pieces[4, 0] = Instantiate(W_King, new Vector3(4, 0), Quaternion.identity);

        pieces[0, 7] = Instantiate(B_Rook, new Vector3(0, 7), Quaternion.identity);
        pieces[7, 7] = Instantiate(B_Rook, new Vector3(7, 7), Quaternion.identity);
        pieces[1, 7] = Instantiate(B_Knight, new Vector3(1, 7), Quaternion.identity);
        pieces[6, 7] = Instantiate(B_Knight, new Vector3(6, 7), Quaternion.identity);
        pieces[2, 7] = Instantiate(B_Bishop, new Vector3(2, 7), Quaternion.identity);
        pieces[5, 7] = Instantiate(B_Bishop, new Vector3(5, 7), Quaternion.identity);
        pieces[3, 7] = Instantiate(B_Queen, new Vector3(3, 7), Quaternion.identity);
        pieces[4, 7] = Instantiate(B_King, new Vector3(4, 7), Quaternion.identity);
    }

    //Stores current state of board under "storedState"
    public void StoreState()
    {
        for(int x = 0; x < 7; x++)
        {
            for(int y = 0; y < 7; y++)
            {
                storedState[x, y] = pieces[x, y];
            }
        }
    }

    //Returns board to "storedState"
    public void returnToStored()
    {
        for(int x = 0; x < 7; x++)
        {
            for(int y = 0; y < 7; y++)
            {
                pieces[x, y] = storedState[x, y];
            }
        }
    }

    //Returns position of the king of "color"
    public Vector3 returnKingPos(string color)
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(pieces[i, j].name.Contains("King") && pieces[i, j].CompareTag(color))
                {
                    return pieces[i, j].transform.position;
                }
            }
        }
        return new Vector3(0, 0, 0);
    }

    #endregion
}