using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the user interface for the Akash Colyseus Unity SDK Demo.
/// Displays connection status, player count, messages, and instructions.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Panel Settings")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform uiPanel;

    [Header("Text Display Settings")]
    [SerializeField] private Text connectionStatusText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Text messageText;
    [SerializeField] private Text instructionsText;

    [Header("UI Colors")]
    [SerializeField] private Color connectedColor = Color.green;
    [SerializeField] private Color disconnectedColor = Color.red;
    [SerializeField] private Color connectingColor = Color.yellow;
    [SerializeField] private Color errorColor = Color.red;

    [Header("Message Settings")]
    [SerializeField] private float messageDisplayTime = 5f;
    [SerializeField] private float messageFadeTime = 1f;

    private Coroutine _messageCoroutine;
    private bool _isInitialized = false;

    private void Awake()
    {
        InitializeUI();
    }

    private void Start()
    {
        SetupInitialUI();
    }

    /// <summary>
    /// Initializes the UI components
    /// </summary>
    private void InitializeUI()
    {
        // Create canvas if not present
        if (uiCanvas == null)
        {
            CreateUICanvas();
        }

        // Create UI panel if not present
        if (uiPanel == null)
        {
            CreateUIPanel();
        }

        // Create text components
        CreateTextComponents();

        _isInitialized = true;
        Debug.Log("Akash Demo: UI Manager initialized");
    }

    /// <summary>
    /// Creates the main UI canvas
    /// </summary>
    private void CreateUICanvas()
    {
        GameObject canvasObject = new GameObject("AkashDemoUI");
        uiCanvas = canvasObject.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 100; // Ensure UI is on top

        // Add CanvasScaler for responsive design
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Add GraphicRaycaster for UI interactions
        canvasObject.AddComponent<GraphicRaycaster>();
    }

    /// <summary>
    /// Creates the main UI panel
    /// </summary>
    private void CreateUIPanel()
    {
        GameObject panelObject = new GameObject("UIPanel");
        panelObject.transform.SetParent(uiCanvas.transform, false);

        uiPanel = panelObject.AddComponent<RectTransform>();

        // Position panel at top-right corner
        uiPanel.anchorMin = new Vector2(1, 1);
        uiPanel.anchorMax = new Vector2(1, 1);
        uiPanel.pivot = new Vector2(1, 1);
        uiPanel.anchoredPosition = new Vector2(-20, -20);
        uiPanel.sizeDelta = new Vector2(350, 200);

        // Add background
        Image panelBackground = panelObject.AddComponent<Image>();
        panelBackground.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
    }

    /// <summary>
    /// Creates all text components for the UI
    /// </summary>
    private void CreateTextComponents()
    {
        // Connection Status Text (Top-right)
        connectionStatusText = CreateTextComponent("ConnectionStatus", "Connection: Disconnected",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-20, -20), disconnectedColor, 16);

        // Player Count Text (Below connection status)
        playerCountText = CreateTextComponent("PlayerCount", "Players: 0",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-20, -50), Color.white, 14);

        // Instructions Text (Bottom-center)
        instructionsText = CreateTextComponent("Instructions", GetInstructionsText(),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(0, 30), Color.white, 14, uiCanvas.transform);
        instructionsText.alignment = TextAnchor.LowerCenter;

        // Message Text (Center)
        messageText = CreateTextComponent("MessageText", "",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            Vector2.zero, Color.white, 18, uiCanvas.transform);
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Creates a text component with specified settings
    /// </summary>
    private Text CreateTextComponent(string name, string content, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 position, Color color, int fontSize, Transform parent = null)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent ?? uiPanel, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = anchorMin;
        textRect.anchorMax = anchorMax;
        textRect.pivot = pivot;
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(300, 30);

        Text textComponent = textObject.AddComponent<Text>();
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.text = content;
        textComponent.color = color;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAnchor.UpperRight;

        return textComponent;
    }

    /// <summary>
    /// Sets up the initial UI state
    /// </summary>
    private void SetupInitialUI()
    {
        if (!_isInitialized) return;

        UpdateConnectionStatus("Disconnected");
        UpdatePlayerCount(0);
        ShowInstructions();
    }

    /// <summary>
    /// Updates the connection status display
    /// </summary>
    /// <param name="status">Current connection status</param>
    public void UpdateConnectionStatus(string status)
    {
        if (connectionStatusText == null) return;

        connectionStatusText.text = $"Connection: {status}";

        // Set color based on status
        switch (status.ToLower())
        {
            case "connected":
                connectionStatusText.color = connectedColor;
                break;
            case "connecting":
            case "reconnecting":
                connectionStatusText.color = connectingColor;
                break;
            case "disconnected":
            case "failed":
                connectionStatusText.color = disconnectedColor;
                break;
            default:
                connectionStatusText.color = Color.white;
                break;
        }
    }

    /// <summary>
    /// Updates the player count display
    /// </summary>
    /// <param name="count">Current number of players</param>
    public void UpdatePlayerCount(int count)
    {
        if (playerCountText == null) return;

        playerCountText.text = $"Players: {count}";

        // Add some visual feedback for player count changes
        if (count > 0)
        {
            playerCountText.color = Color.green;
        }
        else
        {
            playerCountText.color = Color.white;
        }
    }

    /// <summary>
    /// Shows a temporary message to the user
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="duration">How long to show the message (optional)</param>
    public void ShowMessage(string message, float duration = -1f)
    {
        if (messageText == null) return;

        if (_messageCoroutine != null)
        {
            StopCoroutine(_messageCoroutine);
        }

        float displayDuration = duration > 0 ? duration : messageDisplayTime;
        _messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, displayDuration));
    }

    /// <summary>
    /// Shows an error message
    /// </summary>
    /// <param name="error">Error message to display</param>
    public void ShowError(string error)
    {
        if (messageText == null) return;

        messageText.color = errorColor;
        ShowMessage($"Error: {error}", messageDisplayTime * 1.5f);
    }

    /// <summary>
    /// Shows the demo instructions
    /// </summary>
    public void ShowInstructions()
    {
        if (instructionsText == null) return;

        instructionsText.text = GetInstructionsText();
        instructionsText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the demo instructions
    /// </summary>
    public void HideInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the instruction text for the demo
    /// </summary>
    private string GetInstructionsText()
    {
        return "Akash Colyseus Demo | Click to move • Shift+Click to move to logo";
    }

    /// <summary>
    /// Coroutine for displaying messages with fade effect
    /// </summary>
    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        messageText.text = message;
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);
        messageText.gameObject.SetActive(true);

        // Show message for specified duration
        yield return new WaitForSeconds(duration - messageFadeTime);

        // Fade out
        float fadeTimer = 0f;
        Color originalColor = messageText.color;

        while (fadeTimer < messageFadeTime)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / messageFadeTime);
            messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the room information display
    /// </summary>
    /// <param name="roomInfo">Room information string</param>
    public void UpdateRoomInfo(string roomInfo)
    {
        // Could be used to display room ID or other info if needed
        Debug.Log($"Akash Demo Room Info: {roomInfo}");
    }

    /// <summary>
    /// Toggles the visibility of the UI
    /// </summary>
    public void ToggleUI()
    {
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(!uiCanvas.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Shows a welcome message when connected
    /// </summary>
    public void ShowWelcomeMessage()
    {
        ShowMessage("Welcome to the Akash Colyseus Demo!\nMultiplayer networking ready!", 3f);
    }

    /// <summary>
    /// Shows a goodbye message when disconnecting
    /// </summary>
    public void ShowGoodbyeMessage()
    {
        ShowMessage("Disconnected from Akash Demo", 2f);
    }

    private void Update()
    {
        // Handle UI toggle with keyboard shortcut
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }

        // Handle instruction toggle
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (instructionsText != null)
            {
                instructionsText.gameObject.SetActive(!instructionsText.gameObject.activeSelf);
            }
        }
    }

    private void OnDestroy()
    {
        if (_messageCoroutine != null)
        {
            StopCoroutine(_messageCoroutine);
        }
    }
}