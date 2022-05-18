using UnityEngine;

public class Minimax : MonoBehaviour
{
    public GridManager gridManager;
    public GameManager gameManager;
    public Move moveScript;


    public void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        moveScript = GameObject.Find("GameManager").GetComponent<Move>();
    }
    private int minimax(Piece[,] state, int depth, bool maximizingPlayer)
    {
        if(depth == 0)
        {
            return gameManager.StatusEval();
        }

        return 0;
    }
    
}
