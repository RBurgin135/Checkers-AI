using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{
    public static PlayerTurnManager Instance;
    public Team PlayerTurn;
    private List<Tile> _validMoves;

    void Awake()
    {
        Instance = this;
        PlayerTurn = Team.playerOne;
    }

    /**
     * called once a player has made a move
     */
    public void NextTurn()
    {
        //next turn
        PlayerTurn = PlayerTurn == Team.playerOne ? Team.playerTwo : Team.playerOne;

        //update game state
        GameManager.Instance.UpdateGameState(GameState.checkForWin);
    }

    /**
     * shows valid moves by activating tiles
     */
    public void FindValidMoves(Vector3 position, Team team)
    {
        //calc valid transforms
        Vector2[] moves = { 
            new Vector2(position[0] - 1, position[1] + (team == Team.playerOne ? 1 : -1)),
            new Vector2(position[0] + 1, position[1] + (team == Team.playerOne ? 1 : -1))
        };

        //find valid tiles to move to
        _validMoves = new List<Tile>();
        foreach (Vector2 m in moves)
        {
            //calculate valid tiles
            if (GridManager.Instance.GetTiles().ContainsKey(m)  //tile is in board
                && !FriendlyPresent(m, team))  //cannot move onto 
            {
                if (EnemyPresent(m, team)) //cannot move onto enemies
                {
                      //can take them if the next tile is free
                }
                    _validMoves.Add(GridManager.Instance.GetTiles()[m]);
            }
        }

        //set all found tiles to show valid
        foreach (Tile t in _validMoves)
        {
            t.ShowValid();
        }
    }

    /**
     * stop showing valid tiles
     */
    public void StopShowingValidMoves()
    {
        foreach (Tile t in _validMoves)
        {
            t.StopShowValid();
        }
    }

    /**
     * move a piece to where the mouse is if the tile is valid
     */
    public void Move(Piece p)
    {
        //find the tile the mouse is over when it stops holding
        Tile chosenTile = null;
        foreach (Tile t in GridManager.Instance.GetTiles().Values)
        {
            //finds the chosen tile as the one being highlighted
            if (t.IsHighlightOn())
            {
                chosenTile = t;
                break;
            }
        }

        //if the chosen tile is valid the move is made
        if(chosenTile != null && _validMoves.Contains(chosenTile))
        {
            p.GetOccupying().SetOccupiedBy(null);
            p.SetOccupying(chosenTile);
            chosenTile.SetOccupiedBy(p);
            p.transform.position = chosenTile.transform.position;

            //next turn
            NextTurn();
            GameManager.Instance.UpdateGameState(GameState.checkForWin);
        }

    }

    /**
     * returns true if a friendly piece is in a given tile
     */
    private bool FriendlyPresent(Vector2 position, Team team)
    {
        Piece p = GridManager.Instance.GetTiles()[position].GetOccupiedBy();
        if (p == null) return false;
        return p.GetTeam() == team;
    }

    /**
     * returns true if an opposition piece is in a given tile
     */
    private bool EnemyPresent(Vector2 position, Team team)
    {
        Piece p = GridManager.Instance.GetTiles()[position].GetOccupiedBy();
        if (p == null) return false;
        return p.GetTeam() != team;
    }
}