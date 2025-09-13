using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// CameraBehaviorSystem provides dynamic camera management for the enhanced Akash 2D walkway demo.
/// Features smooth player following, boundary constraints, and adaptive framing for professional presentation.

public class CameraBehaviorSystem : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Vector2 offset = new Vector2(0, 2f);
    [SerializeField] private bool autoFindTarget = true;

    [Header("Smoothing & Damping")]
    [SerializeField] private float positionSmoothTime = 0.3f;
    [SerializeField] private float zoomSmoothTime = 0.5f;
    [SerializeField] private bool enablePredictiveTracking = true;
    [SerializeField] private float predictionTime = 0.2f;

    [Header("Constraints")]
    [SerializeField] private Vector2 minBounds = new Vector2(-20f, -5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(20f, 15f);
    [SerializeField] private bool useWalkwayBounds = true;
    [SerializeField] private float boundsMargin = 2f;

    [Header("Zoom Settings")]
    [SerializeField] private float baseOrthographicSize = 8f;
    [SerializeField] private float minZoom = 4f;
    [SerializeField] private float maxZoom = 12f;
    [SerializeField] private bool adaptiveZoom = true;
    [SerializeField] private float zoomPadding = 3f;

    [Header("Multi-Player Support")]
    [SerializeField] private bool trackMultiplePlayers = true;
    [SerializeField] private float multiPlayerZoomFactor = 1.5f;
    [SerializeField] private float playerGroupingThreshold = 10f;

    [Header("Cinematic Effects")]
    [SerializeField] private bool enableCinematicTransitions = true;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float transitionDuration = 1f;

    // Camera components
    private Camera targetCamera;
    private Vector3 velocity;
    private float zoomVelocity;

    // Walkway integration
    private WalkwaySystem walkwaySystem;
    private EnhancedPlayerMovement playerMovement;

    // Multi-player tracking
    private List<Transform> allPlayers = new List<Transform>();
    private Vector3 lastTargetPosition;
    private Vector2 playerGroupCenter;
    private float playerGroupRadius;

    // State management
    private bool isInitialized = false;
    private Vector3 currentTargetPosition;
    private float currentTargetZoom;

    // Events
    public System.Action<Transform> OnTargetChanged;
    public System.Action<Vector2> OnBoundsUpdated;
    public System.Action<float> OnZoomChanged;

    private void Awake()
    {
        // Get camera component
        targetCamera = GetComponent<Camera>();
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("CameraBehaviorSystem: No camera found!");
            return;
        }

        // Initialize camera settings
        targetCamera.orthographicSize = baseOrthographicSize;
        currentTargetZoom = baseOrthographicSize;
    }

    private void Start()
    {
        // Find system components
        FindSystemComponents();

        // Auto-find target if enabled
        if (autoFindTarget && followTarget == null)
        {
            FindFollowTarget();
        }

        // Setup initial bounds from walkway
        if (useWalkwayBounds && walkwaySystem != null)
        {
            UpdateBoundsFromWalkway();
        }

        // Initialize position
        if (followTarget != null)
        {
            transform.position = GetTargetCameraPosition(followTarget.position);
            lastTargetPosition = followTarget.position;
        }

        isInitialized = true;
        Debug.Log("CameraBehaviorSystem: Initialized with camera following enabled");
    }

    private void LateUpdate()
    {
        if (!isInitialized || targetCamera == null) return;

        UpdatePlayerTracking();
        UpdateCameraPosition();
        UpdateCameraZoom();
    }


    /// Finds and connects to system components

    private void FindSystemComponents()
    {
        walkwaySystem = FindObjectOfType<WalkwaySystem>();
        playerMovement = FindObjectOfType<EnhancedPlayerMovement>();

        if (walkwaySystem != null)
        {
            walkwaySystem.OnWalkwayGenerated += OnWalkwayGenerated;
        }
    }


    /// Finds the primary follow target

    private void FindFollowTarget()
    {
        // Try to find enhanced player movement first
        if (playerMovement != null)
        {
            SetFollowTarget(playerMovement.transform);
            return;
        }

        // Fallback to regular player movement
        PlayerMovement regularPlayer = FindObjectOfType<PlayerMovement>();
        if (regularPlayer != null)
        {
            SetFollowTarget(regularPlayer.transform);
            return;
        }

        Debug.LogWarning("CameraBehaviorSystem: No valid follow target found");
    }


    /// Sets the primary follow target

    public void SetFollowTarget(Transform newTarget)
    {
        if (newTarget == followTarget) return;

        Transform oldTarget = followTarget;
        followTarget = newTarget;

        if (enableCinematicTransitions && oldTarget != null && newTarget != null)
        {
            StartCoroutine(TransitionToNewTarget(oldTarget.position, newTarget.position));
        }

        OnTargetChanged?.Invoke(newTarget);
        Debug.Log($"CameraBehaviorSystem: Follow target changed to {(newTarget != null ? newTarget.name : "null")}");
    }


    /// Updates tracking of all players in the scene

    private void UpdatePlayerTracking()
    {
        if (!trackMultiplePlayers) return;

        // Clear and rebuild player list
        allPlayers.Clear();

        // Add main player
        if (followTarget != null)
        {
            allPlayers.Add(followTarget);
        }

        // Find other players
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerObjects)
        {
            if (player.transform != followTarget)
            {
                allPlayers.Add(player.transform);
            }
        }

        // Also look for objects with "Player_" prefix
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith("Player_") && !allPlayers.Contains(obj.transform))
            {
                allPlayers.Add(obj.transform);
            }
        }

        // Calculate group center and radius
        CalculatePlayerGroup();
    }


    /// Calculates the center and radius of the player group

    private void CalculatePlayerGroup()
    {
        if (allPlayers.Count == 0) return;

        // Calculate center
        Vector2 center = Vector2.zero;
        foreach (Transform player in allPlayers)
        {
            if (player != null)
            {
                center += (Vector2)player.position;
            }
        }
        center /= allPlayers.Count;
        playerGroupCenter = center;

        // Calculate radius (max distance from center)
        float maxDistance = 0f;
        foreach (Transform player in allPlayers)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(center, player.position);
                maxDistance = Mathf.Max(maxDistance, distance);
            }
        }
        playerGroupRadius = maxDistance;
    }


    /// Updates camera position with smooth following

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition;

        if (trackMultiplePlayers && allPlayers.Count > 1 && playerGroupRadius > playerGroupingThreshold)
        {
            // Track group of players
            targetPosition = GetTargetCameraPosition(playerGroupCenter);
        }
        else if (followTarget != null)
        {
            // Track single target
            Vector3 targetPos = followTarget.position;

            // Apply predictive tracking
            if (enablePredictiveTracking && lastTargetPosition != Vector3.zero)
            {
                Vector3 velocity = (targetPos - lastTargetPosition) / Time.deltaTime;
                targetPos += velocity * predictionTime;
            }

            targetPosition = GetTargetCameraPosition(targetPos);
            lastTargetPosition = followTarget.position;
        }
        else
        {
            return; // No target to follow
        }

        // Apply bounds constraints
        targetPosition = ApplyBoundsConstraints(targetPosition);

        // Smooth movement
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            positionSmoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );

        transform.position = smoothedPosition;
        currentTargetPosition = targetPosition;
    }


    /// Calculates the target camera position based on follow target

    private Vector3 GetTargetCameraPosition(Vector3 targetWorldPosition)
    {
        Vector3 cameraPosition = targetWorldPosition + (Vector3)offset;
        cameraPosition.z = transform.position.z; // Maintain camera's Z position
        return cameraPosition;
    }


    /// Applies boundary constraints to camera position

    private Vector3 ApplyBoundsConstraints(Vector3 targetPosition)
    {
        Vector2 constrainedPosition = new Vector2(targetPosition.x, targetPosition.y);

        // Apply min/max bounds
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, minBounds.x, maxBounds.x);
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, minBounds.y, maxBounds.y);

        return new Vector3(constrainedPosition.x, constrainedPosition.y, targetPosition.z);
    }


    /// Updates camera zoom with adaptive behavior

    private void UpdateCameraZoom()
    {
        float targetZoom = baseOrthographicSize;

        if (adaptiveZoom)
        {
            if (trackMultiplePlayers && allPlayers.Count > 1)
            {
                // Zoom out to fit all players
                targetZoom = Mathf.Max(baseOrthographicSize, playerGroupRadius * multiPlayerZoomFactor + zoomPadding);
            }
            else
            {
                // Standard zoom for single player
                targetZoom = baseOrthographicSize;
            }
        }

        // Clamp zoom
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smooth zoom transition
        if (Mathf.Abs(targetZoom - targetCamera.orthographicSize) > 0.1f)
        {
            float smoothedZoom = Mathf.SmoothDamp(
                targetCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime,
                Mathf.Infinity,
                Time.deltaTime
            );

            targetCamera.orthographicSize = smoothedZoom;
            currentTargetZoom = targetZoom;

            OnZoomChanged?.Invoke(smoothedZoom);
        }
    }


    /// Updates bounds based on walkway system

    private void UpdateBoundsFromWalkway()
    {
        if (walkwaySystem == null) return;

        Vector2 walkwayBounds = walkwaySystem.GetWalkwayBounds();
        if (walkwayBounds != Vector2.zero)
        {
            minBounds.x = walkwayBounds.x - boundsMargin;
            maxBounds.x = walkwayBounds.y + boundsMargin;

            OnBoundsUpdated?.Invoke(new Vector2(minBounds.x, maxBounds.x));
            Debug.Log($"CameraBehaviorSystem: Updated bounds from walkway: {minBounds.x} to {maxBounds.x}");
        }
    }


    /// Called when walkway is generated

    private void OnWalkwayGenerated(Vector2[] platformPositions)
    {
        if (useWalkwayBounds)
        {
            UpdateBoundsFromWalkway();
        }
    }


    /// Smoothly transitions camera to a new target

    private IEnumerator TransitionToNewTarget(Vector3 fromPosition, Vector3 toPosition)
    {
        float elapsed = 0f;
        Vector3 startCameraPos = transform.position;
        Vector3 endCameraPos = GetTargetCameraPosition(toPosition);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            t = transitionCurve.Evaluate(t);

            Vector3 currentPos = Vector3.Lerp(startCameraPos, endCameraPos, t);
            transform.position = ApplyBoundsConstraints(currentPos);

            yield return null;
        }

        transform.position = ApplyBoundsConstraints(endCameraPos);
    }


    /// Focuses camera on a specific position with smooth transition

    public void FocusOnPosition(Vector3 worldPosition, float duration = 1f)
    {
        StartCoroutine(FocusOnPositionCoroutine(worldPosition, duration));
    }

    private IEnumerator FocusOnPositionCoroutine(Vector3 worldPosition, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = GetTargetCameraPosition(worldPosition);
        targetPos = ApplyBoundsConstraints(targetPos);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = transitionCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }


    /// Sets the camera zoom level

    public void SetZoom(float newZoom, bool immediate = false)
    {
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        if (immediate)
        {
            targetCamera.orthographicSize = newZoom;
            currentTargetZoom = newZoom;
        }
        else
        {
            currentTargetZoom = newZoom;
        }
    }


    /// Shakes the camera for impact effects

    public void ShakeCamera(float intensity = 1f, float duration = 0.5f)
    {
        StartCoroutine(CameraShakeCoroutine(intensity, duration));
    }

    private IEnumerator CameraShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = Mathf.Lerp(intensity, 0f, elapsed / duration);

            Vector3 shakeOffset = Random.insideUnitSphere * strength;
            shakeOffset.z = 0f;

            transform.position = originalPosition + shakeOffset;
            yield return null;
        }

        transform.position = originalPosition;
    }


    /// Gets the current camera view bounds in world space

    public Bounds GetCameraViewBounds()
    {
        float height = targetCamera.orthographicSize * 2f;
        float width = height * targetCamera.aspect;

        Vector3 center = transform.position;
        center.z = 0f;

        return new Bounds(center, new Vector3(width, height, 0f));
    }


    /// Checks if a world position is visible in the camera view

    public bool IsPositionVisible(Vector3 worldPosition)
    {
        return GetCameraViewBounds().Contains(worldPosition);
    }


    /// Gets the world position at the edge of the camera view

    public Vector3 GetViewEdgePosition(Vector2 direction)
    {
        Bounds viewBounds = GetCameraViewBounds();
        Vector3 center = viewBounds.center;

        direction = direction.normalized;
        Vector3 edge = center + new Vector3(
            direction.x * viewBounds.extents.x,
            direction.y * viewBounds.extents.y,
            0f
        );

        return edge;
    }

    private void OnDrawGizmos()
    {
        // Draw camera bounds
        Gizmos.color = Color.cyan;
        Vector3 boundsCenter = new Vector3((minBounds.x + maxBounds.x) * 0.5f, (minBounds.y + maxBounds.y) * 0.5f, 0);
        Vector3 boundsSize = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);

        // Draw current view bounds
        if (targetCamera != null)
        {
            Gizmos.color = Color.yellow;
            Bounds viewBounds = GetCameraViewBounds();
            Gizmos.DrawWireCube(viewBounds.center, viewBounds.size);
        }

        // Draw follow target
        if (followTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(followTarget.position, 0.5f);
        }

        // Draw player group
        if (trackMultiplePlayers && allPlayers.Count > 1)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(playerGroupCenter, playerGroupRadius);
        }
    }

    private void OnDestroy()
    {
        if (walkwaySystem != null)
        {
            walkwaySystem.OnWalkwayGenerated -= OnWalkwayGenerated;
        }
    }
}