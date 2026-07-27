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

    [Tooltip("Maximum number of lives at the start of each game.")]
    public int maxLives = 3;

    [SerializeField] private UIManager uiManager;

    /// <summary>
    /// Public read-only access to the current game state.
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// Current remaining lives. Read-only from outside this class.
    /// </summary>
    public int CurrentLives { get; private set; }

    /// <summary>
    /// Optional event fired whenever lives change.
    /// int argument = current remaining lives.
    /// </summary>
    public System.Action<int> OnLivesChanged;

    /// <summary>
    /// True only while the game is actively running.
    /// Other scripts can check this before moving/spawning/updating.
    /// </summary>
    public bool IsGameActive => currentState == GameState.Playing;

    private void Awake()
    {
        // Ensure the game starts in the Start state when the scene loads.
        currentState = GameState.Start;
        CurrentLives = maxLives;

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

        CurrentLives = Mathf.Max(0, maxLives);
        OnLivesChanged?.Invoke(CurrentLives);
        currentState = GameState.Playing;
        Debug.Log("Game started.");
    }

    /// <summary>
    /// Switches the game into Game Over mode.
    /// Call this when time runs out, player loses, or end condition is reached.
    /// </summary>
    public void EndGame()
    {
        if (currentState == GameState.GameOver)
        {
            return;
        }

        currentState = GameState.GameOver;
        Debug.Log("Game over.");
    }

    /// <summary>
    /// Handles timer expiration and routes to a win result when the player still has lives.
    /// </summary>
    public void HandleTimerExpired()
    {
        if (!IsGameActive)
        {
            return;
        }

        if (CurrentLives > 0 && uiManager != null)
        {
            uiManager.ShowWinScreen();
            return;
        }

        EndGame();
    }

    /// <summary>
    /// Applies damage to the player.
    /// Ends the game when lives reach zero.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (!IsGameActive)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        Debug.Log($"GameManager: Player took {amount} damage.");
        CurrentLives -= amount;
        if (CurrentLives < 0)
        {
            CurrentLives = 0;
        }

        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            EndGame();
        }
    }
}
