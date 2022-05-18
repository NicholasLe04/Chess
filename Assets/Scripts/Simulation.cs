using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    #region Variables
    //Scripts
    public GridManager gridManager;
    public GameManager gameManager;
    public Move moveScript;

    //Empty piece
    [SerializeField]
    private Piece empty;

    //Pieces stored in simulation
    Piece startPiece;
    Piece endPiece;
    #endregion

    void Start()
    {
        //Access scripts
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        moveScript = GameObject.Find("GameManager").GetComponent<Move>();
    }

    public void SimulateMove(Vector3 start, Vector3 finish)
    {
        startPiece = gridManager.pieces[(int)start.x, (int)start.y];
        endPiece = gridManager.pieces[(int)finish.x, (int)finish.y];
        
        gridManager.pieces[(int)finish.x, (int)finish.y] = gridManager.pieces[(int)start.x, (int)start.y];
        gridManager.pieces[(int)start.x, (int)start.y] = empty;
    }

    public void UnsimulateMove(Vector3 start, Vector3 finish)
    {
        gridManager.pieces[(int)start.x, (int)start.y] = startPiece;
        gridManager.pieces[(int)finish.x, (int)finish.y] = endPiece;
        startPiece = null;
        endPiece = null;
    }
}
