using UnityEngine;

/// <summary>
/// Moves a spawned item downward while the game is active,
/// then destroys it when it leaves the play area.
/// </summary>
public class FallingObject : MonoBehaviour
{
    [Header("Falling")]
    [Tooltip("Vertical fall speed in units per second.")]
    public float fallSpeed = 5f;

    [Tooltip("If the object goes below this Y position, it will be destroyed.")]
    public float destroyY = -6f;

    [Header("References (Optional)")]
    [Tooltip("If left empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager != null)
        {
            Debug.Log($"FallingObject: GameManager found for '{name}'.");
        }
        else
        {
            Debug.LogWarning($"FallingObject: GameManager not found for '{name}'. Object will not move until GameManager exists.");
        }
    }

    private void Update()
    {
        // Move only while the game is active.
        if (gameManager == null || !gameManager.IsGameActive)
        {
            return;
        }

        transform.Translate(0f, -fallSpeed * Time.deltaTime, 0f, Space.World);

        // Remove object when it falls below the cleanup line.
        if (transform.position.y < destroyY)
        {
            Debug.Log($"FallingObject: Destroying '{name}' below destroyY ({destroyY}).");
            Destroy(gameObject);
        }
    }
}
