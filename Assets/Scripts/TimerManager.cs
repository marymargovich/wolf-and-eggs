using UnityEngine;

/// <summary>
/// Handles the 60-second game timer and ends the game when time runs out.
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Tooltip("Total game time in seconds.")]
    public float totalTime = 60f;

    [Tooltip("Optional direct reference. If empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    /// <summary>
    /// Current time remaining in seconds.
    /// </summary>
    public float TimeRemaining { get; private set; }

    /// <summary>
    /// Optional event fired whenever the formatted time string should be refreshed in UI.
    /// string argument = current remaining time in MM:SS format.
    /// </summary>
    public System.Action<string> OnTimeChanged;

    private bool hasTriggeredEndGame;

    private void Awake()
    {
        TimeRemaining = Mathf.Max(0f, totalTime);
        OnTimeChanged?.Invoke(GetFormattedTime());

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

    }

    private void Update()
    {
        if (hasTriggeredEndGame)
        {
            return;
        }

        if (gameManager == null)
        {
            return;
        }

        // Count down only during active gameplay.
        if (!gameManager.IsGameActive)
        {
            return;
        }

        if (TimeRemaining <= 0f)
        {
            TriggerEndGame();
            return;
        }

        TimeRemaining -= Time.deltaTime;
        OnTimeChanged?.Invoke(GetFormattedTime());

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            TriggerEndGame();
        }
    }

    /// <summary>
    /// Resets timer back to totalTime. Useful for restart flows.
    /// </summary>
    public void ResetTimer()
    {
        TimeRemaining = Mathf.Max(0f, totalTime);
        hasTriggeredEndGame = false;
        OnTimeChanged?.Invoke(GetFormattedTime());
    }

    /// <summary>
    /// Returns remaining time in MM:SS format for UI.
    /// </summary>
    public string GetFormattedTime()
    {
        int seconds = Mathf.CeilToInt(TimeRemaining);
        int minutesPart = seconds / 60;
        int secondsPart = seconds % 60;
        return $"{minutesPart:00}:{secondsPart:00}";
    }

    private void TriggerEndGame()
    {
        if (hasTriggeredEndGame)
        {
            return;
        }

        hasTriggeredEndGame = true;
        gameManager.HandleTimerExpired();
    }
}
