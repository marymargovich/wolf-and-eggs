using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnItem
{
    public GameObject prefab;
    public int weight = 100;
}

/// <summary>
/// Spawns random falling item prefabs while the game is active.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    [Tooltip("Weighted list of items that can be spawned. Add treasure and obstacle prefabs here.")]
    public List<SpawnItem> spawnItems = new List<SpawnItem>();

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

            SpawnObject();
        }
    }

    /// <summary>
    /// Spawns one random prefab based on configured item weights.
    /// </summary>
    private void SpawnObject()
    {
        GameObject prefabToSpawn = GetRandomSpawnItem();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("ObjectSpawner: No valid weighted spawn item found. Cannot spawn items.");
            return;
        }

        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Debug.Log($"ObjectSpawner: Spawned '{spawned.name}' at X={randomX:F2}, Y={spawnY:F2}.");
    }

    /// <summary>
    /// Selects a prefab using weighted random choice from spawnItems.
    /// </summary>
    private GameObject GetRandomSpawnItem()
    {
        if (spawnItems == null || spawnItems.Count == 0)
        {
            return null;
        }

        int totalWeight = 0;
        foreach (SpawnItem item in spawnItems)
        {
            if (item == null)
            {
                continue;
            }

            totalWeight += item.weight;
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        // Random.Range with int uses an inclusive minimum and exclusive maximum.
        int randomWeight = Random.Range(0, totalWeight);

        foreach (SpawnItem item in spawnItems)
        {
            if (item == null)
            {
                continue;
            }

            randomWeight -= item.weight;
            if (randomWeight < 0)
            {
                return item.prefab;
            }
        }

        return null;
    }
}
