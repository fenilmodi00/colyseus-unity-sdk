using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene setup utility for creating the Akash Colyseus Unity SDK Demo scene.
/// This script helps set up the complete demo environment with proper styling and components.
/// </summary>

public class AkashDemoSceneSetup : MonoBehaviour
{
    [Header("Scene Setup Configuration")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool createPlayerPrefab = true;
    [SerializeField] private bool setupCamera = true;
    [SerializeField] private bool setupLighting = true;

    [Header("Visual Styling")]
    [SerializeField] private Color backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
    [SerializeField] private Color akashBrandColor = new Color(1f, 0.2f, 0.2f, 1f);
    [SerializeField] private Vector2 sceneSize = new Vector2(20f, 15f);

    [Header("Akash Branding")]
    [SerializeField] private bool showAkashText = true;
    [SerializeField] private string akashTitle = "AKASH NETWORK";
    [SerializeField] private string akashSubtitle = "Decentralized Cloud Computing";

    // References to created objects
    private Camera sceneCamera;
    private AkashLogoDisplay logoDisplay;
    private DemoGameManager gameManager;
    private UIManager uiManager;
    private PlayerMovement playerMovement;

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCompleteScene();
        }
    }


    /// <summary>
    /// Sets up the complete demo scene with all components
    /// </summary>

    [ContextMenu("Setup Complete Scene")]
    public void SetupCompleteScene()
    {
        Debug.Log("Akash Demo: Setting up complete demo scene...");

        // Setup in order of dependency
        SetupSceneCamera();
        SetupBackground();
        SetupLighting();
        SetupAkashLogo();
        SetupDemoGameManager();
        SetupUIManager();
        SetupPlayerMovement();
        SetupAkashBranding();

        Debug.Log("Akash Demo: Scene setup complete! Ready for demonstration.");
    }


    private void SetupSceneCamera()
    {
        if (!setupCamera) return;

        sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            sceneCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        // Configure camera for 2D demo
        sceneCamera.transform.position = new Vector3(0, 0, -10);
        sceneCamera.orthographic = true;
        sceneCamera.orthographicSize = 8f;
        sceneCamera.backgroundColor = backgroundColor;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;

        // Add audio listener if not present
        if (sceneCamera.GetComponent<AudioListener>() == null)
        {
            sceneCamera.gameObject.AddComponent<AudioListener>();
        }

        Debug.Log("Akash Demo: Camera setup complete");
    }


    private void SetupBackground()
    {
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "AkashBackground";
        background.transform.position = new Vector3(0, 0, 5f);
        background.transform.localScale = new Vector3(sceneSize.x * 2, sceneSize.y * 2, 1f);

        // Create gradient background material
        Material backgroundMaterial = new Material(Shader.Find("Sprites/Default"));

        // Create a gradient texture
        Texture2D gradientTexture = CreateGradientTexture();
        backgroundMaterial.mainTexture = gradientTexture;

        Renderer bgRenderer = background.GetComponent<Renderer>();
        bgRenderer.material = backgroundMaterial;
        bgRenderer.sortingOrder = -10; // Behind everything

        // Remove collider
        if (background.GetComponent<Collider>() != null)
        {
            DestroyImmediate(background.GetComponent<Collider>());
        }

        Debug.Log("Akash Demo: Background setup complete");
    }

    /// Creates a gradient texture for the background

    private Texture2D CreateGradientTexture()
    {
        int width = 256;
        int height = 256;
        Texture2D texture = new Texture2D(width, height);

        Color topColor = backgroundColor;
        Color bottomColor = Color.Lerp(backgroundColor, akashBrandColor, 0.1f);

        for (int y = 0; y < height; y++)
        {
            Color color = Color.Lerp(bottomColor, topColor, (float)y / height);
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }


    /// Sets up basic lighting for the scene

    private void SetupLighting()
    {
        if (!setupLighting) return;

        // Find or create directional light
        Light sceneLight = FindObjectOfType<Light>();
        if (sceneLight == null)
        {
            GameObject lightObject = new GameObject("Directional Light");
            sceneLight = lightObject.AddComponent<Light>();
        }

        sceneLight.type = LightType.Directional;
        sceneLight.color = Color.white;
        sceneLight.intensity = 1f;
        sceneLight.shadows = LightShadows.None; // No shadows needed for 2D demo
        sceneLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f, 1f);

        Debug.Log("Akash Demo: Lighting setup complete");
    }


    /// Sets up the Akash logo display

    private void SetupAkashLogo()
    {
        GameObject logoObject = new GameObject("AkashLogo");
        logoDisplay = logoObject.AddComponent<AkashLogoDisplay>();

        // Position logo at center
        logoObject.transform.position = Vector3.zero;

        Debug.Log("Akash Demo: Akash logo setup complete");
    }


    /// Sets up the demo game manager

    private void SetupDemoGameManager()
    {
        GameObject managerObject = new GameObject("DemoGameManager");
        gameManager = managerObject.AddComponent<DemoGameManager>();

        Debug.Log("Akash Demo: Game manager setup complete");
    }


    /// Sets up the UI manager

    private void SetupUIManager()
    {
        GameObject uiObject = new GameObject("UIManager");
        uiManager = uiObject.AddComponent<UIManager>();

        Debug.Log("Akash Demo: UI manager setup complete");
    }


    /// Sets up the player movement component

    private void SetupPlayerMovement()
    {
        if (!createPlayerPrefab) return;

        // Check if player already exists
        if (FindObjectOfType<PlayerMovement>() != null)
        {
            Debug.Log("Akash Demo: PlayerMovement already exists, skipping creation");
            return;
        }

        GameObject playerObject = new GameObject("Player");
        playerMovement = playerObject.AddComponent<PlayerMovement>();

        // Set player to start near the logo
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 3f;
        playerObject.transform.position = new Vector3(spawnOffset.x, spawnOffset.y, 0f);

        Debug.Log("Akash Demo: Player movement setup complete");
    }


    /// Sets up Akash branding elements

    private void SetupAkashBranding()
    {
        if (!showAkashText) return;

        // Create title text
        CreateBrandingText(akashTitle, new Vector3(0, 6f, 0), 24, akashBrandColor);

        // Create subtitle text
        CreateBrandingText(akashSubtitle, new Vector3(0, 5.2f, 0), 16, Color.white);

        // Create demo info text
        string demoInfo = "Multiplayer Demo powered by Colyseus";
        CreateBrandingText(demoInfo, new Vector3(0, -6f, 0), 14, Color.gray);

        Debug.Log("Akash Demo: Branding setup complete");
    }


    /// Creates branded text elements

    private void CreateBrandingText(string text, Vector3 position, int fontSize, Color color)
    {
        GameObject textObject = new GameObject("Text_" + text.Replace(" ", "_"));
        textObject.transform.position = position;

        // This would require TextMesh Pro in a real Unity scene
        // For now, we'll just log the text setup
        Debug.Log("Akash Demo: Created branding text '" + text + "' at " + position);

        // In a real Unity scene, you would add TextMeshPro component here:
        // TextMeshPro textComponent = textObject.AddComponent<TextMeshPro>();
        // textComponent.text = text;
        // textComponent.fontSize = fontSize;
        // textComponent.color = color;
        // textComponent.alignment = TextAlignmentOptions.Center;
    }


    /// Creates a demo boundary for visual reference

    private void CreateDemoBoundary()
    {
        // Create boundary lines
        float halfWidth = sceneSize.x / 2f;
        float halfHeight = sceneSize.y / 2f;

        // Top boundary
        CreateBoundaryLine(new Vector3(-halfWidth, halfHeight, 0), new Vector3(halfWidth, halfHeight, 0));
        // Bottom boundary
        CreateBoundaryLine(new Vector3(-halfWidth, -halfHeight, 0), new Vector3(halfWidth, -halfHeight, 0));
        // Left boundary
        CreateBoundaryLine(new Vector3(-halfWidth, -halfHeight, 0), new Vector3(-halfWidth, halfHeight, 0));
        // Right boundary
        CreateBoundaryLine(new Vector3(halfWidth, -halfHeight, 0), new Vector3(halfWidth, halfHeight, 0));
    }


    /// Creates a boundary line using LineRenderer

    private void CreateBoundaryLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("BoundaryLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = new Color(akashBrandColor.r, akashBrandColor.g, akashBrandColor.b, 0.3f);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 1;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }


    /// Saves the current scene (Unity Editor only)

    [ContextMenu("Save Scene As AkashDemo")]
    public void SaveSceneAsAkashDemo()
    {
#if UNITY_EDITOR
        string scenePath = "Assets/Scenes/AkashDemo.unity";

        // Create Scenes directory if it doesn't exist
        if (!System.IO.Directory.Exists("Assets/Scenes"))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // Save the scene
        bool success = UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            SceneManager.GetActiveScene(), scenePath);

        if (success)
        {
            Debug.Log("Akash Demo: Scene saved successfully at " + scenePath);
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Akash Demo: Failed to save scene");
        }
#else
        Debug.Log("Scene saving is only available in Unity Editor");
#endif
    }


    /// Resets the scene to default state

    [ContextMenu("Reset Scene")]
    public void ResetScene()
    {
        // Find and destroy demo objects
        GameObject[] demoObjects = {
            GameObject.Find("AkashLogo"),
            GameObject.Find("DemoGameManager"),
            GameObject.Find("UIManager"),
            GameObject.Find("Player"),
            GameObject.Find("AkashBackground")
        };

        foreach (GameObject obj in demoObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        Debug.Log("Akash Demo: Scene reset complete");
    }


    /// Validates the scene setup

    [ContextMenu("Validate Scene Setup")]
    public void ValidateSceneSetup()
    {
        bool isValid = true;

        // Check required components
        if (FindObjectOfType<Camera>() == null)
        {
            Debug.LogError("Akash Demo: No camera found in scene");
            isValid = false;
        }

        if (FindObjectOfType<AkashLogoDisplay>() == null)
        {
            Debug.LogWarning("Akash Demo: No AkashLogoDisplay found in scene");
        }

        if (FindObjectOfType<DemoGameManager>() == null)
        {
            Debug.LogWarning("Akash Demo: No DemoGameManager found in scene");
        }

        if (FindObjectOfType<PlayerMovement>() == null)
        {
            Debug.LogWarning("Akash Demo: No PlayerMovement found in scene");
        }

        if (isValid)
        {
            Debug.Log("Akash Demo: Scene validation passed!");
        }
        else
        {
            Debug.LogError("Akash Demo: Scene validation failed!");
        }
    }
}