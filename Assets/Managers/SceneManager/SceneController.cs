using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    public bool awaitingSceneChange;
    private bool awaitingRetry;

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

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // setup event listeners
        PlayerController.PlayerEscaped += OnPlayerEscaped;
        PlayerController.PlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        // clear event listeners
        PlayerController.PlayerEscaped -= OnPlayerEscaped;
        PlayerController.PlayerDied -= OnPlayerDied;
    }

    private void Update()
    {
        if (awaitingRetry && Input.GetKeyDown(KeyCode.Space))
        {
            awaitingSceneChange = false;
            awaitingRetry = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (awaitingSceneChange && Input.GetKeyDown(KeyCode.Space))
        {
            awaitingSceneChange = false;
            awaitingRetry = false;
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(
                activeSceneIndex < SceneManager.sceneCountInBuildSettings - 1 ? activeSceneIndex + 1 : 0);
            return;
        }
    }

    private void OnPlayerDied()
    {
        awaitingRetry = true;
        awaitingSceneChange = false;
    }

    private void OnPlayerEscaped()
    {
        awaitingRetry = false;
        awaitingSceneChange = true;
    }
}
