using UnityEngine;

/// <summary>
/// Handles a collectible item interaction with the player.
/// Adds score and destroys itself when collected.
/// </summary>
public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;

    [Tooltip("How many points this collectible adds.")]
    public int scoreValue = 10;

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
            Debug.Log("Collectible: ScoreManager found.");
        }
        else
        {
            Debug.LogWarning("Collectible: ScoreManager not found. Collectible will still be destroyed on pickup.");
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

        // If ScoreManager exists, add points directly.
        if (scoreManager != null)
        {
            scoreManager.AddCollectibleHit(scoreValue);
            Debug.Log($"Collectible: Added {scoreValue} points.");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGoodItemSFX();
        }

        // Spawn the splash effect at the collectible position if a prefab is assigned.
        if (splashPrefab != null)
        {
            Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log($"Collectible: '{name}' collected and destroyed.");
        Destroy(gameObject);
    }
}
