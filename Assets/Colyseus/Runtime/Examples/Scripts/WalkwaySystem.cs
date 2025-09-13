using UnityEngine;
using System.Collections.Generic;


/// WalkwaySystem manages the procedural generation and layout of platform-based walkway
/// for the enhanced Akash 2D demo. Provides modular platform pieces for flexible level construction.

public class WalkwaySystem : MonoBehaviour
{
    [Header("Platform Configuration")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private float platformWidth = 10f;
    [SerializeField] private int platformCount = 8;
    [SerializeField] private float platformSpacing = 2f;
    [SerializeField] private Vector2 startPosition = Vector2.zero;

    [Header("Multi-Level Support")]
    [SerializeField] private bool enableMultiLevel = true;
    [SerializeField] private float[] platformHeights = { 0f, 3f, 6f };
    [SerializeField] private int platformsPerLevel = 3;

    [Header("Physics Settings")]
    [SerializeField] private LayerMask walkableLayer = 1;
    [SerializeField] private PhysicsMaterial2D platformMaterial;

    [Header("Boundary Management")]
    [SerializeField] private bool createBoundaries = true;
    [SerializeField] private float boundaryHeight = 10f;
    [SerializeField] private float boundaryThickness = 1f;

    [Header("Visual Settings")]
    [SerializeField] private Color platformColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color boundaryColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);

    // Runtime data
    private List<GameObject> platforms = new List<GameObject>();
    private List<GameObject> boundaries = new List<GameObject>();
    private float totalWalkwayWidth;

    // Events
    public System.Action<Vector2[]> OnWalkwayGenerated;
    public System.Action<Vector2> OnPlatformAdded;

    private void Start()
    {
        GenerateWalkway();
    }


    /// Generates the complete walkway system with platforms and boundaries

    public void GenerateWalkway()
    {
        Debug.Log("WalkwaySystem: Starting walkway generation...");

        // Clear existing platforms
        ClearExistingPlatforms();

        // Create platform prefab if not assigned
        if (platformPrefab == null)
        {
            CreateDefaultPlatformPrefab();
        }

        // Generate platforms
        if (enableMultiLevel)
        {
            GenerateMultiLevelPlatforms();
        }
        else
        {
            GenerateSingleLevelPlatforms();
        }

        // Create boundaries
        if (createBoundaries)
        {
            CreateWalkwayBoundaries();
        }

        // Calculate total width
        CalculateTotalWidth();

        // Notify listeners
        Vector2[] platformPositions = GetPlatformPositions();
        OnWalkwayGenerated?.Invoke(platformPositions);

        Debug.Log($"WalkwaySystem: Generated {platforms.Count} platforms across {totalWalkwayWidth} units");
    }


    /// Generates platforms in a single horizontal line

    private void GenerateSingleLevelPlatforms()
    {
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < platformCount; i++)
        {
            GameObject platform = CreatePlatform(currentPosition, 0);
            platforms.Add(platform);

            OnPlatformAdded?.Invoke(currentPosition);
            currentPosition.x += platformWidth + platformSpacing;
        }
    }


    /// Generates platforms across multiple height levels

    private void GenerateMultiLevelPlatforms()
    {
        Vector2 currentPosition = startPosition;
        int currentLevelIndex = 0;
        int platformsInCurrentLevel = 0;

        for (int i = 0; i < platformCount; i++)
        {
            // Determine height based on current level
            float height = platformHeights[currentLevelIndex % platformHeights.Length];
            Vector2 platformPos = new Vector2(currentPosition.x, startPosition.y + height);

            GameObject platform = CreatePlatform(platformPos, currentLevelIndex);
            platforms.Add(platform);

            OnPlatformAdded?.Invoke(platformPos);

            // Move to next position
            currentPosition.x += platformWidth + platformSpacing;
            platformsInCurrentLevel++;

            // Check if we should move to next level
            if (platformsInCurrentLevel >= platformsPerLevel)
            {
                currentLevelIndex++;
                platformsInCurrentLevel = 0;
            }
        }
    }


    /// Creates a single platform at the specified position

    private GameObject CreatePlatform(Vector2 position, int levelIndex)
    {
        GameObject platform = Instantiate(platformPrefab, transform);
        platform.transform.position = new Vector3(position.x, position.y, 0);
        platform.name = $"Platform_{platforms.Count}_Level{levelIndex}";

        // Ensure proper physics setup
        SetupPlatformPhysics(platform);

        // Setup visual appearance
        SetupPlatformVisuals(platform, levelIndex);

        return platform;
    }


    /// Sets up physics components for a platform

    private void SetupPlatformPhysics(GameObject platform)
    {
        // Ensure BoxCollider2D exists
        BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = platform.AddComponent<BoxCollider2D>();
        }

        // Configure collider
        collider.size = new Vector2(platformWidth, 1f);
        collider.isTrigger = false;

        // Set layer
        platform.layer = (int)Mathf.Log(walkableLayer.value, 2);

        // Apply physics material if available
        if (platformMaterial != null)
        {
            collider.sharedMaterial = platformMaterial;
        }
    }


    /// Sets up visual components for a platform

    private void SetupPlatformVisuals(GameObject platform, int levelIndex)
    {
        SpriteRenderer renderer = platform.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = platform.AddComponent<SpriteRenderer>();
        }

        // Create or get sprite
        if (renderer.sprite == null)
        {
            renderer.sprite = CreatePlatformSprite();
        }

        // Set color with slight variation based on level
        Color levelColor = platformColor;
        levelColor.r += levelIndex * 0.1f;
        levelColor.g += levelIndex * 0.05f;
        renderer.color = levelColor;

        // Set sorting order
        renderer.sortingOrder = -levelIndex;
    }


    /// Creates a default platform sprite

    private Sprite CreatePlatformSprite()
    {
        int width = Mathf.RoundToInt(platformWidth * 32); // 32 pixels per unit
        int height = 32;
        Texture2D texture = new Texture2D(width, height);

        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
    }


    /// Creates boundary colliders to prevent players from falling off

    private void CreateWalkwayBoundaries()
    {
        if (platforms.Count == 0) return;

        // Calculate boundary positions
        float leftBound = platforms[0].transform.position.x - platformWidth * 0.5f - boundaryThickness;
        float rightBound = platforms[platforms.Count - 1].transform.position.x + platformWidth * 0.5f + boundaryThickness;

        // Create left boundary
        CreateBoundary("LeftBoundary", new Vector2(leftBound, startPosition.y + boundaryHeight * 0.5f));

        // Create right boundary
        CreateBoundary("RightBoundary", new Vector2(rightBound, startPosition.y + boundaryHeight * 0.5f));
    }


    /// Creates a single boundary collider

    private void CreateBoundary(string name, Vector2 position)
    {
        GameObject boundary = new GameObject(name);
        boundary.transform.parent = transform;
        boundary.transform.position = new Vector3(position.x, position.y, 0);

        // Add collider
        BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(boundaryThickness, boundaryHeight);
        collider.isTrigger = false;

        // Add visual component
        SpriteRenderer renderer = boundary.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateBoundarySprite();
        renderer.color = boundaryColor;
        renderer.sortingOrder = 10;

        boundaries.Add(boundary);
    }


    /// Creates a sprite for boundary walls

    private Sprite CreateBoundarySprite()
    {
        int width = Mathf.RoundToInt(boundaryThickness * 32);
        int height = Mathf.RoundToInt(boundaryHeight * 32);
        Texture2D texture = new Texture2D(width, height);

        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
    }


    /// Creates a default platform prefab if none is assigned

    private void CreateDefaultPlatformPrefab()
    {
        platformPrefab = new GameObject("DefaultPlatform");
        platformPrefab.AddComponent<SpriteRenderer>();
        platformPrefab.AddComponent<BoxCollider2D>();

        // Don't instantiate the prefab yet, it will be used as a template
        platformPrefab.SetActive(false);
    }


    /// Clears all existing platforms and boundaries

    private void ClearExistingPlatforms()
    {
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                DestroyImmediate(platform);
            }
        }
        platforms.Clear();

        foreach (GameObject boundary in boundaries)
        {
            if (boundary != null)
            {
                DestroyImmediate(boundary);
            }
        }
        boundaries.Clear();
    }


    /// Calculates the total width of the walkway

    private void CalculateTotalWidth()
    {
        if (platforms.Count == 0)
        {
            totalWalkwayWidth = 0f;
            return;
        }

        float leftMost = platforms[0].transform.position.x - platformWidth * 0.5f;
        float rightMost = platforms[platforms.Count - 1].transform.position.x + platformWidth * 0.5f;
        totalWalkwayWidth = rightMost - leftMost;
    }


    /// Gets positions of all platforms

    public Vector2[] GetPlatformPositions()
    {
        Vector2[] positions = new Vector2[platforms.Count];
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i] != null)
            {
                positions[i] = platforms[i].transform.position;
            }
        }
        return positions;
    }


    /// Gets the nearest platform to a given position

    public Vector2 GetNearestPlatformPosition(Vector2 worldPosition)
    {
        if (platforms.Count == 0) return Vector2.zero;

        Vector2 nearest = platforms[0].transform.position;
        float nearestDistance = Vector2.Distance(worldPosition, nearest);

        foreach (GameObject platform in platforms)
        {
            if (platform == null) continue;

            Vector2 platformPos = platform.transform.position;
            float distance = Vector2.Distance(worldPosition, platformPos);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = platformPos;
            }
        }

        return nearest;
    }


    /// Checks if a position is on a valid platform

    public bool IsValidPlatformPosition(Vector2 worldPosition, float tolerance = 1f)
    {
        foreach (GameObject platform in platforms)
        {
            if (platform == null) continue;

            BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
            if (collider != null && collider.bounds.Contains(worldPosition))
            {
                return true;
            }
        }
        return false;
    }


    /// Gets the total width of the walkway

    public float GetTotalWalkwayWidth()
    {
        return totalWalkwayWidth;
    }


    /// Gets the walkway bounds (min and max X positions)

    public Vector2 GetWalkwayBounds()
    {
        if (platforms.Count == 0) return Vector2.zero;

        float minX = platforms[0].transform.position.x - platformWidth * 0.5f;
        float maxX = platforms[platforms.Count - 1].transform.position.x + platformWidth * 0.5f;

        return new Vector2(minX, maxX);
    }


    /// Regenerates the walkway with current settings

    [ContextMenu("Regenerate Walkway")]
    public void RegenerateWalkway()
    {
        GenerateWalkway();
    }

    private void OnDrawGizmos()
    {
        // Draw platform positions in editor
        Gizmos.color = Color.green;
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                Gizmos.DrawWireCube(platform.transform.position, new Vector3(platformWidth, 1f, 0f));
            }
        }

        // Draw walkway bounds
        if (platforms.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Vector2 bounds = GetWalkwayBounds();
            Vector3 center = new Vector3((bounds.x + bounds.y) * 0.5f, startPosition.y, 0);
            Vector3 size = new Vector3(bounds.y - bounds.x, 0.5f, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}