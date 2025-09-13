using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// PerformanceOptimizer provides additional performance enhancements for the enhanced Akash 2D walkway demo.
/// Includes object pooling, level-of-detail management, and frame rate optimization.

public class PerformanceOptimizer : MonoBehaviour
{
    [Header("Performance Settings")]
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool enableFrameRateControl = true;
    [SerializeField] private bool enablePerformanceMonitoring = true;

    [Header("Object Pooling")]
    [SerializeField] private bool enableObjectPooling = true;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private GameObject[] poolablePrefabs;

    [Header("LOD Management")]
    [SerializeField] private bool enableLODSystem = true;
    [SerializeField] private float highDetailDistance = 10f;
    [SerializeField] private float mediumDetailDistance = 20f;
    [SerializeField] private float lowDetailDistance = 30f;

    [Header("Network Optimization")]
    [SerializeField] private bool optimizeNetworkUpdates = true;
    [SerializeField] private float positionUpdateThreshold = 0.1f;
    [SerializeField] private float maxUpdateRate = 20f;

    [Header("Memory Management")]
    [SerializeField] private bool enableAutoGarbageCollection = true;
    [SerializeField] private float gcInterval = 10f;
    [SerializeField] private int maxCachedObjects = 50;

    // Object pooling
    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, List<GameObject>> activeObjects = new Dictionary<string, List<GameObject>>();

    // Performance monitoring
    private float lastFrameTime;
    private float averageFrameTime;
    private int frameCount;
    private List<float> frameTimes = new List<float>();

    // LOD management
    private Camera mainCamera;
    private List<LODObject> lodObjects = new List<LODObject>();

    // Network optimization
    private Dictionary<Transform, Vector3> lastNetworkPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> lastUpdateTimes = new Dictionary<Transform, float>();

    // Memory management
    private float lastGCTime;
    private int objectsCreated;
    private int objectsDestroyed;

    private void Awake()
    {
        InitializePerformanceOptimizer();
    }

    private void Start()
    {
        if (enableFrameRateControl)
        {
            Application.targetFrameRate = targetFrameRate;
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        StartCoroutine(PerformanceMonitoringCoroutine());

        if (enableAutoGarbageCollection)
        {
            StartCoroutine(AutoGarbageCollectionCoroutine());
        }
    }

    private void Update()
    {
        if (enablePerformanceMonitoring)
        {
            UpdatePerformanceMetrics();
        }

        if (enableLODSystem)
        {
            UpdateLODSystem();
        }

        if (optimizeNetworkUpdates)
        {
            OptimizeNetworkUpdates();
        }
    }


    /// Initializes the performance optimization system

    private void InitializePerformanceOptimizer()
    {
        if (enableObjectPooling)
        {
            InitializeObjectPools();
        }

        Debug.Log("PerformanceOptimizer: Initialized with performance enhancements enabled");
    }


    /// Initializes object pools for frequently created objects

    private void InitializeObjectPools()
    {
        foreach (GameObject prefab in poolablePrefabs)
        {
            if (prefab != null)
            {
                string poolKey = prefab.name;
                objectPools[poolKey] = new Queue<GameObject>();
                activeObjects[poolKey] = new List<GameObject>();

                // Pre-populate pool
                for (int i = 0; i < poolSize / poolablePrefabs.Length; i++)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.SetActive(false);
                    obj.transform.parent = transform;
                    objectPools[poolKey].Enqueue(obj);
                }

                Debug.Log($"PerformanceOptimizer: Initialized pool for {poolKey} with {poolSize / poolablePrefabs.Length} objects");
            }
        }
    }


    /// Gets an object from the pool or creates a new one

    public GameObject GetPooledObject(string poolKey)
    {
        if (!enableObjectPooling || !objectPools.ContainsKey(poolKey))
        {
            return null;
        }

        GameObject obj;
        if (objectPools[poolKey].Count > 0)
        {
            obj = objectPools[poolKey].Dequeue();
        }
        else
        {
            // Create new object if pool is empty
            GameObject prefab = System.Array.Find(poolablePrefabs, p => p.name == poolKey);
            if (prefab != null)
            {
                obj = Instantiate(prefab);
                objectsCreated++;
            }
            else
            {
                return null;
            }
        }

        obj.SetActive(true);
        activeObjects[poolKey].Add(obj);
        return obj;
    }


    /// Returns an object to the pool

    public void ReturnToPool(GameObject obj, string poolKey)
    {
        if (!enableObjectPooling || !objectPools.ContainsKey(poolKey))
        {
            Destroy(obj);
            objectsDestroyed++;
            return;
        }

        obj.SetActive(false);
        obj.transform.parent = transform;

        activeObjects[poolKey].Remove(obj);
        objectPools[poolKey].Enqueue(obj);
    }


    /// Updates performance metrics

    private void UpdatePerformanceMetrics()
    {
        float currentFrameTime = Time.unscaledDeltaTime;
        frameTimes.Add(currentFrameTime);

        // Keep only recent frame times
        if (frameTimes.Count > 60)
        {
            frameTimes.RemoveAt(0);
        }

        // Calculate average
        float total = 0f;
        foreach (float time in frameTimes)
        {
            total += time;
        }
        averageFrameTime = total / frameTimes.Count;

        frameCount++;
    }


    /// Updates the LOD system based on camera distance

    private void UpdateLODSystem()
    {
        if (mainCamera == null) return;

        Vector3 cameraPos = mainCamera.transform.position;

        // Update existing LOD objects
        for (int i = lodObjects.Count - 1; i >= 0; i--)
        {
            if (lodObjects[i] == null || lodObjects[i].gameObject == null)
            {
                lodObjects.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(cameraPos, lodObjects[i].transform.position);
            lodObjects[i].UpdateLOD(distance, highDetailDistance, mediumDetailDistance, lowDetailDistance);
        }

        // Find new objects that need LOD management
        LODObject[] newLODObjects = FindObjectsOfType<LODObject>();
        foreach (LODObject lodObj in newLODObjects)
        {
            if (!lodObjects.Contains(lodObj))
            {
                lodObjects.Add(lodObj);
            }
        }
    }


    /// Optimizes network updates to reduce bandwidth

    private void OptimizeNetworkUpdates()
    {
        float currentTime = Time.time;

        // Find all objects that need network optimization
        EnhancedPlayerMovement[] players = FindObjectsOfType<EnhancedPlayerMovement>();

        foreach (EnhancedPlayerMovement player in players)
        {
            Transform playerTransform = player.transform;

            // Check if enough time has passed since last update
            if (lastUpdateTimes.ContainsKey(playerTransform))
            {
                if (currentTime - lastUpdateTimes[playerTransform] < 1f / maxUpdateRate)
                {
                    continue;
                }
            }

            // Check if position has changed significantly
            if (lastNetworkPositions.ContainsKey(playerTransform))
            {
                float distance = Vector3.Distance(playerTransform.position, lastNetworkPositions[playerTransform]);
                if (distance < positionUpdateThreshold)
                {
                    continue;
                }
            }

            // Update stored values
            lastNetworkPositions[playerTransform] = playerTransform.position;
            lastUpdateTimes[playerTransform] = currentTime;
        }
    }


    /// Performance monitoring coroutine

    private IEnumerator PerformanceMonitoringCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (enablePerformanceMonitoring)
            {
                float fps = 1f / averageFrameTime;

                // Log performance warnings
                if (fps < targetFrameRate * 0.8f)
                {
                    Debug.LogWarning($"PerformanceOptimizer: Low FPS detected - {fps:F1} FPS (target: {targetFrameRate})");
                }

                // Adjust LOD distances based on performance
                if (enableLODSystem && fps < targetFrameRate * 0.6f)
                {
                    highDetailDistance *= 0.9f;
                    mediumDetailDistance *= 0.9f;
                    Debug.Log("PerformanceOptimizer: Reducing LOD distances due to low performance");
                }
            }
        }
    }


    /// Automatic garbage collection coroutine

    private IEnumerator AutoGarbageCollectionCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(gcInterval);

            if (Time.time - lastGCTime >= gcInterval)
            {
                // Force garbage collection if memory usage is high
                long memoryBefore = System.GC.GetTotalMemory(false);
                System.GC.Collect();
                long memoryAfter = System.GC.GetTotalMemory(false);

                lastGCTime = Time.time;

                Debug.Log($"PerformanceOptimizer: GC completed - freed {(memoryBefore - memoryAfter) / 1024}KB");
            }
        }
    }


    /// Registers an object for LOD management

    public void RegisterLODObject(LODObject lodObject)
    {
        if (!lodObjects.Contains(lodObject))
        {
            lodObjects.Add(lodObject);
        }
    }


    /// Unregisters an object from LOD management

    public void UnregisterLODObject(LODObject lodObject)
    {
        lodObjects.Remove(lodObject);
    }


    /// Gets current performance metrics

    public PerformanceMetrics GetPerformanceMetrics()
    {
        return new PerformanceMetrics
        {
            currentFPS = 1f / averageFrameTime,
            averageFrameTime = averageFrameTime,
            objectsInPools = GetTotalPooledObjects(),
            activeObjects = GetTotalActiveObjects(),
            lodObjects = lodObjects.Count,
            memoryUsage = System.GC.GetTotalMemory(false) / 1024 / 1024 // MB
        };
    }


    /// Gets total objects in all pools

    private int GetTotalPooledObjects()
    {
        int total = 0;
        foreach (var pool in objectPools.Values)
        {
            total += pool.Count;
        }
        return total;
    }


    /// Gets total active objects

    private int GetTotalActiveObjects()
    {
        int total = 0;
        foreach (var activeList in activeObjects.Values)
        {
            total += activeList.Count;
        }
        return total;
    }


    /// Optimizes the current scene for better performance

    [ContextMenu("Optimize Scene")]
    public void OptimizeScene()
    {
        Debug.Log("PerformanceOptimizer: Starting scene optimization...");

        // Find and optimize renderers
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        int optimizedRenderers = 0;

        foreach (Renderer renderer in renderers)
        {
            // Add LOD component if not present
            if (renderer.GetComponent<LODObject>() == null)
            {
                LODObject lodComponent = renderer.gameObject.AddComponent<LODObject>();
                lodComponent.Initialize(renderer);
                RegisterLODObject(lodComponent);
                optimizedRenderers++;
            }
        }

        Debug.Log($"PerformanceOptimizer: Scene optimization complete - {optimizedRenderers} objects optimized");
    }

    private void OnGUI()
    {
        if (!enablePerformanceMonitoring || !Application.isPlaying) return;

        // Show performance metrics in bottom-left corner
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 250, 140));
        GUILayout.Label("Performance Monitor");

        PerformanceMetrics metrics = GetPerformanceMetrics();

        GUILayout.Label($"FPS: {metrics.currentFPS:F1} / {targetFrameRate}");
        GUILayout.Label($"Frame Time: {metrics.averageFrameTime * 1000f:F1}ms");
        GUILayout.Label($"Pool Objects: {metrics.objectsInPools}");
        GUILayout.Label($"Active Objects: {metrics.activeObjects}");
        GUILayout.Label($"LOD Objects: {metrics.lodObjects}");
        GUILayout.Label($"Memory: {metrics.memoryUsage}MB");

        // Performance status indicator
        Color statusColor = metrics.currentFPS >= targetFrameRate * 0.8f ? Color.green :
                           metrics.currentFPS >= targetFrameRate * 0.6f ? Color.yellow : Color.red;

        GUI.color = statusColor;
        GUILayout.Label($"Status: {(metrics.currentFPS >= targetFrameRate * 0.8f ? "GOOD" : metrics.currentFPS >= targetFrameRate * 0.6f ? "OK" : "POOR")}");
        GUI.color = Color.white;

        GUILayout.EndArea();
    }
}


/// Level-of-detail component for individual objects

public class LODObject : MonoBehaviour
{
    [Header("LOD Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private bool enableDistanceCulling = true;
    [SerializeField] private bool enableQualityScaling = true;

    private Material originalMaterial;
    private Material lowQualityMaterial;
    private bool isHighDetail = true;

    public void Initialize(Renderer renderer)
    {
        targetRenderer = renderer;
        if (targetRenderer != null)
        {
            originalMaterial = targetRenderer.material;
        }
    }

    public void UpdateLOD(float distance, float highDetail, float mediumDetail, float lowDetail)
    {
        if (targetRenderer == null) return;

        if (enableDistanceCulling)
        {
            // Cull objects that are too far away
            bool shouldBeVisible = distance <= lowDetail;
            if (targetRenderer.enabled != shouldBeVisible)
            {
                targetRenderer.enabled = shouldBeVisible;
            }
        }

        if (enableQualityScaling && targetRenderer.enabled)
        {
            // Adjust quality based on distance
            if (distance <= highDetail && !isHighDetail)
            {
                // Switch to high detail
                if (originalMaterial != null)
                {
                    targetRenderer.material = originalMaterial;
                }
                isHighDetail = true;
            }
            else if (distance > highDetail && isHighDetail)
            {
                // Switch to low detail
                if (lowQualityMaterial == null)
                {
                    CreateLowQualityMaterial();
                }
                if (lowQualityMaterial != null)
                {
                    targetRenderer.material = lowQualityMaterial;
                }
                isHighDetail = false;
            }
        }
    }

    private void CreateLowQualityMaterial()
    {
        if (originalMaterial != null)
        {
            lowQualityMaterial = new Material(originalMaterial);
            // Reduce quality - make texture smaller, remove effects, etc.
            lowQualityMaterial.name = originalMaterial.name + "_LOD";
        }
    }
}


/// Performance metrics data structure

[System.Serializable]
public struct PerformanceMetrics
{
    public float currentFPS;
    public float averageFrameTime;
    public int objectsInPools;
    public int activeObjects;
    public int lodObjects;
    public long memoryUsage;
}