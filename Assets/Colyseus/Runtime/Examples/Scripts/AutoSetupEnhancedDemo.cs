using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Automatic setup script for the Enhanced Akash Demo
/// This script helps beginners set up the demo scene quickly
/// </summary>
public class AutoSetupEnhancedDemo : MonoBehaviour
{
    [Header("Setup Options")]
    [SerializeField] private bool setupOnStart = true;
    [SerializeField] private bool showInstructions = true;

    [Header("Demo Configuration")]
    [SerializeField] private bool useEnhancedMode = true;
    [SerializeField] private Vector2 cameraPosition = Vector2.zero;
    [SerializeField] private float cameraSize = 10f;

    private void Start()
    {
        if (setupOnStart)
        {
            SetupEnhancedDemo();
        }

        if (showInstructions)
        {
            ShowBeginnerInstructions();
        }
    }

    [ContextMenu("Setup Enhanced Demo")]
    public void SetupEnhancedDemo()
    {
        Debug.Log("Setting up Enhanced Akash Demo...");

        // 1. Setup Camera
        SetupMainCamera();

        // 2. Setup Demo Game Manager
        SetupDemoGameManager();

        // 3. Setup Network Manager
        SetupNetworkManager();

        // 4. Setup UI Manager
        SetupUIManager();

        // 5. Setup Physics
        SetupPhysics2D();

        Debug.Log("Enhanced Akash Demo setup complete! Press Play to start the demo.");
    }

    private void SetupMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }

        // Configure camera for 2D
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = cameraSize;
        mainCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10f);

        // Add camera behavior system
        CameraBehaviorSystem cameraSystem = mainCamera.GetComponent<CameraBehaviorSystem>();
        if (cameraSystem == null)
        {
            cameraSystem = mainCamera.gameObject.AddComponent<CameraBehaviorSystem>();
        }

        Debug.Log("✓ Main Camera configured for Enhanced Demo");
    }

    private void SetupDemoGameManager()
    {
        DemoGameManager existingManager = FindObjectOfType<DemoGameManager>();
        if (existingManager == null)
        {
            GameObject managerObj = new GameObject("DemoGameManager");
            DemoGameManager manager = managerObj.AddComponent<DemoGameManager>();

            // Configure for enhanced mode
            // Note: These would need to be set via reflection or made public
            Debug.Log("✓ DemoGameManager created (configure Enhanced Mode in inspector)");
        }
        else
        {
            Debug.Log("✓ DemoGameManager already exists");
        }
    }

    private void SetupNetworkManager()
    {
        NetworkManager existingNetwork = FindObjectOfType<NetworkManager>();
        if (existingNetwork == null)
        {
            GameObject networkObj = new GameObject("NetworkManager");
            NetworkManager network = networkObj.AddComponent<NetworkManager>();
            Debug.Log("✓ NetworkManager created");
        }
        else
        {
            Debug.Log("✓ NetworkManager already exists");
        }
    }

    private void SetupUIManager()
    {
        UIManager existingUI = FindObjectOfType<UIManager>();
        if (existingUI == null)
        {
            GameObject uiObj = new GameObject("UIManager");
            UIManager ui = uiObj.AddComponent<UIManager>();
            Debug.Log("✓ UIManager created");
        }
        else
        {
            Debug.Log("✓ UIManager already exists");
        }
    }

    private void SetupPhysics2D()
    {
        // Configure Physics2D settings for the demo
        Physics2D.gravity = new Vector2(0, -9.81f);

        // Set up layers (this requires manual setup in Project Settings)
        Debug.Log("✓ Physics2D gravity configured");
        Debug.Log("Note: Please set up Physics layers manually:");
        Debug.Log("  - Layer 8: Platforms");
        Debug.Log("  - Layer 0: Default (Players)");
    }

    private void ShowBeginnerInstructions()
    {
        string instructions =
            "=== ENHANCED AKASH DEMO - BEGINNER GUIDE ===\n\n" +
            "SETUP STEPS:\n" +
            "1. This script has automatically configured basic components\n" +
            "2. In the Inspector, configure DemoGameManager:\n" +
            "   - Set 'Use Enhanced Mode' to TRUE\n" +
            "   - Enable walkway, banners, camera, and effects\n\n" +
            "MANUAL SETUP REQUIRED:\n" +
            "3. Set up Physics Layers:\n" +
            "   - Edit → Project Settings → Tags & Layers\n" +
            "   - Set Layer 8 to 'Platforms'\n\n" +
            "RUNNING THE DEMO:\n" +
            "4. Press Play button in Unity\n" +
            "5. The demo will automatically start\n" +
            "6. Click on platforms to move\n" +
            "7. Walk near Akash banners for effects\n\n" +
            "MULTIPLAYER TESTING:\n" +
            "8. Start the Colyseus server first (see Server folder)\n" +
            "9. Multiple Unity instances can connect\n\n" +
            "Need help? Check the Console for messages!";

        Debug.Log(instructions);
    }

    [ContextMenu("Show Instructions Again")]
    public void ShowInstructionsAgain()
    {
        ShowBeginnerInstructions();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Enhanced Akash Demo - Quick Start");

        if (GUILayout.Button("Setup Demo Components"))
        {
            SetupEnhancedDemo();
        }

        if (GUILayout.Button("Show Instructions"))
        {
            ShowBeginnerInstructions();
        }

        GUILayout.Space(10);
        GUILayout.Label("Status:");

        bool hasCamera = Camera.main != null;
        bool hasDemoManager = FindObjectOfType<DemoGameManager>() != null;
        bool hasNetwork = FindObjectOfType<NetworkManager>() != null;

        GUILayout.Label($"Camera: {(hasCamera ? "✓" : "✗")}");
        GUILayout.Label($"Demo Manager: {(hasDemoManager ? "✓" : "✗")}");
        GUILayout.Label($"Network: {(hasNetwork ? "✓" : "✗")}");

        GUILayout.EndArea();
    }
}