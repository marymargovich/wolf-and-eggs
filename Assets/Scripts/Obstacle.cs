using UnityEngine;

/// <summary>
/// Handles an obstacle interaction with the player.
/// Applies player damage and destroys itself when hit.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;

    [Tooltip("How many points this obstacle subtracts.")]
    public int penaltyValue = 5;

    [Tooltip("Optional direct reference. If empty, this script will try to find ScoreManager automatically.")]
    public ScoreManager scoreManager;

    [Tooltip("Optional direct reference. If empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private void Awake()
    {
        // Auto-find ScoreManager if it was not assigned in the Inspector.
        if (scoreManager == null)
        {
            scoreManager = FindAnyObjectByType<ScoreManager>();
        }

        // Auto-find GameManager if it was not assigned in the Inspector.
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (scoreManager != null)
        {
            Debug.Log("Obstacle: ScoreManager found.");
        }
        else
        {
            Debug.LogWarning("Obstacle: ScoreManager not found. Obstacle will still be destroyed on hit.");
        }

        if (gameManager != null)
        {
            Debug.Log("Obstacle: GameManager found.");
        }
        else
        {
            Debug.LogWarning("Obstacle: GameManager not found. Damage will not be applied on hit.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Accept either Player tag or DragonController component as a valid player hit.
        bool isPlayerTag = collision.CompareTag("Player");
        bool hasDragonController = collision.GetComponent<DragonController>() != null;

        if (!isPlayerTag && !hasDragonController)
        {
            return;
        }

        // Apply one point of damage through GameManager when available.
        if (gameManager != null)
        {
            gameManager.TakeDamage(1);
        }

        // Reset combo streak when the player hits an obstacle.
        if (scoreManager != null)
        {
            scoreManager.ResetCombo();
        }

        // Spawn the splash effect at the obstacle position if a prefab is assigned.
        if (splashPrefab != null)
        {
            Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"Obstacle: '{name}' hit and destroyed.");
        Destroy(gameObject);
    }
}
