using System.Collections.Generic;
using UnityEngine;
public class Piece : MonoBehaviour
{
    #region Variables
    public int moveCount = 0;
    Vector3 currentPosition;

    //Scripts
    public GridManager gridManager;
    public GameManager gameManager;
    public Move moveScript;
    public Simulation simulate;

    //IN PROGRESS
    private bool epVulnerable = true; 

    //Holds all possible moves for each piece
    public List<Vector3> possibleMoves = new List<Vector3>();
    #endregion


    public void Start()
    {
        //Access scripts
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        moveScript = GameObject.Find("GameManager").GetComponent<Move>();
        simulate = GameObject.Find("GameManager").GetComponent<Simulation>();
        generatePossibleMoves();
    }

 
    #region Mouse methods
    //Drag piece with cursor
    public void OnMouseDrag()
    {
        transform.position = GetMousePos();
    }

    //Select piece on mouse click
    //Highlight possible move squares
    public void OnMouseDown()
    {
        currentPosition = new Vector3(Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
        highlightOpenSquares();
    }
    
    //On mouse up:
    //- Check mouse position
    //- If legal position, move piece
    //- Unhighlight possible move squares
    public void OnMouseUp()
    {
        //Check position of mouse up
        Vector3 newPosition = new Vector3(Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));

        //If legal, move piece
        if (verifyMove(currentPosition, newPosition))
        {
            moveScript.MovePiece(currentPosition, newPosition);
            generatePossibleMoves(); //generate moves
        }
        else
        {
            transform.position = currentPosition; //set piece position
        }
        unhighlightOpenSquares();
    }

    //Finds position of cursor on screen
    public Vector3 GetMousePos()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
    #endregion


    #region Methods
    //Checks the piece at oldPos to possible move to newPos
    public bool verifyMove(Vector3 oldPos, Vector3 newPos)
    {
        //Delta variables
        int deltaX = (int)(newPos.x - oldPos.x);
        int deltaY = (int)(newPos.y - oldPos.y);
        
        if(gameManager.inCheck("White") && gameObject.CompareTag("White"))
        {
            return false;
        }
        /*
         * Checks if:
         *newPos in range of board
         *if its the right turn
         */
        if ((newPos.x >= 0 && newPos.x <= 7) && (newPos.y >= 0 && newPos.y <= 7) && newPos != oldPos && ((gameManager.turn % 2 == 0 && gameObject.tag == "White") || (gameManager.turn % 2 == 1 && gameObject.tag == "Black")))
        {
            #region Pawn Move Verification
            if (gameObject.name.Contains("Pawn"))
                {
                    //if piece moves 1 square forward into an empty square
                    if (((gameObject.tag == "White" && deltaY == 1) || (gameObject.tag == "Black" && deltaY == -1)) && deltaX == 0 && gridManager.pieces[(int)newPos.x, (int)newPos.y].tag == "Empty")
                    {
                        return true;                     
                    }
                    //if piece moves 2 square forward on first move
                    else if (((gameObject.tag == "White" && deltaY == 2) || (gameObject.tag == "Black" && deltaY == -2)) && deltaX == 0 && gridManager.pieces[(int)newPos.x, (int)newPos.y].tag == "Empty" && moveCount == 0 && !pieceInTheWay(oldPos, newPos))
                    {
                        epVulnerable = false;
                        /*
                         * EN PASSANT
                         * IN PROGRESS
                         */
                        return true;
                    }
                    //if piece moves diagonally on opposing piece
                    else if (gridManager.pieces[(int)oldPos.x, (int)oldPos.y].opposingColor(gridManager.pieces[(int)newPos.x, (int)newPos.y]) && Mathf.Abs(deltaX) == 1 && ((gameObject.tag == "White" && deltaY == 1) || (gameObject.tag == "Black" && deltaY == -1)))
                    {
                        return true;
                    }
                    return false;
                }
            #endregion

            #region Rook Move Verification
            if (gameObject.name.Contains("Rook"))
            {
                //Checks if rook moving in straight line
                if ((oldPos.x == newPos.x && oldPos.y != newPos.y) || (oldPos.x != newPos.x && oldPos.y == newPos.y))
                {
                    //checks if new position is valid
                    //and if there is not piece in the way
                    if (!gridManager.pieces[(int)newPos.x, (int)newPos.y].sameColor(gridManager.pieces[(int)oldPos.x, (int)oldPos.y]) && !pieceInTheWay(oldPos, newPos))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            #endregion

            #region Knight Move Verification
            else if (gameObject.name.Contains("Knight"))
            {
                //Checks if knight moves in L-shape
                if (((newPos.x == oldPos.x + 2 || newPos.x == oldPos.x - 2) && (newPos.y == oldPos.y + 1 || newPos.y == oldPos.y - 1)) || ((newPos.y == oldPos.y + 2 || newPos.y == oldPos.y - 2) && (newPos.x == oldPos.x + 1 || newPos.x == oldPos.x - 1)))
                {
                    //If new position isn't occupied by same color
                    if (!gridManager.pieces[(int)newPos.x, (int)newPos.y].sameColor(gridManager.pieces[(int)oldPos.x, (int)oldPos.y]))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            #endregion

            #region Bishop Move Verification
            else if (gameObject.name.Contains("Bishop"))
            {
                //Checks if bishop moves diagonally
                if (Mathf.Abs(newPos.x - oldPos.x) == Mathf.Abs(newPos.y - oldPos.y))
                {
                    //checks if new position is valid
                    //and if there is not piece in the way
                    if (!gridManager.pieces[(int)newPos.x, (int)newPos.y].sameColor(gridManager.pieces[(int)oldPos.x, (int)oldPos.y]) && !pieceInTheWay(oldPos, newPos))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            #endregion

            #region Queen Move Verification
            if (gameObject.name.Contains("Queen"))
            {
                //Checks if queen moving in straight line or diagonally
                if (((oldPos.x == newPos.x && oldPos.y != newPos.y) || (oldPos.x != newPos.x && oldPos.y == newPos.y)) || Mathf.Abs(newPos.x - oldPos.x) == Mathf.Abs(newPos.y - oldPos.y))
                {
                    //checks if new position is valid
                    //and if there is not piece in the way
                    if (!gridManager.pieces[(int)newPos.x, (int)newPos.y].sameColor(gridManager.pieces[(int)oldPos.x, (int)oldPos.y]) && !pieceInTheWay(oldPos, newPos))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            #endregion

            #region King Move Verification
            if (gameObject.name.Contains("King"))
            {
                //Checks king default movement range
                if (Mathf.Abs(deltaX) < 2 && Mathf.Abs(deltaY) < 2 && oldPos != newPos)
                {
                    if (!gridManager.pieces[(int)newPos.x, (int)newPos.y].sameColor(gridManager.pieces[(int)oldPos.x, (int)oldPos.y])) 
                    {
                        return true;
                    }
                }
                //Checks castling movement for king
                else if (deltaY == 0 && Mathf.Abs(deltaX) == 2 && moveCount == 0 && gridManager.pieces[(int)newPos.x, (int)newPos.y].name.Contains("Empty") && !pieceInTheWay(oldPos, newPos))
                {
                    //Checks right castling
                    //Checks right rook conditions
                    if (deltaX == 2 && gridManager.pieces[7, (int)oldPos.y].name.Contains("Rook") && gridManager.pieces[7, (int)oldPos.y].moveCount == 0)
                    {
                        return true;
                    }
                    //Checks left castling
                    //Checks left rook conditions
                    else if (deltaX == -2 && gridManager.pieces[0, (int)oldPos.y].name.Contains("Rook") && gridManager.pieces[0, (int)oldPos.y].moveCount == 0)
                    {
                        return true;
                    }
                }
            }
            #endregion
        }
        return false;
    }

    //Highlights all possible moves
    void highlightOpenSquares()
    {
        if (gameObject.tag != "Empty")
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (verifyMove(currentPosition, new Vector3(x, y)))
                    {
                        gridManager.GetTileAtPosition(new Vector3(x, y)).GetComponent<Tile>().highlight.SetActive(true);
                    }
                }
            }
        }
    }

    //Unhighlight all highlighted squares
    void unhighlightOpenSquares()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                   
                gridManager.GetTileAtPosition(new Vector3(x, y)).GetComponent<Tile>().highlight.SetActive(false);
                    
            }
        }
    }

    //Checks for pieces in way of sliding movements
    bool pieceInTheWay(Vector3 start, Vector3 finish)
    {
        //Delta variables
        float deltaX = finish.x - start.x;
        float deltaY = finish.y - start.y;
        
        //Bishop check
        if (gridManager.pieces[(int)start.x, (int)start.y].name.Contains("Bishop"))
        {
            if (deltaX == 0 || deltaY == 0)
            {
                return false;
            }
            else if(deltaX > 1)
            {
                for (int i = 1; i < deltaX; i++)
                {
                    if (deltaY > 1) 
                    {
                        if (!gridManager.pieces[(int)start.x + i, (int)start.y + i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (!gridManager.pieces[(int)start.x + i, (int)start.y - i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (deltaX < 0)
            {
                for (int i = 1; i < Mathf.Abs(deltaX); i++)
                {
                    if (deltaY > 1)
                    {
                        if (!gridManager.pieces[(int)start.x - i, (int)start.y + i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (!gridManager.pieces[(int)start.x - i, (int)start.y - i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //Rook check
        else if (gridManager.pieces[(int)start.x, (int)start.y].name.Contains("Rook"))
        {
            if (deltaY > 1)
            {
                for (int i = 1; i < deltaY; i++)
                {
                    if (!gridManager.pieces[(int)start.x, (int)start.y + i].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }

            if (deltaY < 0)
            {
                for (int i = 1; i < Mathf.Abs(deltaY); i++)
                {
                    if (!gridManager.pieces[(int)start.x, (int)start.y - i].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }

            if (deltaX > 1)
            {
                for (int i = 1; i < deltaX; i++)
                {
                    if (!gridManager.pieces[(int)start.x + i, (int)start.y].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }

            if (deltaX < 0)
            {
                for (int i = 1; i < Mathf.Abs(deltaX); i++)
                {
                    if (!gridManager.pieces[(int)start.x - i, (int)start.y].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        //Queen check
        else if (gridManager.pieces[(int)start.x, (int)start.y].name.Contains("Queen"))
        {

            //Bishop
            if (Mathf.Abs(finish.x - start.x) == Mathf.Abs(finish.y - start.y))
            {
                if (deltaX > 1)
                {
                    for (int i = 1; i < deltaX; i++)
                    {
                        if (deltaY > 1)
                        {
                            if (!gridManager.pieces[(int)start.x + i, (int)start.y + i].name.Contains("Empty"))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (!gridManager.pieces[(int)start.x + i, (int)start.y - i].name.Contains("Empty"))
                            {
                                return true;
                            }
                        }
                    }
                }
                else if (deltaX < 0)
                {
                    for (int i = 1; i < Mathf.Abs(deltaX); i++)
                    {
                        if (deltaY > 1)
                        {
                            if (!gridManager.pieces[(int)start.x - i, (int)start.y + i].name.Contains("Empty"))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (!gridManager.pieces[(int)start.x - i, (int)start.y - i].name.Contains("Empty"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            //Rook
            if ((deltaX != 0 && deltaY == 0) || (deltaY != 0 && deltaX == 0))
            {
                if (deltaY > 1)
                {
                    for (int i = 1; i < deltaY; i++)
                    {
                        if (!gridManager.pieces[(int)start.x, (int)start.y + i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }

                if (deltaY < 0)
                {
                    for (int i = 1; i < Mathf.Abs(deltaY); i++)
                    {
                        if (!gridManager.pieces[(int)start.x, (int)start.y - i].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }

                if (deltaX > 1)
                {
                    for (int i = 1; i < deltaX; i++)
                    {
                        if (!gridManager.pieces[(int)start.x + i, (int)start.y].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }

                if (deltaX < 0)
                {
                    for (int i = 1; i < Mathf.Abs(deltaX); i++)
                    {
                        if (!gridManager.pieces[(int)start.x - i, (int)start.y].name.Contains("Empty"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }
        //King check
        else if (gridManager.pieces[(int)start.x, (int)start.y].name.Contains("King"))
        {
            if (deltaY == 2)
            {
                for (int i = 1; i < deltaY; i++)
                {
                    if (!gridManager.pieces[(int)start.x, (int)start.y + i].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }

            if (deltaY == -2)
            {
                for (int i = 1; i < Mathf.Abs(deltaY); i++)
                {
                    if (!gridManager.pieces[(int)start.x, (int)start.y - i].name.Contains("Empty"))
                    {
                        return true;
                    }
                }
            }
        }
        //Pawn check
        else if (gridManager.pieces[(int)start.x, (int)start.y].name.Contains("Pawn"))
        {
            if (Mathf.Abs(deltaY) == 2) 
            {
                if (!gridManager.pieces[(int)start.x, (int)start.y + (int)(deltaY/2)].name.Contains("Empty"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //Checks if otherPiece is a different color
    public bool opposingColor(Piece otherPiece)
    {
        if (gameObject.tag != otherPiece.tag && otherPiece.tag != "Empty")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Checks if otherPiece is the same color
    public bool sameColor(Piece otherPiece)
    {
        if (gameObject.tag == otherPiece.tag && otherPiece.tag != "Empty")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Generates all possible moves for current piece
    //Stores into List<Vector3> possibleMoves
    public void generatePossibleMoves()
    {
        possibleMoves.Clear();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if(verifyMove(new Vector3(transform.position.x, transform.position.y), new Vector3(x, y)))
                {
                    possibleMoves.Add(new Vector3(x, y));
                }
            }
        }
    }
    #endregion
}
