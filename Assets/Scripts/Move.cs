using UnityEngine;

public class Move : MonoBehaviour
{
    #region Variables
    //Scripts
    public GridManager gridManager;
    public GameManager gameManager;

    //Empty pieces used to replace old position
    [SerializeField] private Piece empty;
    #endregion

    void Start()
    {
        //Access scripts
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    #region Methods
    //Moves piece from _oldPos to _newPos
    public void MovePiece(Vector3 _old, Vector3 _new)
    {
        //Access current pieces "Piece" script
        Piece pieceController = gridManager.pieces[(int)_old.x, (int)_old.y].GetComponent<Piece>();

        //Checks if new position is occupied by opposing color
        //If so, capture
        if (gridManager.pieces[(int)_old.x, (int)_old.y].opposingColor(gridManager.pieces[(int)_new.x, (int)_new.y]))
        {
            Capture(_old, _new);
            Debug.Log("Capture");
            gameManager.BoardSource.PlayOneShot(gameManager.CaptureAudio);
        }
        //Checks if new position is empty
        else if (gridManager.pieces[(int)_new.x, (int)_new.y].name.Contains("Empty"))
        {
            //Checks for castle conditions
            if (gridManager.pieces[(int)_old.x, (int)_old.y].name.Contains("King") && Mathf.Abs(_new.x - _old.x) == 2)
            {
                Castle(_old, _new);
                gameManager.BoardSource.PlayOneShot(gameManager.CastleAudio);
            }
            else
            {
                gameManager.BoardSource.PlayOneShot(gameManager.MoveAudio);
            }

            //Moves piece to new position 
            //Then, replaces old position with empty object
            gridManager.pieces[(int)_old.x, (int)_old.y].transform.position = new Vector3(Mathf.Round(_new.x), Mathf.Round(_new.y));
            gridManager.pieces[(int)_new.x, (int)_new.y] = gridManager.pieces[(int)_old.x, (int)_old.y];
            gridManager.pieces[(int)_old.x, (int)_old.y] = empty;
        }

        //Increment pieces move count
        pieceController.moveCount++;
        //Updates piece move list
        UpdatePossibleMoves();
        //Initiates next turn
        gameManager.turn++;
    }

    //Piece at _old captures piece at _new
    private void Capture(Vector3 _old, Vector3 _new)
    {
        //Adds captured pieces to captured lists
        if(gridManager.pieces[(int)_new.x, (int)_new.y].CompareTag("White"))
        {
            gameManager.WPiecesCaptured.Add(gridManager.pieces[(int)_new.x, (int)_new.y]);
        }
        else
        {
            gameManager.BPiecesCaptured.Add(gridManager.pieces[(int)_new.x, (int)_new.y]);
        }

        //Access captured piece components
        //"Destroys" captured piece
        gridManager.pieces[(int)_new.x, (int)_new.y].GetComponent<SpriteRenderer>().enabled = false;
        gridManager.pieces[(int)_new.x, (int)_new.y].GetComponent<BoxCollider2D>().enabled = false;

        //Moves capturing piece to new position
        gridManager.pieces[(int)_old.x, (int)_old.y].transform.position = new Vector3(Mathf.Round(_new.x), Mathf.Round(_new.y));
        gridManager.pieces[(int)_new.x, (int)_new.y] = empty;
        gridManager.pieces[(int)_new.x, (int)_new.y] = gridManager.pieces[(int)_old.x, (int)_old.y];
        gridManager.pieces[(int)_old.x, (int)_old.y] = empty;
        Debug.Log(gridManager.pieces[(int)_new.x, (int)_new.y]);
    }

    //Castling king from _old to _new
    private void Castle(Vector3 _old, Vector3 _new)
    {
        //Castling king side
        if (_new.x - _old.x > 0)
        {
            gridManager.pieces[7, (int)_old.y].transform.position = new Vector3(5, Mathf.Round(_old.y));
            gridManager.pieces[5, (int)_old.y] = gridManager.pieces[7, (int)_old.y];
            gridManager.pieces[7, (int)_old.y] = empty;
        }
        //Castling queen side
        else
        {
            gridManager.pieces[0, (int)_old.y].transform.position = new Vector3(3, Mathf.Round(_old.y));
            gridManager.pieces[3, (int)_old.y] = gridManager.pieces[0, (int)_old.y];
            gridManager.pieces[0, (int)_old.y] = empty;
        }
        Debug.Log("Castle");
    }

    //Updates the possible moves for all pieces on the board
    private void UpdatePossibleMoves()
    {
        foreach (Piece piece in gridManager.pieces)
        {
            if(piece.tag != "Empty")
            {
                piece.generatePossibleMoves();
            }
        }
    }


    #endregion
}
