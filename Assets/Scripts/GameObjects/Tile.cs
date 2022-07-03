using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColour, _offsetColour, _validMoveColour;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Piece _occupiedBy;

    public void InitColour(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColour : _baseColour;
    }

    void OnMouseEnter()
    { 
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    //sets occupied by to the given piece
    public void SetOccupiedBy(Piece p)
    {
        _occupiedBy = p;
    }

    //gets the occupying piece
    public Piece GetOccupiedBy()
    {
        return _occupiedBy;
    }

    //sets the tile to show as valid
    public void ShowValid()
    {
        _renderer.color = _validMoveColour;
    }
    
    //sets the tile to not show as valid
    public void StopShowValid()
    {
        _renderer.color = _baseColour;
    }

    //returns true if the tile is being highlighted
    public bool IsHighlightOn()
    {
        return _highlight.activeSelf;
    }
}
