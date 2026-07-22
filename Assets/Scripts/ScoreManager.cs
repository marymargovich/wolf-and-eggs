using System;
using UnityEngine;

/// <summary>
/// Tracks and updates the player's score.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Current player score. Read-only from outside this class.
    /// </summary>
    public int CurrentScore { get; private set; }

    /// <summary>
    /// Optional event fired whenever score changes.
    /// int argument = new score value.
    /// </summary>
    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        CurrentScore = 0;
        Debug.Log("ScoreManager: Initialized with score = 0.");
    }

    /// <summary>
    /// Adds points to the current score.
    /// </summary>
    public void AddScore(int points)
    {
        if (points < 0)
        {
            Debug.LogWarning($"ScoreManager: AddScore received negative value ({points}). It will be treated as 0.");
            points = 0;
        }

        CurrentScore += points;
        Debug.Log($"ScoreManager: Added {points} points. New score = {CurrentScore}.");
        OnScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>
    /// Subtracts points from the current score.
    /// Score is clamped at 0.
    /// </summary>
    public void SubtractScore(int points)
    {
        if (points < 0)
        {
            Debug.LogWarning($"ScoreManager: SubtractScore received negative value ({points}). It will be treated as 0.");
            points = 0;
        }

        CurrentScore -= points;

        if (CurrentScore < 0)
        {
            CurrentScore = 0;
        }

        Debug.Log($"ScoreManager: Subtracted {points} points. New score = {CurrentScore}.");
        OnScoreChanged?.Invoke(CurrentScore);
    }
}
