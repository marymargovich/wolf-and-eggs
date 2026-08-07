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

    [Tooltip("Starting fall speed for spawned objects.")]
    public float initialFallSpeed = 2f;

    [Tooltip("Maximum fall speed reached at full difficulty.")]
    public float maxFallSpeed = 6f;

    [Tooltip("Starting time between spawn attempts in seconds.")]
    public float initialSpawnInterval = 1.5f;

    [Tooltip("Minimum time between spawn attempts at full difficulty.")]
    public float minSpawnInterval = 0.6f;

    [Tooltip("Seconds of active gameplay required to reach full difficulty.")]
    public float difficultyRampDuration = 60f;

    [Tooltip("Y position where items appear.")]
    public float spawnY = 6f;

    [Tooltip("Spawns at a random X between -spawnRangeX and +spawnRangeX.")]
    public float spawnRangeX = 7f;

    [Tooltip("Horizontal padding from camera edges to keep spawns inside the visible view.")]
    public float spawnEdgePadding = 0.15f;

    [Header("References (Optional)")]
    [Tooltip("If left empty, this script will try to find GameManager automatically.")]
    public GameManager gameManager;

    private Coroutine spawnRoutine;
    private float elapsedActiveGameTime;
    private float currentFallSpeed;
    private float currentSpawnInterval;
    private float minSpawnX;
    private float maxSpawnX;
    private Camera cachedMainCamera;

    private void Awake()
    {
        elapsedActiveGameTime = 0f;
        currentFallSpeed = initialFallSpeed;
        currentSpawnInterval = initialSpawnInterval;
        cachedMainCamera = Camera.main;
        UpdateSpawnBoundsFromCamera();

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

    }

    private void Update()
    {
        if (gameManager == null)
        {
            return;
        }

        // Increase difficulty only while gameplay is active.
        if (gameManager.IsGameActive)
        {
            elapsedActiveGameTime += Time.deltaTime;
        }

        // Clamp progress from 0 to 1 based on active gameplay time.
        float progressRatio = difficultyRampDuration > 0f
            ? Mathf.Clamp01(elapsedActiveGameTime / difficultyRampDuration)
            : 1f;

        currentFallSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, progressRatio);
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, progressRatio);
        UpdateSpawnBoundsFromCamera();
    }

    private void OnEnable()
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    /// <summary>
    /// Repeatedly tries to spawn an item at a difficulty-adjusted interval.
    /// Spawning happens only while the game is active.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Mathf.Max(0.05f, currentSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (gameManager == null)
            {
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
            return;
        }

        float randomX = Random.Range(minSpawnX, maxSpawnX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        FallingObject fallingObject = spawned.GetComponent<FallingObject>();
        if (fallingObject != null)
        {
            // Apply current difficulty speed to this spawned object.
            fallingObject.fallSpeed = currentFallSpeed;
        }
    }

    /// <summary>
    /// Updates horizontal spawn bounds from camera view so objects stay on-screen.
    /// </summary>
    private void UpdateSpawnBoundsFromCamera()
    {
        Camera mainCamera = cachedMainCamera;
        if (mainCamera == null)
        {
            cachedMainCamera = Camera.main;
            mainCamera = cachedMainCamera;
        }

        if (mainCamera == null)
        {
            minSpawnX = -spawnRangeX;
            maxSpawnX = spawnRangeX;
            return;
        }

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, distanceToCamera));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, distanceToCamera));

        minSpawnX = leftEdge.x + spawnEdgePadding;
        maxSpawnX = rightEdge.x - spawnEdgePadding;

        if (maxSpawnX <= minSpawnX)
        {
            float centerX = (leftEdge.x + rightEdge.x) * 0.5f;
            minSpawnX = centerX;
            maxSpawnX = centerX;
        }

        spawnRangeX = Mathf.Max(0f, (maxSpawnX - minSpawnX) * 0.5f);
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
