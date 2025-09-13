using System;
using System.Threading.Tasks;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;
using System.Collections.Generic;


/// Enhanced player movement system for the Akash 2D walkway demo.
/// Features 2D physics, gravity, jumping, platform-aware movement, and improved multiplayer synchronization.

public class EnhancedPlayerMovement : MonoBehaviour
{
    [Header("2D Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private LayerMask groundLayer = 1;

    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color[] playerColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };
    [SerializeField] private Vector2 playerSize = Vector2.one;

    [Header("Network Sync")]
    [SerializeField] private float positionSyncRate = 10f;
    [SerializeField] private float smoothingDistance = 0.1f;
    [SerializeField] private bool enablePrediction = true;

    [Header("Platform Detection")]
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);

    // Physics components
    private Rigidbody2D rb2D;
    private BoxCollider2D playerCollider;

    // Movement state
    private bool isGrounded;
    private bool canJump;
    private Vector2 movement;
    private Vector2 targetPosition;
    private bool hasTargetPosition;

    // Network components
    private NetworkManager networkManager;
    private string mySessionId;
    private Dictionary<string, GameObject> otherPlayers = new Dictionary<string, GameObject>();

    // Platform system
    private WalkwaySystem walkwaySystem;

    // Visual effects
    private Vector2 lastSyncedPosition;
    private float lastSyncTime;

    // Events
    public System.Action<Vector2> OnPlayerMoved;
    public System.Action OnPlayerJumped;
    public System.Action<bool> OnGroundedStateChanged;

    private void Awake()
    {
        // Ensure only one EnhancedPlayerMovement exists
        EnhancedPlayerMovement[] existingPlayers = FindObjectsOfType<EnhancedPlayerMovement>();
        if (existingPlayers.Length > 1)
        {
            Debug.LogWarning("Akash Demo: Multiple EnhancedPlayerMovement instances found, destroying duplicate");
            Destroy(gameObject);
            return;
        }

        SetupPhysicsComponents();
        SetupNetworkComponents();
        SetupVisualComponents();

        Debug.Log("Akash Demo: EnhancedPlayerMovement initialized");
    }

    private async void Start()
    {
        // Find walkway system
        walkwaySystem = FindObjectOfType<WalkwaySystem>();
        if (walkwaySystem != null)
        {
            // Position player at start of walkway
            Vector2[] platforms = walkwaySystem.GetPlatformPositions();
            if (platforms.Length > 0)
            {
                Vector3 spawnPos = new Vector3(platforms[0].x, platforms[0].y + 2f, 0);
                transform.position = spawnPos;
                targetPosition = spawnPos;
            }
        }

        // Connect to network
        await ConnectToNetwork();
    }


    /// Sets up physics components for 2D movement

    private void SetupPhysicsComponents()
    {
        // Setup Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }

        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.freezeRotation = true;
        rb2D.gravityScale = gravity / 9.8f;
        rb2D.linearDamping = 2f; // Add some drag for better control

        // Setup Collider
        playerCollider = GetComponent<BoxCollider2D>();
        if (playerCollider == null)
        {
            playerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        playerCollider.size = new Vector2(0.8f, 1.8f);
        playerCollider.offset = new Vector2(0, 0.9f);

        // Ensure we're on the correct layer
        gameObject.layer = LayerMask.NameToLayer("Default");
    }


    /// Sets up network components

    private void SetupNetworkComponents()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            networkManager = gameObject.AddComponent<NetworkManager>();
        }
    }


    /// Sets up visual components

    private void SetupVisualComponents()
    {
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null)
            {
                playerSprite = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        // Create player sprite
        playerSprite.sprite = CreatePlayerSprite(playerColors[0]);
        transform.localScale = new Vector3(playerSize.x, playerSize.y, 1f);
    }

    private void Update()
    {
        CheckGroundStatus();
        HandleInput();
        UpdateMovement();
        UpdateNetworkSync();
    }

    private void FixedUpdate()
    {
        ApplyPhysicsMovement();
    }


    /// Checks if player is grounded using physics overlap

    private void CheckGroundStatus()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.down * groundCheckDistance;
        bool wasGrounded = isGrounded;

        // Use OverlapBox to check for ground
        Collider2D hit = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);
        isGrounded = hit != null;

        // Update jump capability
        canJump = isGrounded;

        // Notify if grounded state changed
        if (wasGrounded != isGrounded)
        {
            OnGroundedStateChanged?.Invoke(isGrounded);
        }
    }


    /// Handles input for movement and jumping

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            Jump();
        }
    }


    /// Handles mouse click for movement targeting

    private void HandleMouseClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Check if click is on a valid platform
        if (walkwaySystem != null)
        {
            Vector2 nearestPlatform = walkwaySystem.GetNearestPlatformPosition(mouseWorldPos);

            // Set target to platform surface
            targetPosition = new Vector2(nearestPlatform.x, nearestPlatform.y + 1f);
            hasTargetPosition = true;

            Debug.Log($"Akash Demo: Moving to platform at {targetPosition}");

            // Send to network
            if (networkManager != null && networkManager.GameRoom != null)
            {
                networkManager.PlayerPosition(targetPosition);
            }

            OnPlayerMoved?.Invoke(targetPosition);
        }
    }


    /// Updates movement logic

    private void UpdateMovement()
    {
        if (!hasTargetPosition) return;

        // Calculate movement direction
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);

        // Stop if we're close enough
        if (distance < 0.5f)
        {
            hasTargetPosition = false;
            movement = Vector2.zero;
            return;
        }

        // Set horizontal movement
        movement.x = direction.x;
    }


    /// Applies physics-based movement in FixedUpdate

    private void ApplyPhysicsMovement()
    {
        Vector2 velocity = rb2D.linearVelocity;

        // Apply horizontal movement
        velocity.x = movement.x * walkSpeed;

        // Apply velocity
        rb2D.linearVelocity = velocity;
    }


    /// Makes the player jump

    public void Jump()
    {
        if (!canJump) return;

        // Calculate jump force needed for desired height
        float jumpForce = Mathf.Sqrt(2f * jumpHeight * gravity);
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

        canJump = false;
        OnPlayerJumped?.Invoke();

        Debug.Log("Akash Demo: Player jumped!");
    }


    /// Connects to the network and sets up multiplayer

    private async Task ConnectToNetwork()
    {
        try
        {
            bool connected = await networkManager.JoinOrCreateGame();
            if (connected && networkManager.GameRoom != null)
            {
                mySessionId = networkManager.GameRoom.SessionId;
                RegisterNetworkListeners();
                Debug.Log("Akash Demo: Enhanced player movement connected to network");
            }
            else
            {
                Debug.LogWarning("Akash Demo: Enhanced player movement running in offline mode");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Akash Demo: Network connection failed, running offline: {e.Message}");
        }
    }


    /// Registers network event listeners

    private void RegisterNetworkListeners()
    {
        if (networkManager?.GameRoom == null) return;

        try
        {
            // Handle state changes
            networkManager.GameRoom.OnStateChange += (state, firstState) =>
            {
                if (state == null) return;

                // Update all players
                foreach (var key in state.players.Keys)
                {
                    string playerId = key as string;
                    Player player = state.players[key] as Player;

                    if (player != null && playerId != null)
                    {
                        if (playerId == mySessionId)
                        {
                            // Update my position from server (with smoothing)
                            SmoothServerPosition(new Vector2(player.x, player.y));
                        }
                        else
                        {
                            // Update other players
                            UpdateOtherPlayer(playerId, new Vector2(player.x, player.y));
                        }
                    }
                }

                // Clean up disconnected players
                CleanupDisconnectedPlayers(state);
            };

            Debug.Log("Akash Demo: Network listeners registered for enhanced movement");
        }
        catch (Exception e)
        {
            Debug.LogError($"Akash Demo: Failed to register network listeners: {e.Message}");
        }
    }


    /// Smoothly interpolates server position updates

    private void SmoothServerPosition(Vector2 serverPosition)
    {
        float distance = Vector2.Distance(transform.position, serverPosition);

        // Only apply server correction if distance is significant
        if (distance > smoothingDistance && enablePrediction)
        {
            // Smoothly move towards server position
            transform.position = Vector2.Lerp(transform.position, serverPosition, Time.deltaTime * 5f);
        }
        else if (!enablePrediction)
        {
            // Direct position update without prediction
            transform.position = serverPosition;
        }

        lastSyncedPosition = serverPosition;
        lastSyncTime = Time.time;
    }


    /// Updates network synchronization

    private void UpdateNetworkSync()
    {
        if (networkManager?.GameRoom == null) return;

        // Send position updates at specified rate
        if (Time.time - lastSyncTime >= 1f / positionSyncRate)
        {
            Vector2 currentPos = transform.position;
            float distance = Vector2.Distance(currentPos, lastSyncedPosition);

            // Only send if position changed significantly
            if (distance > 0.1f)
            {
                networkManager.PlayerPosition(currentPos);
                lastSyncedPosition = currentPos;
                lastSyncTime = Time.time;
            }
        }
    }


    /// Updates or creates visual representation for other players

    private void UpdateOtherPlayer(string playerId, Vector2 position)
    {
        if (!otherPlayers.ContainsKey(playerId))
        {
            // Create new player object
            GameObject otherPlayer = new GameObject($"Player_{playerId}");
            SpriteRenderer sr = otherPlayer.AddComponent<SpriteRenderer>();

            // Create different colored sprite for other players
            Color otherColor = GetPlayerColor(playerId);
            sr.sprite = CreatePlayerSprite(otherColor);
            otherPlayer.transform.localScale = new Vector3(playerSize.x, playerSize.y, 1f);

            // Add smooth movement component
            OtherPlayerSmoothMovement smoothMovement = otherPlayer.AddComponent<OtherPlayerSmoothMovement>();
            smoothMovement.Initialize(position);

            otherPlayers[playerId] = otherPlayer;
            Debug.Log($"Akash Demo: Player {playerId} joined the enhanced walkway!");
        }

        // Update position with smooth movement
        OtherPlayerSmoothMovement movement = otherPlayers[playerId].GetComponent<OtherPlayerSmoothMovement>();
        if (movement != null)
        {
            movement.SetTargetPosition(position);
        }
        else
        {
            otherPlayers[playerId].transform.position = position;
        }
    }


    /// Removes visual representation of disconnected players

    private void CleanupDisconnectedPlayers(MyRoomState state)
    {
        List<string> playersToRemove = new List<string>();

        foreach (var playerId in otherPlayers.Keys)
        {
            if (!state.players.ContainsKey(playerId))
            {
                playersToRemove.Add(playerId);
            }
        }

        foreach (var playerId in playersToRemove)
        {
            if (otherPlayers[playerId] != null)
            {
                Destroy(otherPlayers[playerId]);
            }
            otherPlayers.Remove(playerId);
            Debug.Log($"Akash Demo: Player {playerId} left the enhanced walkway!");
        }
    }


    /// Generates a unique color for each player based on their ID

    private Color GetPlayerColor(string playerId)
    {
        int hash = playerId.GetHashCode();
        int colorIndex = Mathf.Abs(hash) % playerColors.Length;
        return playerColors[colorIndex];
    }


    /// Creates a sprite with the specified color

    private Sprite CreatePlayerSprite(Color color)
    {
        Texture2D texture = new Texture2D(32, 64);
        Color[] pixels = new Color[32 * 64];

        // Create a simple character-like shape
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Head (circle at top)
                if (y > 48 && y < 60)
                {
                    Vector2 center = new Vector2(16, 54);
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * 32 + x] = distance <= 6 ? color : Color.clear;
                }
                // Body (rectangle in middle)
                else if (y > 20 && y <= 48 && x > 10 && x < 22)
                {
                    pixels[y * 32 + x] = color;
                }
                // Legs (two rectangles at bottom)
                else if (y <= 20 && ((x > 8 && x < 14) || (x > 18 && x < 24)))
                {
                    pixels[y * 32 + x] = color;
                }
                else
                {
                    pixels[y * 32 + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 32, 64), new Vector2(0.5f, 0f), 32f);
    }


    /// Gets current movement state information

    public MovementState GetMovementState()
    {
        return new MovementState
        {
            position = transform.position,
            velocity = rb2D.linearVelocity,
            isGrounded = isGrounded,
            hasTarget = hasTargetPosition,
            targetPosition = targetPosition
        };
    }

    private void OnDrawGizmos()
    {
        // Draw ground check area
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 checkPos = (Vector2)transform.position + Vector2.down * groundCheckDistance;
        Gizmos.DrawWireCube(checkPos, groundCheckSize);

        // Draw target position
        if (hasTargetPosition)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }

    private void OnDestroy()
    {
        // Clean up other player objects
        foreach (var playerObj in otherPlayers.Values)
        {
            if (playerObj != null)
            {
                Destroy(playerObj);
            }
        }
        otherPlayers.Clear();
    }
}


/// Data structure for movement state information

[System.Serializable]
public struct MovementState
{
    public Vector2 position;
    public Vector2 velocity;
    public bool isGrounded;
    public bool hasTarget;
    public Vector2 targetPosition;
}


/// Component for smooth movement of other players

public class OtherPlayerSmoothMovement : MonoBehaviour
{
    private Vector2 targetPosition;
    private Vector2 velocity;
    private float smoothTime = 0.3f;

    public void Initialize(Vector2 startPosition)
    {
        transform.position = startPosition;
        targetPosition = startPosition;
    }

    public void SetTargetPosition(Vector2 newTarget)
    {
        targetPosition = newTarget;
    }

    private void Update()
    {
        // Smooth movement towards target
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.SmoothDamp(currentPos, targetPosition, ref velocity, smoothTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}