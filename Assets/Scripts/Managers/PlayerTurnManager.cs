using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnManager : MonoBehaviour
{
    public static PlayerTurnManager Instance;
    public Team PlayerTurn;
    public bool ForcedMove;
    private List<Tile> _validMoves;
    private Dictionary<Tile, Piece> _takeMoves;

    void Awake()
    {
        Instance = this;
        PlayerTurn = Team.playerOne;
        ForcedMove = false;
    }

    /**
     * called once a player has made a move
     */
    public void NextTurn()
    {
        //cancel forced move
        ForcedMove = false;

        //next turn
        PlayerTurn = PlayerTurn == Team.playerOne ? Team.playerTwo : Team.playerOne;

        //update game state
        if (GameManager.Instance.AIPlaying && PlayerTurn == Team.playerOne)
            GameManager.Instance.UpdateGameState(GameState.aiTurn);
        else
            GameManager.Instance.UpdateGameState(GameState.playerTurn);
    }

    /**.
     * method to check if the player is forced to take an opponents piece
     */
    public void CheckForForcedMove()
    {
        //iterate through pieces
        foreach (Piece p in PieceManager.Instance.GetPieces())
        {
            //if they are players pieces
            if (p.GetTeam() == PlayerTurn)
            {
                //find if they can take an opponents piece
                if (FindTakeMoves(p))
                {
                    
                    //if so the player must take on this turn
                    ForcedMove = true;
                    StopShowingValidMoves();
                    break;
                }
            }
        }
    }

    /**
     * finds and returns all of the tiles a given piece can move to in 
     * order to take a opponents piece
     * returns true if 1 or more valid take moves are found
     */ 
    public bool FindTakeMoves(Piece p)
    {
        //get resources
        Vector3 position = p.transform.position;
        Team team = p.GetTeam();
        bool isking = p.IsKing();
        Dictionary<Vector2, Tile> tiles = GridManager.Instance.GetTiles();

        //valid move transforms
        List<Vector2> moves = GetValidTransforms(isking, team);


        //look at adjacent diagonal tiles
        _takeMoves = new Dictionary<Tile, Piece>();
        foreach (Vector2 m in moves)
        {
            Vector2 takeAt = new Vector2(position[0] + m[0], position[1] + m[1]);

            //if tile is on board and enemy is present
            if (tiles.ContainsKey(takeAt)
                && EnemyPresent(takeAt, team))
            {
                //check for space beyond it, if empty add as a valid take move
                Vector2 moveTo = new Vector2(position[0] + m[0] * 2, position[1] + m[1] * 2);
                if (tiles.ContainsKey(moveTo)
                    && NobodyPresent(moveTo))
                {
                    _takeMoves.Add(tiles[moveTo], tiles[takeAt].GetOccupiedBy()); 
                }
            }
        }

        //set all found tiles to show valid
        foreach (Tile t in _takeMoves.Keys)
        {
            t.ShowValid();
        }

        //return bool
        return _takeMoves.Count > 0;
    }

    /**
     * shows valid moves by activating tiles
     */
    public void FindValidMoves(Piece p)
    {
        //get resources
        Vector3 position = p.transform.position;
        Team team = p.GetTeam();
        bool isking = p.IsKing();
        Dictionary<Vector2, Tile> tiles = GridManager.Instance.GetTiles();

        //get valid move transforms
        List<Vector2> moves = GetValidTransforms(isking, team);

        //find valid tiles to move to
        _validMoves = new List<Tile>();
        foreach (Vector2 m in moves)
        {
            Vector2 movePos = new Vector2(position[0] + m[0], position[1] +m[1]);

            //if tile is in board and is empty
            if (tiles.ContainsKey(movePos) 
                && NobodyPresent(movePos))  
            {
                _validMoves.Add(tiles[movePos]);
            }
        }

        //set all found tiles to show valid
        foreach (Tile t in _validMoves)
        {
            t.ShowValid();
        }
    }

    /**
     * move a piece to where the mouse is if the tile is valid
     */
    public void Move(Piece p)
    {
        //find chosen tile
        Tile chosenTile = FindChosenTile();

        //if the chosen tile is valid the move is made
        if(chosenTile != null && _validMoves.Contains(chosenTile))
        {
            //move piece
            MovePieceToTile(p, chosenTile);

            //check if king
            KingCheck(p);

            //next turn
            NextTurn();
        }
    }

    /**
     * move a piece to take another piece if the move is valid
     */
    public void TakeMove(Piece p)
    {
        //find chosen tile
        Tile chosenTile = FindChosenTile();

        //if the chosen tile is valid the move is made
        if (chosenTile != null && _takeMoves.ContainsKey(chosenTile))
        {
            //move piece and take opponents
            RemovePieceFromTile(_takeMoves[chosenTile]);
            MovePieceToTile(p, chosenTile);

            //can take again?
            if (!KingCheck(p) && FindTakeMoves(p))
            {
                return;
            }

            //next turn
            NextTurn();
        }
    }

    /**
     * method responsible for checking if a piece can be made a king and then does so
     */
    public bool KingCheck(Piece p)
    {
        float goal = p.GetTeam() == Team.playerOne ? GridManager.Instance.GetHeight()-1 : 0;
        if (p.transform.position[1] == goal)
        {
            p.CrownPiece();
            return true;
        }
        return false;
    }

    /**
     * removes a piece from the tile its occupying
     */
    private void RemovePieceFromTile(Piece p)
    { 
        p.GetOccupying().SetOccupiedBy(null);
        p.SetOccupying(null);
        PieceManager.Instance.GetPieces().Remove(p);
        p.transform.position = new Vector3(0, 0, -10);
        Destroy(p);
    }

    /**
     * moves a piece to a given tile
     */
    private void MovePieceToTile(Piece p, Tile t)
    {
        p.GetOccupying().SetOccupiedBy(null);
        p.SetOccupying(t);
        t.SetOccupiedBy(p);
        p.transform.position = t.transform.position;
    }

    /**
     * finds the tile the mouse is hovering over
     */
    public Tile FindChosenTile()
    {
        //find the tile the mouse is over when it stops holding
        foreach (Tile t in GridManager.Instance.GetTiles().Values)
        {
            //finds the chosen tile as the one being highlighted
            if (t.IsHighlightOn())
            {
                return t;
            }
        }
        return null;
    }

    /**
     * stop showing valid tiles
     */
    public void StopShowingValidMoves()
    {
        if(ForcedMove)  foreach (Tile t in  _takeMoves.Keys)  t.StopShowValid();
        else            foreach (Tile t in _validMoves)       t.StopShowValid();
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

    /**
     * returns true if no pieces are in a given tile
     */
    private bool NobodyPresent(Vector2 position)
    {
        Piece p = GridManager.Instance.GetTiles()[position].GetOccupiedBy();
        if (p == null)  return true;
        else            return false;
    }

    /**
     * holds the valid move tranforms
     */
    private List<Vector2> GetValidTransforms(bool isking, Team team)
    {
        List<Vector2> moves = new List<Vector2>();
;       moves.Add(new Vector2(-1, team == Team.playerOne ? 1 : -1));
        moves.Add(new Vector2(1, team == Team.playerOne ? 1 : -1));
        if (isking)
        {
            moves.Add(new Vector2(-1, team == Team.playerOne ? -1 : 1));
            moves.Add(new Vector2(1, team == Team.playerOne ? -1 : 1));
        }
        return moves;
    }
}