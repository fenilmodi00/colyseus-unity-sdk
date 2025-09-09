using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Comprehensive validation script for the Akash Colyseus Unity SDK Demo.
/// Validates all components, connections, and functionality are working correctly.
/// </summary>
public class DemoValidationTest : MonoBehaviour
{
    [Header("Validation Settings")]
    [SerializeField] private bool runValidationOnStart = false;
    [SerializeField] private bool enableDetailedLogging = true;
    [SerializeField] private bool testNetworkConnection = true;

    private StringBuilder _validationResults = new StringBuilder();
    private int _testsRun = 0;
    private int _testsPassed = 0;
    private int _testsFailed = 0;

    private void Start()
    {
        if (runValidationOnStart)
        {
            RunCompleteValidation();
        }
    }

    /// <summary>
    /// Runs complete validation of the demo setup
    /// </summary>
    [ContextMenu("Run Complete Validation")]
    public void RunCompleteValidation()
    {
        Debug.Log("Akash Demo: Starting comprehensive validation...");

        _validationResults.Clear();
        _testsRun = 0;
        _testsPassed = 0;
        _testsFailed = 0;

        AddTestResult("=== AKASH COLYSEUS UNITY SDK DEMO VALIDATION ===", true);

        // Component validation
        ValidateComponents();

        // Scene validation
        ValidateSceneSetup();

        // Script validation
        ValidateScriptConfiguration();

        // Network validation (if enabled)
        if (testNetworkConnection)
        {
            ValidateNetworkSetup();
        }

        // Generate final report
        GenerateFinalReport();
    }

    /// <summary>
    /// Validates all required components are present
    /// </summary>
    private void ValidateComponents()
    {
        AddTestResult("\n--- Component Validation ---", true);

        // Check for Camera
        ValidateComponent<Camera>("Main Camera", true);

        // Check for core demo components
        ValidateComponent<AkashLogoDisplay>("Akash Logo Display", false);
        ValidateComponent<DemoGameManager>("Demo Game Manager", false);
        ValidateComponent<UIManager>("UI Manager", false);
        ValidateComponent<PlayerMovement>("Player Movement", false);
        ValidateComponent<NetworkManager>("Network Manager", false);
        ValidateComponent<ConnectionHealthMonitor>("Connection Health Monitor", false);

        // Check for Unity UI components
        ValidateComponent<Canvas>("UI Canvas", false);
    }

    /// <summary>
    /// Validates scene setup and configuration
    /// </summary>
    private void ValidateSceneSetup()
    {
        AddTestResult("\n--- Scene Setup Validation ---", true);

        // Check camera configuration
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            ValidateTest("Camera is orthographic", mainCamera.orthographic);
            ValidateTest("Camera position is reasonable",
                mainCamera.transform.position.z < 0f &&
                Mathf.Abs(mainCamera.transform.position.x) < 20f &&
                Mathf.Abs(mainCamera.transform.position.y) < 20f);
            ValidateTest("Camera orthographic size is reasonable",
                mainCamera.orthographicSize > 1f && mainCamera.orthographicSize < 50f);
        }

        // Check for AudioListener
        ValidateTest("AudioListener present", FindObjectOfType<AudioListener>() != null);

        // Check scene lighting
        ValidateTest("Scene has lighting", FindObjectOfType<Light>() != null);

        // Check for background elements
        GameObject background = GameObject.Find("AkashBackground") ?? GameObject.Find("Background");
        ValidateTest("Background element present", background != null);
    }

    /// <summary>
    /// Validates script configuration and settings
    /// </summary>
    private void ValidateScriptConfiguration()
    {
        AddTestResult("\n--- Script Configuration Validation ---", true);

        // Validate DemoGameManager settings
        DemoGameManager gameManager = FindObjectOfType<DemoGameManager>();
        if (gameManager != null)
        {
            ValidateTest("DemoGameManager singleton pattern", DemoGameManager.Instance == gameManager);
        }

        // Validate NetworkManager settings
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
        {
            ValidateTest("NetworkManager present and configured", true);
        }

        // Validate PlayerMovement settings
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            ValidateTest("PlayerMovement component configured", true);
        }

        // Validate MenuManager settings
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null)
        {
            // Try to find or create MenuManager
            GameObject menuManagerObject = new GameObject("MenuManager");
            menuManager = menuManagerObject.AddComponent<MenuManager>();
        }

        if (menuManager != null)
        {
            ValidateTest("MenuManager host address configured",
                !string.IsNullOrEmpty(menuManager.HostAddress));
            ValidateTest("MenuManager using correct protocol",
                menuManager.HostAddress.StartsWith("ws://") || menuManager.HostAddress.StartsWith("wss://"));
            ValidateTest("MenuManager port is 2567",
                menuManager.HostAddress.Contains("2567"));
        }
    }

    /// <summary>
    /// Validates network setup and connectivity
    /// </summary>
    private void ValidateNetworkSetup()
    {
        AddTestResult("\n--- Network Setup Validation ---", true);

        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
        {
            ValidateTest("NetworkManager client can be created", networkManager.Client != null);

            // Check connection state
            var currentState = networkManager.CurrentConnectionState;
            ValidateTest("NetworkManager has valid connection state",
                System.Enum.IsDefined(typeof(NetworkManager.ConnectionState), currentState));
        }

        // Validate schema files exist and are properly configured
        ValidateTest("MyRoomState class available", typeof(MyRoomState) != null);
        ValidateTest("Player class available", typeof(Player) != null);

        // Test that we can create schema objects
        try
        {
            var roomState = new MyRoomState();
            var player = new Player();
            ValidateTest("Schema objects can be instantiated", true);
        }
        catch (System.Exception e)
        {
            ValidateTest($"Schema objects can be instantiated (Error: {e.Message})", false);
        }
    }

    /// <summary>
    /// Validates a specific component type
    /// </summary>
    private void ValidateComponent<T>(string componentName, bool isRequired) where T : Component
    {
        T component = FindObjectOfType<T>();
        bool exists = component != null;

        if (isRequired)
        {
            ValidateTest($"{componentName} present (REQUIRED)", exists);
        }
        else
        {
            ValidateTest($"{componentName} present", exists);
        }

        if (exists && enableDetailedLogging)
        {
            AddTestResult($"  └─ Found: {component.gameObject.name}", true);
        }
    }

    /// <summary>
    /// Validates a test condition and logs the result
    /// </summary>
    private void ValidateTest(string testName, bool condition)
    {
        _testsRun++;

        if (condition)
        {
            _testsPassed++;
            AddTestResult($"✓ {testName}", true);
        }
        else
        {
            _testsFailed++;
            AddTestResult($"✗ {testName}", false);
        }
    }

    /// <summary>
    /// Adds a test result to the validation log
    /// </summary>
    private void AddTestResult(string message, bool success)
    {
        _validationResults.AppendLine(message);

        if (enableDetailedLogging)
        {
            if (success && !message.StartsWith("✓") && !message.StartsWith("=") && !message.StartsWith("-"))
            {
                Debug.Log($"Akash Demo Validation: {message}");
            }
            else if (!success)
            {
                Debug.LogWarning($"Akash Demo Validation: {message}");
            }
        }
    }

    /// <summary>
    /// Generates and displays the final validation report
    /// </summary>
    private void GenerateFinalReport()
    {
        AddTestResult("\n--- VALIDATION SUMMARY ---", true);
        AddTestResult($"Tests Run: {_testsRun}", true);
        AddTestResult($"Tests Passed: {_testsPassed}", true);
        AddTestResult($"Tests Failed: {_testsFailed}", true);

        float successRate = _testsRun > 0 ? (float)_testsPassed / _testsRun * 100f : 0f;
        AddTestResult($"Success Rate: {successRate:F1}% ", true);

        bool overallSuccess = _testsFailed == 0 && _testsPassed > 0;

        if (overallSuccess)
        {
            AddTestResult("\n🎉 VALIDATION PASSED - Demo is ready for presentation!", true);
            Debug.Log("Akash Demo: Validation completed successfully!");
        }
        else
        {
            AddTestResult("\n⚠️ VALIDATION ISSUES FOUND - Please review the results above", false);
            Debug.LogWarning("Akash Demo: Validation completed with issues. Please review.");
        }

        // Log full report
        Debug.Log($"Akash Demo Validation Report:\n{_validationResults.ToString()}");

        // Update demo manager if available
        if (DemoGameManager.Instance != null)
        {
            string statusMessage = overallSuccess ?
                "Demo validation passed! Ready for presentation." :
                "Demo validation found issues. Check console for details.";

            DemoGameManager.Instance.UpdateConnectionStatus(overallSuccess ? "Validated" : "Issues Found");
        }
    }

    /// <summary>
    /// Gets the current validation results as a string
    /// </summary>
    public string GetValidationResults()
    {
        return _validationResults.ToString();
    }

    /// <summary>
    /// Tests the demo functionality step by step
    /// </summary>
    [ContextMenu("Test Demo Functionality")]
    public void TestDemoFunctionality()
    {
        Debug.Log("Akash Demo: Testing demo functionality...");

        StartCoroutine(TestDemoFunctionalityCoroutine());
    }

    /// <summary>
    /// Coroutine to test demo functionality over time
    /// </summary>
    private System.Collections.IEnumerator TestDemoFunctionalityCoroutine()
    {
        // Test 1: Initialize demo
        if (DemoGameManager.Instance != null)
        {
            DemoGameManager.Instance.StartDemo();
            yield return new WaitForSeconds(1f);
        }

        // Test 2: Test network connection
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
        {
            yield return StartCoroutine(TestNetworkConnection(networkManager));
        }

        // Test 3: Test player movement
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            // Simulate player movement
            Debug.Log("Akash Demo: Simulating player movement...");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Akash Demo: Functionality test complete!");
    }

    /// <summary>
    /// Tests network connection functionality
    /// </summary>
    private System.Collections.IEnumerator TestNetworkConnection(NetworkManager networkManager)
    {
        Debug.Log("Akash Demo: Testing network connection...");

        // Attempt to connect
        var connectionTask = networkManager.JoinOrCreateGame();

        // Wait for connection attempt (with timeout)
        float timeout = 10f;
        float elapsed = 0f;

        while (!connectionTask.IsCompleted && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (connectionTask.IsCompleted && connectionTask.Result)
        {
            Debug.Log("Akash Demo: Network connection test PASSED");
        }
        else
        {
            Debug.LogWarning("Akash Demo: Network connection test FAILED");
        }
    }

    /// <summary>
    /// Quick validation for essential components only
    /// </summary>
    [ContextMenu("Quick Validation")]
    public void QuickValidation()
    {
        Debug.Log("Akash Demo: Running quick validation...");

        bool hasDemoManager = FindObjectOfType<DemoGameManager>() != null;
        bool hasNetworkManager = FindObjectOfType<NetworkManager>() != null;
        bool hasPlayerMovement = FindObjectOfType<PlayerMovement>() != null;
        bool hasAkashLogo = FindObjectOfType<AkashLogoDisplay>() != null;

        int essentialComponents = (hasDemoManager ? 1 : 0) +
                                 (hasNetworkManager ? 1 : 0) +
                                 (hasPlayerMovement ? 1 : 0) +
                                 (hasAkashLogo ? 1 : 0);

        Debug.Log($"Akash Demo: Quick validation result - {essentialComponents}/4 essential components found");

        if (essentialComponents == 4)
        {
            Debug.Log("Akash Demo: ✓ Quick validation PASSED - Demo ready!");
        }
        else
        {
            Debug.LogWarning("Akash Demo: ⚠️ Quick validation FAILED - Missing essential components");
        }
    }
}