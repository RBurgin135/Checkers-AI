using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    public static GameManager Instance;
    public GameState GameState;
    public bool AIPlaying;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.startScreen);
    }

    /**
     * starts the game
     */
    public void StartGame()
    {
        _canvas.gameObject.SetActive(false);
        UpdateGameState(GameState.generateGrid);
        UpdateGameState(GameState.generatePieces);
        UpdateGameState(AIPlaying ? GameState.aiTurn : GameState.playerTurn);
    }

    /**
     * ai button on the start screen is pressed
     */
    public void AIButton()
    {
        AIPlaying = true;
        StartGame();
    }

    /**
     * 2 player button on the start screen is pressed
     */
    public void TwoPlayerButton()
    {
        AIPlaying = false;
        StartGame();
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
            case GameState.startGame:

                break;
            case GameState.playerTurn:
                PlayerTurnManager.Instance.CheckForForcedMove();
                break;
            case GameState.aiTurn:
                AI.Instance.TakeTurn();
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
    startGame,
    playerTurn,
    aiTurn
}