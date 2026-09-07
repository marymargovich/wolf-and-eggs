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

    [Tooltip("Maximum number of lives at the start of each game.")]
    public int maxLives = 5;

    private int currentLives;

    /// <summary>
    /// Public read-only access to the current game state.
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// Current remaining lives. Read-only from outside this class.
    /// </summary>
    public int CurrentLives => currentLives;

    /// <summary>
    /// Optional event fired whenever lives change.
    /// int argument = current remaining lives.
    /// </summary>
    public System.Action<int> OnLivesChanged;

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
        currentLives = maxLives;

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

        currentLives = Mathf.Max(1, maxLives);
        OnLivesChanged?.Invoke(currentLives);
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

    /// <summary>
    /// Applies damage to the player (loses one life).
    /// Ends the game when lives reach zero.
    /// </summary>
    public void TakeDamage(int amount)
    {
        Debug.Log($"GameManager.TakeDamage({amount}) called. IsGameActive={IsGameActive}");
        
        if (!IsGameActive)
        {
            Debug.LogWarning($"GameManager: TakeDamage called but game is not active (State={currentState}). Ignoring.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"GameManager: TakeDamage called with invalid amount {amount}. Ignoring.");
            return;
        }

        Debug.Log($"GameManager: Player took {amount} damage. Lives before: {currentLives}");
        currentLives -= amount;

        if (currentLives < 0)
        {
            currentLives = 0;
        }

        Debug.Log($"GameManager: Invoking OnLivesChanged with {currentLives} lives. Listeners: {(OnLivesChanged == null ? 0 : OnLivesChanged.GetInvocationList().Length)}");
        OnLivesChanged?.Invoke(currentLives);
        Debug.Log($"GameManager: Lives remaining = {currentLives}.");

        if (currentLives <= 0)
        {
            Debug.Log("GameManager: Lives reached 0. Calling EndGame().");
            EndGame();
        }
    }
}
