using UnityEngine;

/// <summary>
/// Handles an obstacle interaction with the player.
/// Applies obstacle penalty logic and destroys itself when hit.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;

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

        // Apply penalty points and reset combo streak on obstacle hit.
        if (scoreManager != null)
        {
            scoreManager.SubtractScore(penaltyValue);
            scoreManager.ResetCombo();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBadItemSFX();
        }

        // Spawn the splash effect at the obstacle position if a prefab is assigned.
        if (splashPrefab != null)
        {
            Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
