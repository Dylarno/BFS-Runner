using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public EndScreenManager endScreen;
    public DeathScreenManager deathScreen;

    void Start()
    {
        // setup event listeners
        GameController.GameEnded += OnGameEnded;
    }

    private void OnDestroy()
    {
        // clear event listeners
        GameController.GameEnded -= OnGameEnded;
    }

    private void OnGameEnded(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.PlayerDied:
                deathScreen.gameObject.SetActive(true);
                break;

            case GameOverReason.LevelComplete:
                endScreen.gameObject.SetActive(true);
                break;
        }
    }
}
