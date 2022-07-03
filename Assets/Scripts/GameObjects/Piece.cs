using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Color _playerOneColour;
    [SerializeField] private Color _playerTwoColour;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _kingGUI;
    [SerializeField] private Tile _occupying;
    private Team _team;
    private bool _clicked;
    private Vector3 _lastPosition;
    private bool _isKing;

    public void Init(bool isSecondPlayer)
    {
        _kingGUI.SetActive(false);
        _renderer.color = isSecondPlayer ? _playerTwoColour : _playerOneColour;
        _team = isSecondPlayer ? Team.playerTwo : Team.playerOne;
        string teamString = isSecondPlayer ? "P1" : "P2";
        name = $"Piece {teamString} {transform.position[0]} {transform.position[1]}";
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.playerTurn) return;
        if (_clicked)
        { 
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
    }

    /**
     * when clicked on
     */
    private void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.playerTurn) return;
        if (PlayerTurnManager.Instance.PlayerTurn == _team)
        {
            //handle GUI
            _lastPosition = transform.position;
            _clicked = true;
            GetComponent<BoxCollider2D>().enabled = false;

            //find valid moves to move to
            if (PlayerTurnManager.Instance.ForcedMove)
                PlayerTurnManager.Instance.FindTakeMoves(this);
            else
                PlayerTurnManager.Instance.FindValidMoves(this);
        }
    }

    /**
     * when the mouse button is lifted
     */
    private void OnMouseUp()
    {
        if (GameManager.Instance.GameState != GameState.playerTurn) return;
        if (_clicked)
        {
            _clicked = false;
            transform.position = _lastPosition;
            GetComponent<BoxCollider2D>().enabled = true;
            PlayerTurnManager.Instance.StopShowingValidMoves();

            //move piece
            if (PlayerTurnManager.Instance.ForcedMove)
                PlayerTurnManager.Instance.TakeMove(this);
            else
                PlayerTurnManager.Instance.Move(this);
        }
    }

    /**
     * method to make a piece into a king
     */
    public void CrownPiece()
    {
        _isKing = true;
        _kingGUI.SetActive(true);
    }

    /**
     * sets occupying to the tile the piece is currently occupying
     */
    public void SetOccupying(Tile t)
    {
        _occupying = t;
    }

    /**
     * gets the tile the piece is currently occupying
     */
    public Tile GetOccupying()
    {
        return _occupying;
    }

    /**
     * getter for team
     */
    public Team GetTeam()
    {
        return _team;
    }

    /**
     * getter for isking
     */
    public bool IsKing()
    {
        return _isKing;
    }
}


public enum Team
{
    playerOne,
    playerTwo
}