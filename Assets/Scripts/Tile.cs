using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables
    [SerializeField] private Color baseColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] public GameObject highlight;
    #endregion

    //Grid coloring
    public void colored(bool isOffset)
    {
        if (!isOffset)
        {
            _renderer.color = baseColor;
        }
    }
}
