using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    //Scripts
    public GridManager gridManager;
    public Move moveScript;

    //Game Variables
    public int turn = 0;
    public bool gameOver = false;

    //Pieces captured
    public List<Piece> WPiecesCaptured = new List<Piece>();
    public List<Piece> BPiecesCaptured = new List<Piece>();

    //Audio files
    #region Audio Variables
    public AudioSource BoardSource;
    public AudioClip GameStartAudio;
    public AudioClip MoveAudio;
    public AudioClip CastleAudio;
    public AudioClip CaptureAudio;
    #endregion
    #endregion

    public void Start()
    {
        //Access scripts
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        moveScript = GameObject.Find("GameManager").GetComponent<Move>();
        BoardSource.PlayOneShot(GameStartAudio);
    }

    //Checks for checks against color king (haha funny pun)
    public bool inCheck(string color)
    {
        //Checks every piece of opposing colors possible moves
        foreach (Piece piece in gridManager.pieces)
        {
            if(!piece.CompareTag(color) && !piece.CompareTag("Empty"))
            {
                foreach (Vector3 possibleMove in piece.possibleMoves)
                {
                    if (possibleMove == gridManager.returnKingPos(color))
                    {
                        Debug.Log("In Check");
                        return true;
                    }
                }
            }
            
        }
        return false;
    }

    //Return material advantages
    public int StatusEval()
    {
        int eval = 0;
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (gridManager.pieces[i, j].CompareTag("White"))
                {
                    if (gridManager.pieces[i, j].name.Contains("Pawn"))
                    {
                        eval += 1;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Bishop"))
                    {
                        eval += 3;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Knight"))
                    {
                        eval += 3;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Rook"))
                    {
                        eval += 5;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Queen"))
                    {
                        eval += 9;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("King"))
                    {
                        eval += 900;
                    }
                }

                if (gridManager.pieces[i, j].CompareTag("Black"))
                {
                    if (gridManager.pieces[i, j].name.Contains("Pawn"))
                    {
                        eval -= 1;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Bishop"))
                    {
                        eval -= 3;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Knight"))
                    {
                        eval -= 3;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Rook"))
                    {
                        eval -= 5;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("Queen"))
                    {
                        eval -= 9;
                    }
                    else if (gridManager.pieces[i, j].name.Contains("King"))
                    {
                        eval -= 900;
                    }
                }
            }
        }
        return eval;
    }

}
