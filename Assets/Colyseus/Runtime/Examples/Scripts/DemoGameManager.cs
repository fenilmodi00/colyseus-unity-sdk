using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Central manager for the Akash Colyseus Unity SDK Demo.
/// Coordinates game initialization, scene management, and demo flow.
/// </summary>
public class DemoGameManager : MonoBehaviour
{
    [Header("Demo Configuration")]
    [SerializeField] private bool autoStartDemo = true;
    [SerializeField] private float autoStartDelay = 2f;

    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AkashLogoDisplay akashLogo;
    [SerializeField] private PlayerMovement playerMovement;

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

    /// <summary>
    /// Initializes the demo environment and components
    /// </summary>
    private void InitializeDemo()
    {
        Debug.Log("Akash Demo: Initializing demo environment...");

        SetupCamera();
        SetupReferences();
        SetupBackground();

        isInitialized = true;
        Debug.Log("Akash Demo: Demo environment initialized successfully!");
    }

    /// <summary>
    /// Sets up the main camera for optimal demo viewing
    /// </summary>
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

    /// <summary>
    /// Sets up references to demo components
    /// </summary>
    private void SetupReferences()
    {
        // Find or create Akash logo
        if (akashLogo == null)
        {
            akashLogo = FindObjectOfType<AkashLogoDisplay>();
            if (akashLogo == null)
            {
                GameObject logoObject = new GameObject("AkashLogo");
                akashLogo = logoObject.AddComponent<AkashLogoDisplay>();
            }
        }

        // Find or create player movement
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null)
            {
                GameObject playerObject = new GameObject("Player");
                playerMovement = playerObject.AddComponent<PlayerMovement>();
            }
        }

        // Find or create UI manager
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }

    /// <summary>
    /// Sets up the background styling
    /// </summary>
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

    /// <summary>
    /// Starts the demo presentation
    /// </summary>
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

        // Show instructional message
        ShowInstructions();

        OnDemoStarted?.Invoke();
    }

    /// <summary>
    /// Stops the demo presentation
    /// </summary>
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

    /// <summary>
    /// Restarts the demo
    /// </summary>
    public void RestartDemo()
    {
        StopDemo();
        StartCoroutine(RestartDemoCoroutine());
    }

    /// <summary>
    /// Shows demo instructions to the user
    /// </summary>
    private void ShowInstructions()
    {
        string instructions =
            "Akash Colyseus Unity SDK Demo\n" +
            "\n" +
            "Controls:\n" +
            "• Click anywhere to move\n" +
            "• Shift + Click to move towards Akash logo\n" +
            "• Multiple players will appear as they join\n" +
            "\n" +
            "This demo showcases real-time multiplayer\n" +
            "synchronization using Colyseus on Akash Network!";

        Debug.Log(instructions);

        if (uiManager != null)
        {
            uiManager.ShowMessage(instructions, 5f);
        }
    }

    /// <summary>
    /// Updates the player count display
    /// </summary>
    /// <param name="count">Current number of connected players</param>
    public void UpdatePlayerCount(int count)
    {
        OnPlayerCountChanged?.Invoke(count);

        if (uiManager != null)
        {
            uiManager.UpdatePlayerCount(count);
        }
    }

    /// <summary>
    /// Updates the connection status display
    /// </summary>
    /// <param name="status">Current connection status</param>
    public void UpdateConnectionStatus(string status)
    {
        if (uiManager != null)
        {
            uiManager.UpdateConnectionStatus(status);
        }
    }

    /// <summary>
    /// Handles error messages
    /// </summary>
    /// <param name="error">Error message to display</param>
    public void ShowError(string error)
    {
        Debug.LogError($"Akash Demo Error: {error}");

        if (uiManager != null)
        {
            uiManager.ShowError(error);
        }
    }

    /// <summary>
    /// Returns to the main menu (if available)
    /// </summary>
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

    /// <summary>
    /// Exits the application
    /// </summary>
    public void ExitApplication()
    {
        Debug.Log("Akash Demo: Exiting application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Coroutine for auto-starting the demo
    /// </summary>
    private IEnumerator AutoStartDemoCoroutine()
    {
        yield return new WaitForSeconds(autoStartDelay);
        StartDemo();
    }

    /// <summary>
    /// Coroutine for restarting the demo with a delay
    /// </summary>
    private IEnumerator RestartDemoCoroutine()
    {
        yield return new WaitForSeconds(1f);
        StartDemo();
    }

    /// <summary>
    /// Gets the Akash logo position for other components
    /// </summary>
    /// <returns>World position of the Akash logo</returns>
    public Vector2 GetAkashLogoPosition()
    {
        if (akashLogo != null)
        {
            return akashLogo.GetLogoPosition();
        }
        return Vector2.zero;
    }

    /// <summary>
    /// Gets the current demo state
    /// </summary>
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

        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.Label("Akash Demo Controls");

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

        GUILayout.EndArea();
    }
}