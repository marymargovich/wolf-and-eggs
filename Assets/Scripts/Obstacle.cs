using UnityEngine;

/// <summary>
/// Handles an obstacle interaction with the player.
/// Subtracts score and destroys itself when hit.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [Tooltip("How many points this obstacle subtracts.")]
    public int penaltyValue = 5;

    [Tooltip("Optional direct reference. If empty, this script will try to find ScoreManager automatically.")]
    public ScoreManager scoreManager;

    private void Awake()
    {
        // Auto-find ScoreManager if it was not assigned in the Inspector.
        if (scoreManager == null)
        {
            scoreManager = FindAnyObjectByType<ScoreManager>();
        }

        if (scoreManager != null)
        {
            Debug.Log("Obstacle: ScoreManager found.");
        }
        else
        {
            Debug.LogWarning("Obstacle: ScoreManager not found. Obstacle will still be destroyed on hit.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Accept either Player tag or DragonController component as a valid player hit.
        bool isPlayerTag = other.CompareTag("Player");
        bool hasDragonController = other.GetComponent<DragonController>() != null;

        if (!isPlayerTag && !hasDragonController)
        {
            return;
        }

        // If ScoreManager exists, subtract points directly.
        if (scoreManager != null)
        {
            scoreManager.SubtractScore(penaltyValue);
            Debug.Log($"Obstacle: Subtracted {penaltyValue} points.");
        }

        Debug.Log($"Obstacle: '{name}' hit and destroyed.");
        Destroy(gameObject);
    }
}
