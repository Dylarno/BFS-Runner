using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameOverReason
{
    PlayerDied,
    LevelComplete
}

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public delegate void GameControllerDelegate(GameOverReason reason);
    public static event GameControllerDelegate GameEnded;

    public bool isGameRunning = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void EndGame(GameOverReason reason)
    {
        // ensure the game hasn't already ended
        if (!isGameRunning)
            return;

        // end the game
        isGameRunning = false;
        if (GameEnded != null)
            GameEnded(reason);
    }

    private void OnPlayerEscaped()
    {
        EndGame(GameOverReason.LevelComplete);
    }

    private void OnPlayerDied()
    {
        EndGame(GameOverReason.PlayerDied);
    }

    private void Start()
    {
        // setup event listeners
        PlayerController.PlayerDied += OnPlayerDied;
        PlayerController.PlayerEscaped += OnPlayerEscaped;
    }

    private void OnDestroy()
    {
        // clear event listeners
        PlayerController.PlayerDied -= OnPlayerDied;
        PlayerController.PlayerEscaped -= OnPlayerEscaped;
    }
}
