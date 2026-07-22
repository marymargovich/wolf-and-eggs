using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns random falling item prefabs while the game is active.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    [Tooltip("Prefabs that can be spawned. Add treasure and obstacle prefabs here.")]
    public GameObject[] itemPrefabs;

    [Tooltip("Time between spawn attempts in seconds.")]
    public float spawnInterval = 1f;

    [Tooltip("Y position where items appear.")]
    public float spawnY = 6f;

    [Tooltip("Spawns at a random X between -spawnRangeX and +spawnRangeX.")]
    public float spawnRangeX = 7f;

    [Header("References (Optional)")]
    [Tooltip("If left empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private Coroutine spawnRoutine;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (gameManager != null)
        {
            Debug.Log("ObjectSpawner: GameManager found.");
        }
        else
        {
            Debug.LogWarning("ObjectSpawner: GameManager not found. Spawner will wait until one exists.");
        }
    }

    private void OnEnable()
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
            Debug.Log("ObjectSpawner: Spawn loop started.");
        }
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            Debug.Log("ObjectSpawner: Spawn loop stopped.");
        }
    }

    /// <summary>
    /// Repeatedly tries to spawn an item at a fixed interval.
    /// Spawning happens only while the game is active.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Mathf.Max(0.05f, spawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
                continue;
            }

            if (!gameManager.IsGameActive)
            {
                continue;
            }

            SpawnOne();
        }
    }

    /// <summary>
    /// Spawns one random prefab from the itemPrefabs list.
    /// </summary>
    private void SpawnOne()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("ObjectSpawner: No itemPrefabs assigned. Cannot spawn items.");
            return;
        }

        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject prefabToSpawn = itemPrefabs[randomIndex];

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"ObjectSpawner: itemPrefabs[{randomIndex}] is null.");
            return;
        }

        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Debug.Log($"ObjectSpawner: Spawned '{spawned.name}' at X={randomX:F2}, Y={spawnY:F2}.");
    }
}
