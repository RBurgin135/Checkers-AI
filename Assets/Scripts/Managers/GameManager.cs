using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.generateGrid);
    }

    /**
     * updates the game state
     */
    public void UpdateGameState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.startScreen:
                break;
            case GameState.generateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.generatePieces:
                PieceManager.Instance.GeneratePieces();
                break;
            case GameState.playerTurn:
                break;
            case GameState.checkForWin:
                UpdateGameState(GameState.playerTurn);
                break;
            case GameState.victoryScreen:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState
{
    startScreen,
    generateGrid,
    generatePieces,
    playerTurn,
    checkForWin,
    victoryScreen
}