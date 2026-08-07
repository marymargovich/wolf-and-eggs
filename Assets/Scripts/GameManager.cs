using UnityEngine;

/// <summary>
/// Controls the basic game flow for Little Dragon Treasure Hunt.
/// Attach this script to a single GameObject in the scene (for example: "GameManager").
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Simple game flow states.
    /// </summary>
    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }

    // Current state of the game. Other scripts can read this in the Inspector and via code.
    [SerializeField] private GameState currentState = GameState.Start;

    [SerializeField] private UIManager uiManager;

    /// <summary>
    /// Public read-only access to the current game state.
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// Optional event fired whenever game state changes.
    /// </summary>
    public System.Action<GameState> OnGameStateChanged;

    /// <summary>
    /// True only while the game is actively running.
    /// Other scripts can check this before moving/spawning/updating.
    /// </summary>
    public bool IsGameActive => currentState == GameState.Playing;

    private void Awake()
    {
        // Ensure the game starts in the Start state when the scene loads.
        currentState = GameState.Start;

        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
        }
    }

    /// <summary>
    /// Switches the game into active gameplay mode.
    /// Call this from a Start button, countdown complete event, or another controller script.
    /// </summary>
    public void StartGame()
    {
        if (currentState == GameState.Playing)
        {
            return;
        }

        currentState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentState);

        if (global::AudioManager.Instance != null)
        {
            global::AudioManager.Instance.PlayBGM();
        }
    }

    /// <summary>
    /// Switches the game into Game Over mode.
    /// Call this when gameplay ends and results should be shown.
    /// </summary>
    public void EndGame()
    {
        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;
        OnGameStateChanged?.Invoke(currentState);
    }

    /// <summary>
    /// Handles timer expiration and routes to the results panel.
    /// </summary>
    public void HandleTimerExpired()
    {
        if (!IsGameActive)
        {
            return;
        }

        ShowResultsPanel();
    }

    /// <summary>
    /// Shows the results panel and stops active gameplay.
    /// Falls back to EndGame when UIManager is unavailable.
    /// </summary>
    private void ShowResultsPanel()
    {
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
        }

        if (uiManager != null)
        {
            uiManager.ShowWinScreen();
            return;
        }

        EndGame();
    }
}
