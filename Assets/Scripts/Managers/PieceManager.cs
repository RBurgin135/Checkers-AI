using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    [SerializeField] private Piece _piecePrefab;
    public static PieceManager Instance;
    private List<Piece> _pieces;

    private void Awake()
    {
        Instance = this;
        _pieces = new List<Piece>();
    }

    /**
     * generates the checker pieces
     */
    public void GeneratePieces()
    {
        //cycle through the teams
        for (int i = 0; i < 2; i++) 
        {   //cycle through x
            for (int x = 0; x < 8; x+=2)
            {   //cycle through y
                for (int y = 0; y < 3; y++)
                {
                    //shift location
                    int xpos = x, ypos = y;
                    if (i == 1)     ypos += 5;
                    if (ypos % 2 != 0) xpos += 1;

                    //create new piece
                    Piece newPiece = Instantiate(_piecePrefab, new Vector3(xpos, ypos), Quaternion.identity);
                    newPiece.Init(i == 1);
                    _pieces.Add(newPiece);

                    //put piece on occupied tile
                    Tile t = GridManager.Instance.GetTiles()[new Vector2(xpos, ypos)];
                    t.SetOccupiedBy(newPiece);
                    newPiece.SetOccupying(t);
                }                
            }
        }
    }

    //getter for pieces
    public List<Piece> GetPieces()
    {
        return _pieces;
    }
}
