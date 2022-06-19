using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
    public static GridManager Instance;
    private Dictionary<Vector2, Tile> _tiles;
    
    private void Awake()
    {
        Instance = this;
    }

    //generates the grid
    public void GenerateGrid()
    {
        //move camera
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        //generates the grid
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                Tile newTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"Tile {x} {y}";
                bool isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                newTile.InitColour(isOffset); 
                _tiles.Add(new Vector2(x, y), newTile);
            }

        //update game state
        GameManager.Instance.UpdateGameState(GameState.generatePieces);
    }

    //get tile at coordinate (x, y)
    public Dictionary<Vector2, Tile> GetTiles()
    {
        return _tiles;
    }
}
