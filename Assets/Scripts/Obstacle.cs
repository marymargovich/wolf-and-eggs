using UnityEngine;

/// <summary>
/// Handles an obstacle interaction with the player.
/// Applies obstacle penalty logic and destroys itself when hit.
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject splashPrefab;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Obstacle: OnTriggerEnter2D triggered by {collision.gameObject.name}");
        
        // Accept either Player tag or DragonController component as a valid player hit.
        bool isPlayerTag = collision.CompareTag("Player");
        bool hasDragonController = collision.GetComponent<DragonController>() != null;

        Debug.Log($"Obstacle: isPlayerTag={isPlayerTag}, hasDragonController={hasDragonController}");

        if (!isPlayerTag && !hasDragonController)
        {
            Debug.Log($"Obstacle: {collision.gameObject.name} is not a valid player target. Ignoring collision.");
            return;
        }

        Debug.Log($"Obstacle: Valid collision detected! gameManager is {(gameManager == null ? "NULL" : "OK")}");
        
        // Apply damage to reduce lives and reset combo streak on obstacle hit.
        if (gameManager != null)
        {
            Debug.Log($"Obstacle: Calling TakeDamage(1). Current lives before: {gameManager.CurrentLives}");
            gameManager.TakeDamage(1);
        }
        else
        {
            Debug.LogError("Obstacle: GameManager reference is NULL! Cannot apply damage.");
        }

        if (scoreManager != null)
        {
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

        Debug.Log($"Obstacle: '{name}' hit and destroyed.");
        Destroy(gameObject);
    }
}
