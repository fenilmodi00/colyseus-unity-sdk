using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// AkashLogoBannerSystem manages the strategic placement and interactive effects of Akash Network banners
/// throughout the 2D walkway demo. Provides brand visibility and engagement features.

public class AkashLogoBannerSystem : MonoBehaviour
{
    [Header("Banner Configuration")]
    [SerializeField] private GameObject bannerPrefab;
    [SerializeField] private float bannerScale = 2f;
    [SerializeField] private int bannersPerWalkway = 4;
    [SerializeField] private Vector2 bannerOffset = new Vector2(0, 3f);

    [Header("Interactive Effects")]
    [SerializeField] private bool enableProximityEffects = true;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
    [SerializeField] private AnimationCurve glowCurve = AnimationCurve.EaseInOut(0, 0.5f, 1, 1f);

    [Header("Visual Settings")]
    [SerializeField] private Color akashPrimaryColor = new Color(0.95f, 0.18f, 0.18f, 1f); // Akash red
    [SerializeField] private Color akashSecondaryColor = new Color(0.1f, 0.1f, 0.2f, 1f); // Dark blue
    [SerializeField] private Color glowColor = new Color(1f, 0.5f, 0.5f, 0.8f);

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private float proximityAnimationDuration = 1f;
    [SerializeField] private bool enableIdleAnimation = true;
    [SerializeField] private float idleAnimationAmplitude = 0.1f;

    // Runtime data
    private List<AkashBanner> banners = new List<AkashBanner>();
    private WalkwaySystem walkwaySystem;
    private List<Transform> playersInScene = new List<Transform>();

    // Events
    public System.Action<AkashBanner, Transform> OnPlayerNearBanner;
    public System.Action<AkashBanner, Transform> OnPlayerLeftBanner;
    public System.Action<int> OnBannersCreated;

    private void Awake()
    {
        // Find walkway system
        walkwaySystem = FindObjectOfType<WalkwaySystem>();
        if (walkwaySystem != null)
        {
            // Subscribe to walkway generation
            walkwaySystem.OnWalkwayGenerated += OnWalkwayGenerated;
        }
    }

    private void Start()
    {
        // If walkway already exists, generate banners immediately
        if (walkwaySystem != null && walkwaySystem.GetTotalWalkwayWidth() > 0)
        {
            GenerateBanners();
        }
    }

    private void Update()
    {
        if (enableProximityEffects)
        {
            UpdateProximityEffects();
        }

        if (enableIdleAnimation)
        {
            UpdateIdleAnimations();
        }
    }


    /// Called when walkway system generates platforms

    private void OnWalkwayGenerated(Vector2[] platformPositions)
    {
        GenerateBanners();
    }


    /// Generates banners along the walkway at strategic positions

    public void GenerateBanners()
    {
        if (walkwaySystem == null)
        {
            Debug.LogWarning("AkashLogoBannerSystem: No WalkwaySystem found, cannot generate banners");
            return;
        }

        // Clear existing banners
        ClearExistingBanners();

        Vector2[] platformPositions = walkwaySystem.GetPlatformPositions();
        if (platformPositions.Length == 0)
        {
            Debug.LogWarning("AkashLogoBannerSystem: No platforms found, cannot place banners");
            return;
        }

        // Calculate banner placement positions
        Vector2[] bannerPositions = CalculateBannerPositions(platformPositions);

        // Create banners
        for (int i = 0; i < bannerPositions.Length; i++)
        {
            CreateBanner(bannerPositions[i], i);
        }

        OnBannersCreated?.Invoke(banners.Count);
        Debug.Log($"AkashLogoBannerSystem: Generated {banners.Count} banners along walkway");
    }


    /// Calculates strategic positions for banner placement

    private Vector2[] CalculateBannerPositions(Vector2[] platformPositions)
    {
        List<Vector2> positions = new List<Vector2>();

        if (platformPositions.Length <= bannersPerWalkway)
        {
            // Place banner above each platform if we have fewer platforms than desired banners
            foreach (Vector2 platform in platformPositions)
            {
                positions.Add(platform + bannerOffset);
            }
        }
        else
        {
            // Distribute banners evenly across the walkway
            float walkwayLength = Vector2.Distance(platformPositions[0], platformPositions[platformPositions.Length - 1]);
            float spacing = walkwayLength / (bannersPerWalkway - 1);

            for (int i = 0; i < bannersPerWalkway; i++)
            {
                float t = i / (float)(bannersPerWalkway - 1);
                Vector2 position = Vector2.Lerp(platformPositions[0], platformPositions[platformPositions.Length - 1], t);
                position += bannerOffset;
                positions.Add(position);
            }
        }

        return positions.ToArray();
    }


    /// Creates a single banner at the specified position

    private void CreateBanner(Vector2 position, int index)
    {
        // Create banner object
        GameObject bannerObject = new GameObject($"AkashBanner_{index}");
        bannerObject.transform.parent = transform;
        bannerObject.transform.position = new Vector3(position.x, position.y, 0);
        bannerObject.transform.localScale = Vector3.one * bannerScale;

        // Add AkashBanner component
        AkashBanner banner = bannerObject.AddComponent<AkashBanner>();
        banner.Initialize(this, index);

        // Setup visual components
        SetupBannerVisuals(bannerObject, banner);

        banners.Add(banner);
    }


    /// Sets up visual components for a banner

    private void SetupBannerVisuals(GameObject bannerObject, AkashBanner banner)
    {
        // Create banner background
        GameObject background = new GameObject("Background");
        background.transform.parent = bannerObject.transform;
        background.transform.localPosition = Vector3.zero;

        SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = CreateBannerBackgroundSprite();
        bgRenderer.color = akashSecondaryColor;
        bgRenderer.sortingOrder = 0;

        // Create Akash logo
        GameObject logo = new GameObject("AkashLogo");
        logo.transform.parent = bannerObject.transform;
        logo.transform.localPosition = Vector3.zero;

        SpriteRenderer logoRenderer = logo.AddComponent<SpriteRenderer>();
        logoRenderer.sprite = CreateAkashLogoSprite();
        logoRenderer.color = akashPrimaryColor;
        logoRenderer.sortingOrder = 1;

        // Create glow effect
        GameObject glow = new GameObject("Glow");
        glow.transform.parent = bannerObject.transform;
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 1.2f;

        SpriteRenderer glowRenderer = glow.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = CreateGlowSprite();
        glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f); // Start invisible
        glowRenderer.sortingOrder = -1;

        // Store references in banner component
        banner.SetRenderers(bgRenderer, logoRenderer, glowRenderer);
    }


    /// Creates a banner background sprite

    private Sprite CreateBannerBackgroundSprite()
    {
        int width = 128;
        int height = 64;
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


    /// Creates an Akash logo sprite (simplified version)

    private Sprite CreateAkashLogoSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);

                // Create a simple "A" shape for Akash
                if (CreateAkashLetterA(x, y, size))
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }


    /// Creates a simple "A" shape for the Akash logo

    private bool CreateAkashLetterA(int x, int y, int size)
    {
        float centerX = size * 0.5f;
        float centerY = size * 0.5f;

        // Triangle shape for "A"
        float leftEdge = centerX - (size * 0.25f) + (y - centerY) * 0.3f;
        float rightEdge = centerX + (size * 0.25f) - (y - centerY) * 0.3f;
        bool crossBar = Mathf.Abs(y - centerY + 5) < 3 && x > centerX - 15 && x < centerX + 15;

        return (x >= leftEdge && x <= rightEdge && y > centerY - 20) || crossBar;
    }


    /// Creates a glow effect sprite

    private Sprite CreateGlowSprite()
    {
        int size = 96;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                float normalizedDistance = distance / (size * 0.5f);

                float alpha = Mathf.Clamp01(1f - normalizedDistance);
                alpha = Mathf.Pow(alpha, 2f); // Softer falloff

                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }


    /// Updates proximity effects for all banners

    private void UpdateProximityEffects()
    {
        // Find all players in scene
        UpdatePlayerList();

        foreach (AkashBanner banner in banners)
        {
            bool playerNearby = false;
            Transform nearestPlayer = null;
            float nearestDistance = float.MaxValue;

            // Check each player's distance to this banner
            foreach (Transform player in playersInScene)
            {
                if (player == null) continue;

                float distance = Vector2.Distance(banner.transform.position, player.position);
                if (distance <= detectionRadius)
                {
                    playerNearby = true;
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }

            // Update banner proximity state
            banner.SetPlayerNearby(playerNearby, nearestPlayer, nearestDistance);
        }
    }


    /// Updates the list of players in the scene

    private void UpdatePlayerList()
    {
        playersInScene.Clear();

        // Find main player
        EnhancedPlayerMovement mainPlayer = FindObjectOfType<EnhancedPlayerMovement>();
        if (mainPlayer != null)
        {
            playersInScene.Add(mainPlayer.transform);
        }

        // Find other players (they usually have "Player_" in their name)
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.StartsWith("Player_") && obj.transform != null)
            {
                playersInScene.Add(obj.transform);
            }
        }
    }


    /// Updates idle animations for all banners

    private void UpdateIdleAnimations()
    {
        float time = Time.time * animationSpeed;

        for (int i = 0; i < banners.Count; i++)
        {
            AkashBanner banner = banners[i];
            if (banner == null) continue;

            // Create slight floating animation with phase offset
            float phase = i * 0.5f;
            float offset = Mathf.Sin(time + phase) * idleAnimationAmplitude;

            Vector3 originalPos = banner.GetOriginalPosition();
            banner.transform.position = new Vector3(originalPos.x, originalPos.y + offset, originalPos.z);
        }
    }


    /// Clears all existing banners

    private void ClearExistingBanners()
    {
        foreach (AkashBanner banner in banners)
        {
            if (banner != null && banner.gameObject != null)
            {
                DestroyImmediate(banner.gameObject);
            }
        }
        banners.Clear();
    }


    /// Gets the number of banners currently in the scene

    public int GetBannerCount()
    {
        return banners.Count;
    }


    /// Gets all banner positions

    public Vector2[] GetBannerPositions()
    {
        Vector2[] positions = new Vector2[banners.Count];
        for (int i = 0; i < banners.Count; i++)
        {
            if (banners[i] != null)
            {
                positions[i] = banners[i].transform.position;
            }
        }
        return positions;
    }


    /// Triggers a special effect on all banners

    public void TriggerGlobalEffect()
    {
        foreach (AkashBanner banner in banners)
        {
            if (banner != null)
            {
                banner.TriggerSpecialEffect();
            }
        }
    }


    /// Called when a player approaches a banner

    public void NotifyPlayerNearBanner(AkashBanner banner, Transform player)
    {
        OnPlayerNearBanner?.Invoke(banner, player);
    }


    /// Called when a player leaves a banner area

    public void NotifyPlayerLeftBanner(AkashBanner banner, Transform player)
    {
        OnPlayerLeftBanner?.Invoke(banner, player);
    }

    // Public accessors for AkashBanner class
    public float GetDetectionRadius() { return detectionRadius; }
    public AnimationCurve GetGlowCurve() { return glowCurve; }
    public Color GetGlowColor() { return glowColor; }
    public float GetProximityAnimationDuration() { return proximityAnimationDuration; }
    public AnimationCurve GetScaleCurve() { return scaleCurve; }

    [ContextMenu("Regenerate Banners")]
    public void RegenerateBanners()
    {
        GenerateBanners();
    }

    private void OnDrawGizmos()
    {
        // Draw detection radius for each banner
        Gizmos.color = Color.yellow;
        foreach (AkashBanner banner in banners)
        {
            if (banner != null)
            {
                Gizmos.DrawWireSphere(banner.transform.position, detectionRadius);
            }
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


/// Individual banner component that handles its own effects and animations

public class AkashBanner : MonoBehaviour
{
    private AkashLogoBannerSystem bannerSystem;
    private int bannerIndex;
    private Vector3 originalPosition;

    // Visual components
    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer logoRenderer;
    private SpriteRenderer glowRenderer;

    // State
    private bool playerNearby;
    private Transform nearestPlayer;
    private float proximityDistance;
    private Coroutine effectCoroutine;

    public void Initialize(AkashLogoBannerSystem system, int index)
    {
        bannerSystem = system;
        bannerIndex = index;
        originalPosition = transform.position;
    }

    public void SetRenderers(SpriteRenderer bg, SpriteRenderer logo, SpriteRenderer glow)
    {
        backgroundRenderer = bg;
        logoRenderer = logo;
        glowRenderer = glow;
    }

    public void SetPlayerNearby(bool nearby, Transform player, float distance)
    {
        if (nearby != playerNearby)
        {
            playerNearby = nearby;
            nearestPlayer = player;
            proximityDistance = distance;

            if (nearby)
            {
                OnPlayerEntered();
            }
            else
            {
                OnPlayerExited();
            }
        }
        else if (nearby)
        {
            proximityDistance = distance;
            UpdateProximityEffects();
        }
    }

    private void OnPlayerEntered()
    {
        if (bannerSystem != null)
        {
            bannerSystem.NotifyPlayerNearBanner(this, nearestPlayer);
        }

        // Start proximity effect
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(ProximityEffectCoroutine(true));
    }

    private void OnPlayerExited()
    {
        if (bannerSystem != null)
        {
            bannerSystem.NotifyPlayerLeftBanner(this, nearestPlayer);
        }

        // End proximity effect
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(ProximityEffectCoroutine(false));
    }

    private void UpdateProximityEffects()
    {
        // Update glow intensity based on distance
        if (glowRenderer != null && playerNearby)
        {
            float normalizedDistance = Mathf.Clamp01(proximityDistance / bannerSystem.GetDetectionRadius());
            float glowIntensity = bannerSystem.GetGlowCurve().Evaluate(1f - normalizedDistance);

            Color glowColor = bannerSystem.GetGlowColor();
            glowColor.a = glowIntensity;
            glowRenderer.color = glowColor;
        }
    }

    private IEnumerator ProximityEffectCoroutine(bool entering)
    {
        float duration = bannerSystem.GetProximityAnimationDuration();
        float elapsed = 0f;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = entering ? startScale * bannerSystem.GetScaleCurve().Evaluate(1f) : startScale / bannerSystem.GetScaleCurve().Evaluate(1f);

        Color startGlow = glowRenderer != null ? glowRenderer.color : Color.clear;
        Color targetGlow = entering ? bannerSystem.GetGlowColor() : Color.clear;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Scale animation
            transform.localScale = Vector3.Lerp(startScale, targetScale, bannerSystem.GetScaleCurve().Evaluate(t));

            // Glow animation
            if (glowRenderer != null)
            {
                glowRenderer.color = Color.Lerp(startGlow, targetGlow, t);
            }

            yield return null;
        }

        // Ensure final values
        transform.localScale = targetScale;
        if (glowRenderer != null)
        {
            glowRenderer.color = targetGlow;
        }

        effectCoroutine = null;
    }

    public void TriggerSpecialEffect()
    {
        StartCoroutine(SpecialEffectCoroutine());
    }

    private IEnumerator SpecialEffectCoroutine()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float pulse = 1f + Mathf.Sin(t * Mathf.PI * 4f) * 0.1f;
            transform.localScale = originalScale * pulse;

            yield return null;
        }

        transform.localScale = originalScale;
    }

    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    public int GetBannerIndex()
    {
        return bannerIndex;
    }
}