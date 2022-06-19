using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Color _playerOneColour;
    [SerializeField] private Color _playerTwoColour;
    [SerializeField] private SpriteRenderer _renderer;
    private Tile _occupying;
    private Team _team;
    private bool _clicked;
    private Vector3 _lastPosition;

    public void Init(bool isSecondPlayer)
    {
        _renderer.color = isSecondPlayer ? _playerTwoColour : _playerOneColour;
        _team = isSecondPlayer ? Team.playerTwo : Team.playerOne;
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
            _lastPosition = transform.position;
            _clicked = true;
            GetComponent<BoxCollider2D>().enabled = false;
            PlayerTurnManager.Instance.FindValidMoves(transform.position, _team);
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
            PlayerTurnManager.Instance.Move(this);
        }
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
}


public enum Team
{
    playerOne,
    playerTwo
}