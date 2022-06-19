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
    }

    /**
     * generates the checker pieces
     */
    public void GeneratePieces()
    {
        for (int i = 0; i < 2; i++)
            for (int x = 0; x < 8; x++)
            {
                //create new piece
                int y = 0;
                if (i == 1) y += 6; //shift up for other player
                if (x % 2 == 0) y += 1; //shift up for second rank
                Piece newPiece = Instantiate(_piecePrefab, new Vector3(x, y), Quaternion.identity);
                string team = i == 0 ? "P1" : "P2";
                newPiece.name = $"Piece {team} {x} {y}";
                newPiece.Init(i == 1);

                //put piece on occupied tile
                Tile t = GridManager.Instance.GetTiles()[new Vector2(x, y)];
                t.SetOccupiedBy(newPiece);
                newPiece.SetOccupying(t);
            }

        //update game state
        GameManager.Instance.UpdateGameState(GameState.playerTurn);
    }
}
