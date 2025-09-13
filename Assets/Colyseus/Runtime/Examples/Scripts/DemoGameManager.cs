using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

///
/// Central manager for the Akash Colyseus Unity SDK Demo.
/// Coordinates game initialization, scene management, and demo flow.
/// 
public class DemoGameManager : MonoBehaviour
{
    [Header("Demo Configuration")]
    [SerializeField] private bool autoStartDemo = true;
    [SerializeField] private float autoStartDelay = 2f;

    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AkashLogoDisplay akashLogo;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Enhanced Demo Systems")]
    [SerializeField] private WalkwaySystem walkwaySystem;
    [SerializeField] private EnhancedPlayerMovement enhancedPlayerMovement;
    [SerializeField] private AkashLogoBannerSystem bannerSystem;
    [SerializeField] private CameraBehaviorSystem cameraSystem;
    [SerializeField] private VisualEffectsManager visualEffects;

    [Header("Demo Mode Selection")]
    [SerializeField] private bool useEnhancedMode = true;
    [SerializeField] private bool enableWalkwayGeneration = true;
    [SerializeField] private bool enableBannerSystem = true;
    [SerializeField] private bool enableCameraFollowing = true;
    [SerializeField] private bool enableVisualEffects = true;

    [Header("Demo Settings")]
    [SerializeField] private Vector2 cameraPosition = Vector2.zero;
    [SerializeField] private float cameraSize = 10f;
    [SerializeField] private Color backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);

    [Header("UI References")]
    [SerializeField] private UIManager uiManager;

    // Static instance for easy access
    public static DemoGameManager Instance { get; private set; }

    // Game state
    private bool isInitialized = false;
    private bool isDemoRunning = false;
    private bool enhancedSystemsReady = false;

    // Enhanced demo state
    private int platformsGenerated = 0;
    private int bannersCreated = 0;
    private bool systemsIntegrated = false;

    // Events
    public System.Action OnDemoStarted;
    public System.Action OnDemoStopped;
    public System.Action<int> OnPlayerCountChanged;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeDemo();
    }

    private void Start()
    {
        if (autoStartDemo)
        {
            StartCoroutine(AutoStartDemoCoroutine());
        }
    }

    ///
    /// Initializes the demo environment and components
    ///
    private void InitializeDemo()
    {
        Debug.Log("Akash Demo: Initializing demo environment...");

        SetupCamera();
        SetupReferences();
        SetupEnhancedSystems();
        SetupBackground();

        isInitialized = true;
        Debug.Log($"Akash Demo: Demo environment initialized successfully! Enhanced Mode: {useEnhancedMode}");
    }

    ///
    /// Sets up the main camera for optimal demo viewing
    ///
    private void SetupCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10f);
            mainCamera.orthographicSize = cameraSize;
            mainCamera.backgroundColor = backgroundColor;
        }
    }

    ///
    /// Sets up references to demo components
    ///
    private void SetupReferences()
    {
        // Find or create Akash logo (for backwards compatibility)
        if (akashLogo == null)
        {
            akashLogo = FindObjectOfType<AkashLogoDisplay>();
            if (akashLogo == null && !useEnhancedMode)
            {
                GameObject logoObject = new GameObject("AkashLogo");
                akashLogo = logoObject.AddComponent<AkashLogoDisplay>();
            }
        }

        // Setup player movement based on mode
        if (useEnhancedMode)
        {
            // Find or create enhanced player movement
            if (enhancedPlayerMovement == null)
            {
                enhancedPlayerMovement = FindObjectOfType<EnhancedPlayerMovement>();
                if (enhancedPlayerMovement == null)
                {
                    GameObject playerObject = new GameObject("EnhancedPlayer");
                    enhancedPlayerMovement = playerObject.AddComponent<EnhancedPlayerMovement>();
                }
            }

            // Disable old player movement if it exists
            if (playerMovement != null)
            {
                playerMovement.gameObject.SetActive(false);
            }
        }
        else
        {
            // Find or create regular player movement
            if (playerMovement == null)
            {
                playerMovement = FindObjectOfType<PlayerMovement>();
                if (playerMovement == null)
                {
                    GameObject playerObject = new GameObject("Player");
                    playerMovement = playerObject.AddComponent<PlayerMovement>();
                }
            }
        }

        // Find or create UI manager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }

    ///
    /// Sets up the background styling
    ///
    private void SetupBackground()
    {
        // Create a background plane for better visual contrast
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "Background";
        background.transform.position = new Vector3(0, 0, 5f); // Behind other objects
        background.transform.localScale = new Vector3(50f, 50f, 1f);

        Renderer bgRenderer = background.GetComponent<Renderer>();
        if (bgRenderer != null)
        {
            bgRenderer.material = new Material(Shader.Find("Sprites/Default"));
            bgRenderer.material.color = backgroundColor;
        }

        // Remove collider as it's not needed
        Collider bgCollider = background.GetComponent<Collider>();
        if (bgCollider != null)
        {
            Destroy(bgCollider);
        }
    }

    ///
    /// Starts the demo presentation
    ///
    public void StartDemo()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("Akash Demo: Cannot start demo - not initialized!");
            return;
        }

        if (isDemoRunning)
        {
            Debug.Log("Akash Demo: Demo is already running!");
            return;
        }

        Debug.Log("Akash Demo: Starting demo presentation...");
        isDemoRunning = true;

        // Enable demo components
        if (akashLogo != null)
        {
            akashLogo.gameObject.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.gameObject.SetActive(true);
        }

        // Initialize enhanced systems if enabled
        if (useEnhancedMode)
        {
            InitializeEnhancedSystems();
        }

        // Show instructional message
        ShowInstructions();

        OnDemoStarted?.Invoke();
    }

    ///
    /// Stops the demo presentation
    ///
    public void StopDemo()
    {
        if (!isDemoRunning)
        {
            return;
        }

        Debug.Log("Akash Demo: Stopping demo presentation...");
        isDemoRunning = false;

        OnDemoStopped?.Invoke();
    }

    ///
    /// Restarts the demo
    ///
    public void RestartDemo()
    {
        StopDemo();
        StartCoroutine(RestartDemoCoroutine());
    }

    ///
    /// Shows demo instructions to the user
    ///
    private void ShowInstructions()
    {
        string instructions;

        if (useEnhancedMode)
        {
            instructions =
                "Enhanced Akash 2D Walkway Demo\n" +
                "\n" +
                "Controls:\n" +
                "• Click on platforms to move\n" +
                "• Space to jump between platforms\n" +
                "• Walk past Akash banners for effects\n" +
                "• Multiple players synchronized in real-time\n" +
                "\n" +
                "Features:\n" +
                "• 2D Physics-based movement\n" +
                "• Dynamic camera following\n" +
                "• Interactive Akash branding\n" +
                "• Atmospheric visual effects\n" +
                "\n" +
                "Showcasing decentralized multiplayer gaming\n" +
                "on Akash Network!";
        }
        else
        {
            instructions =
                "Akash Colyseus Unity SDK Demo\n" +
                "\n" +
                "Controls:\n" +
                "• Click anywhere to move\n" +
                "• Shift + Click to move towards Akash logo\n" +
                "• Multiple players will appear as they join\n" +
                "\n" +
                "This demo showcases real-time multiplayer\n" +
                "synchronization using Colyseus on Akash Network!";
        }

        Debug.Log(instructions);

        if (uiManager != null)
        {
            uiManager.ShowMessage(instructions, 8f);
        }
    }

    ///
    /// Updates the player count display
    ///
    /// <param name="count">Current number of connected players</param>
    public void UpdatePlayerCount(int count)
    {
        OnPlayerCountChanged?.Invoke(count);

        if (uiManager != null)
        {
            uiManager.UpdatePlayerCount(count);
        }
    }

    ///
    /// Updates the connection status display
    ///
    /// <param name="status">Current connection status</param>
    public void UpdateConnectionStatus(string status)
    {
        if (uiManager != null)
        {
            uiManager.UpdateConnectionStatus(status);
        }
    }

    ///
    /// Handles error messages
    ///
    /// <param name="error">Error message to display</param>
    public void ShowError(string error)
    {
        Debug.LogError($"Akash Demo Error: {error}");

        if (uiManager != null)
        {
            uiManager.ShowError(error);
        }
    }

    ///
    /// Returns to the main menu (if available)
    ///
    public void ReturnToMenu()
    {
        // Check if Menu scene exists
        try
        {
            SceneManager.LoadScene("Menu");
        }
        catch
        {
            Debug.Log("Akash Demo: No Menu scene found, restarting current scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    ///
    /// Exits the application
    ///
    public void ExitApplication()
    {
        Debug.Log("Akash Demo: Exiting application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    ///
    /// Coroutine for auto-starting the demo
    ///
    private IEnumerator AutoStartDemoCoroutine()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartDemo();
    }

    ///
    /// Coroutine for restarting the demo with a delay
    ///
    private IEnumerator RestartDemoCoroutine()
    {
        yield return new WaitForSeconds(1f);
        StartDemo();
    }

    ///
    /// Gets the Akash logo position for other components
    ///
    /// <returns>World position of the Akash logo</returns>
    public Vector2 GetAkashLogoPosition()
    {
        if (useEnhancedMode && bannerSystem != null)
        {
            Vector2[] bannerPositions = bannerSystem.GetBannerPositions();
            if (bannerPositions.Length > 0)
            {
                return bannerPositions[0]; // Return first banner position
            }
        }

        if (akashLogo != null)
        {
            return akashLogo.GetLogoPosition();
        }
        return Vector2.zero;
    }

    ///
    /// Gets the current demo state
    ///
    /// <returns>True if demo is currently running</returns>
    public bool IsDemoRunning()
    {
        return isDemoRunning;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Debug methods for testing
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 250, 150));
        GUILayout.Label($"Akash Demo Controls (Mode: {(useEnhancedMode ? "Enhanced" : "Classic")})");

        if (GUILayout.Button(isDemoRunning ? "Stop Demo" : "Start Demo"))
        {
            if (isDemoRunning)
                StopDemo();
            else
                StartDemo();
        }

        if (GUILayout.Button("Restart Demo"))
        {
            RestartDemo();
        }

        if (useEnhancedMode)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Platforms: {platformsGenerated}");
            GUILayout.Label($"Banners: {bannersCreated}");
            GUILayout.Label($"Systems: {(systemsIntegrated ? "Ready" : "Loading")}");

            if (GUILayout.Button("Toggle Camera Follow"))
            {
                if (cameraSystem != null)
                {
                    cameraSystem.enabled = !cameraSystem.enabled;
                }
            }

            if (GUILayout.Button("Special Effect"))
            {
                TriggerSpecialEffect();
            }
        }

        GUILayout.EndArea();
    }

    ///
    /// Sets up enhanced demo systems
    ///
    private void SetupEnhancedSystems()
    {
        if (!useEnhancedMode) return;

        // Find or create walkway system
        if (enableWalkwayGeneration && walkwaySystem == null)
        {
            walkwaySystem = FindObjectOfType<WalkwaySystem>();
            if (walkwaySystem == null)
            {
                GameObject walkwayObject = new GameObject("WalkwaySystem");
                walkwaySystem = walkwayObject.AddComponent<WalkwaySystem>();
            }
        }

        // Find or create banner system
        if (enableBannerSystem && bannerSystem == null)
        {
            bannerSystem = FindObjectOfType<AkashLogoBannerSystem>();
            if (bannerSystem == null)
            {
                GameObject bannerObject = new GameObject("AkashLogoBannerSystem");
                bannerSystem = bannerObject.AddComponent<AkashLogoBannerSystem>();
            }
        }

        // Find or create camera system
        if (enableCameraFollowing && cameraSystem == null)
        {
            cameraSystem = FindObjectOfType<CameraBehaviorSystem>();
            if (cameraSystem == null && mainCamera != null)
            {
                cameraSystem = mainCamera.gameObject.AddComponent<CameraBehaviorSystem>();
            }
        }

        // Find or create visual effects
        if (enableVisualEffects && visualEffects == null)
        {
            visualEffects = FindObjectOfType<VisualEffectsManager>();
            if (visualEffects == null)
            {
                GameObject vfxObject = new GameObject("VisualEffectsManager");
                visualEffects = vfxObject.AddComponent<VisualEffectsManager>();
            }
        }

        Debug.Log("Akash Demo: Enhanced systems setup completed");
    }

    ///
    /// Initializes enhanced systems after basic setup
    ///
    private void InitializeEnhancedSystems()
    {
        if (!useEnhancedMode || enhancedSystemsReady) return;

        Debug.Log("Akash Demo: Initializing enhanced systems...");

        // Subscribe to system events
        if (walkwaySystem != null)
        {
            walkwaySystem.OnWalkwayGenerated += OnWalkwayGenerated;
            walkwaySystem.OnPlatformAdded += OnPlatformAdded;
        }

        if (bannerSystem != null)
        {
            bannerSystem.OnBannersCreated += OnBannersCreated;
            bannerSystem.OnPlayerNearBanner += OnPlayerNearBanner;
        }

        if (cameraSystem != null)
        {
            cameraSystem.OnTargetChanged += OnCameraTargetChanged;
            cameraSystem.OnZoomChanged += OnCameraZoomChanged;
        }

        if (enhancedPlayerMovement != null)
        {
            enhancedPlayerMovement.OnPlayerJumped += OnPlayerJumped;
            enhancedPlayerMovement.OnGroundedStateChanged += OnPlayerGroundedChanged;
        }

        enhancedSystemsReady = true;
        systemsIntegrated = true;

        Debug.Log("Akash Demo: Enhanced systems initialized and integrated!");
    }

    ///
    /// Called when walkway is generated
    ///
    private void OnWalkwayGenerated(Vector2[] platformPositions)
    {
        platformsGenerated = platformPositions.Length;
        Debug.Log($"Akash Demo: Walkway generated with {platformsGenerated} platforms");

        if (uiManager != null)
        {
            uiManager.ShowMessage($"Walkway ready! {platformsGenerated} platforms generated.", 3f);
        }
    }

    ///
    /// Called when a platform is added
    ///
    private void OnPlatformAdded(Vector2 position)
    {
        // Optional: Visual feedback for platform creation
        if (visualEffects != null)
        {
            visualEffects.TriggerSpecialEffect(position, ParticleEffectType.Sparkle);
        }
    }

    ///
    /// Called when banners are created
    ///
    private void OnBannersCreated(int count)
    {
        bannersCreated = count;
        Debug.Log($"Akash Demo: {bannersCreated} Akash banners created");

        if (uiManager != null)
        {
            uiManager.ShowMessage($"Akash branding ready! {bannersCreated} banners placed.", 3f);
        }
    }

    ///
    /// Called when player approaches a banner
    ///
    private void OnPlayerNearBanner(AkashBanner banner, Transform player)
    {
        Debug.Log($"Akash Demo: Player near banner {banner.GetBannerIndex()}");

        // Trigger visual effects
        if (visualEffects != null)
        {
            visualEffects.TriggerSpecialEffect(banner.transform.position, ParticleEffectType.Sparkle);
        }

        if (uiManager != null)
        {
            uiManager.ShowMessage("Akash Network - Decentralized Cloud Computing!", 2f);
        }
    }

    ///
    /// Called when camera target changes
    ///
    private void OnCameraTargetChanged(Transform newTarget)
    {
        Debug.Log($"Akash Demo: Camera now following {(newTarget != null ? newTarget.name : "none")}");
    }

    ///
    /// Called when camera zoom changes
    ///
    private void OnCameraZoomChanged(float newZoom)
    {
        // Optional: Update UI or other systems based on zoom
    }

    ///
    /// Called when player jumps
    ///
    private void OnPlayerJumped()
    {
        Debug.Log("Akash Demo: Player jumped!");

        // Optional: Add jump effects
        if (visualEffects != null && enhancedPlayerMovement != null)
        {
            visualEffects.TriggerSpecialEffect(enhancedPlayerMovement.transform.position, ParticleEffectType.Trail);
        }
    }

    ///
    /// Called when player grounded state changes
    ///
    private void OnPlayerGroundedChanged(bool grounded)
    {
        if (grounded)
        {
            Debug.Log("Akash Demo: Player landed");
        }
    }

    ///
    /// Triggers a special effect across all systems
    ///
    public void TriggerSpecialEffect()
    {
        Debug.Log("Akash Demo: Triggering special effect!");

        if (bannerSystem != null)
        {
            bannerSystem.TriggerGlobalEffect();
        }

        if (visualEffects != null && enhancedPlayerMovement != null)
        {
            visualEffects.TriggerSpecialEffect(enhancedPlayerMovement.transform.position, ParticleEffectType.Explosion);
        }

        if (cameraSystem != null)
        {
            cameraSystem.ShakeCamera(0.5f, 1f);
        }

        if (uiManager != null)
        {
            uiManager.ShowMessage("Akash Network - Powering the Future!", 3f);
        }
    }

    ///
    /// Gets the current demo mode
    ///
    public bool IsEnhancedMode()
    {
        return useEnhancedMode;
    }

    ///
    /// Gets enhanced systems status
    ///
    public bool AreEnhancedSystemsReady()
    {
        return enhancedSystemsReady;
    }

    ///
    /// Switches between enhanced and classic demo modes
    ///
    public void SwitchDemoMode(bool enhanced)
    {
        if (useEnhancedMode == enhanced) return;

        useEnhancedMode = enhanced;
        Debug.Log($"Akash Demo: Switching to {(enhanced ? "Enhanced" : "Classic")} mode");

        // Restart demo with new mode
        RestartDemo();
    }
}