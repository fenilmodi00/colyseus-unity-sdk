using System.Threading.Tasks;
using Colyseus;
using UnityEngine;
using System;

/// <summary>
/// Network manager specifically designed for the Akash Colyseus Unity SDK Demo.
/// Handles connection, room management, and demo-specific networking events.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    [Header("Demo Connection Settings")]
    [SerializeField] private string roomName = "my_room";
    [SerializeField] private int maxReconnectAttempts = 3;
    [SerializeField] private float reconnectDelay = 2f;

    private static ColyseusClient _client = null;
    private static MenuManager _menuManager = null;
    private static ColyseusRoom<MyRoomState> _room = null;
    private int _reconnectAttempts = 0;

    // Events for demo integration
    public Action<string> OnConnectionStatusChanged;
    public Action<int> OnPlayerCountChanged;
    public Action<string> OnPlayerJoined;
    public Action<string> OnPlayerLeft;
    public Action<string> OnErrorOccurred;

    // Connection states
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Failed
    }

    private ConnectionState _currentState = ConnectionState.Disconnected;

    public ConnectionState CurrentConnectionState => _currentState;
    public bool IsConnected => _room != null && _currentState == ConnectionState.Connected;
    public int CurrentPlayerCount => (int)(_room?.State?.playerCount ?? 0);

    private void Awake()
    {
        // Ensure MenuManager is available
        if (_menuManager == null)
        {
            _menuManager = FindObjectOfType<MenuManager>();
            if (_menuManager == null)
            {
                _menuManager = gameObject.AddComponent<MenuManager>();
            }
        }
    }

    /// <summary>
    /// Initializes the Colyseus client for the demo
    /// </summary>
    public void Initialize()
    {
        SetConnectionState(ConnectionState.Connecting);

        try
        {
            _client = new ColyseusClient(_menuManager.HostAddress);
            Debug.Log($"Akash Demo: Initialized client for {_menuManager.HostAddress}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Akash Demo: Failed to initialize client - {e.Message}");
            OnErrorOccurred?.Invoke($"Failed to initialize: {e.Message}");
            SetConnectionState(ConnectionState.Failed);
        }
    }

    /// <summary>
    /// Joins or creates a demo game room
    /// </summary>
    public async Task<bool> JoinOrCreateGame()
    {
        if (_client == null)
        {
            Initialize();
        }

        if (_client == null)
        {
            return false;
        }

        try
        {
            SetConnectionState(ConnectionState.Connecting);

            // Will create a new game room if there is no available game rooms in the server.
            _room = await Client.JoinOrCreate<MyRoomState>(roomName);

            if (_room != null)
            {
                SetConnectionState(ConnectionState.Connected);
                SetupRoomEventHandlers();
                _reconnectAttempts = 0; // Reset reconnect attempts on successful connection

                Debug.Log($"Akash Demo: Successfully joined room {_room.Id}");
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Akash Demo: Failed to join/create room - {e.Message}");
            OnErrorOccurred?.Invoke($"Connection failed: {e.Message}");
            SetConnectionState(ConnectionState.Failed);

            // Attempt reconnection
            await AttemptReconnection();
        }

        return false;
    }

    /// <summary>
    /// Sets up event handlers for the room
    /// </summary>
    private void SetupRoomEventHandlers()
    {
        if (_room == null) return;

        // Handle playground message types to suppress warnings
        _room.OnMessage<object>("__playground_message_types", (message) => {
            // This suppresses the warning - playground messages can be ignored
        });

        // Handle player joined messages
        _room.OnMessage<object>("player_joined", (message) => {
            var data = message as System.Collections.Generic.Dictionary<string, object>;
            if (data != null && data.ContainsKey("playerId"))
            {
                string playerId = data["playerId"].ToString();
                OnPlayerJoined?.Invoke(playerId);

                if (data.ContainsKey("playerCount"))
                {
                    int count = Convert.ToInt32(data["playerCount"]);
                    OnPlayerCountChanged?.Invoke(count);
                }
            }
        });

        // Handle player left messages
        _room.OnMessage<object>("player_left", (message) => {
            var data = message as System.Collections.Generic.Dictionary<string, object>;
            if (data != null && data.ContainsKey("playerId"))
            {
                string playerId = data["playerId"].ToString();
                OnPlayerLeft?.Invoke(playerId);

                if (data.ContainsKey("playerCount"))
                {
                    int count = Convert.ToInt32(data["playerCount"]);
                    OnPlayerCountChanged?.Invoke(count);
                }
            }
        });

        // Handle connection errors
        _room.OnError += (code, message) => {
            Debug.LogError($"Akash Demo: Room error {code}: {message}");
            OnErrorOccurred?.Invoke($"Room error: {message}");
        };

        // Handle room leave
        _room.OnLeave += (code) => {
            Debug.Log($"Akash Demo: Left room with code {code}");
            SetConnectionState(ConnectionState.Disconnected);
        };
    }

    /// <summary>
    /// Attempts to reconnect to the server
    /// </summary>
    private async Task AttemptReconnection()
    {
        if (_reconnectAttempts >= maxReconnectAttempts)
        {
            Debug.LogError("Akash Demo: Max reconnection attempts reached");
            SetConnectionState(ConnectionState.Failed);
            OnErrorOccurred?.Invoke("Connection failed after multiple attempts");
            return;
        }

        _reconnectAttempts++;
        SetConnectionState(ConnectionState.Reconnecting);

        Debug.Log($"Akash Demo: Attempting reconnection {_reconnectAttempts}/{maxReconnectAttempts}");

        await Task.Delay((int)(reconnectDelay * 1000));
        await JoinOrCreateGame();
    }

    /// <summary>
    /// Sets the connection state and notifies listeners
    /// </summary>
    private void SetConnectionState(ConnectionState newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            OnConnectionStatusChanged?.Invoke(newState.ToString());

            // Notify demo manager
            if (DemoGameManager.Instance != null)
            {
                DemoGameManager.Instance.UpdateConnectionStatus(newState.ToString());
            }
        }
    }

    /// <summary>
    /// Gets the Colyseus client instance
    /// </summary>
    public ColyseusClient Client
    {
        get
        {
            // Initialize Colyseus client, if the client has not been initiated yet or input values from the Menu have been changed.
            if (_client == null || !_client.Settings.WebRequestEndpoint.Contains(_menuManager.HostAddress))
            {
                Initialize();
            }
            return _client;
        }
    }

    /// <summary>
    /// Gets the current game room
    /// </summary>
    public ColyseusRoom<MyRoomState> GameRoom
    {
        get
        {
            if (_room == null)
            {
                Debug.LogWarning("Akash Demo: Room hasn't been initialized yet!");
            }
            return _room;
        }
    }

    /// <summary>
    /// Sends player position to the server
    /// </summary>
    /// <param name="position">New player position</param>
    public void PlayerPosition(Vector2 position)
    {
        if (IsConnected)
        {
            _ = GameRoom.Send("position", new { x = position.x, y = position.y });
        }
        else
        {
            Debug.LogWarning("Akash Demo: Cannot send position - not connected to room");
        }
    }

    /// <summary>
    /// Manually disconnects from the room
    /// </summary>
    public async Task Disconnect()
    {
        if (_room != null)
        {
            await _room.Leave();
            _room = null;
        }

        SetConnectionState(ConnectionState.Disconnected);
        Debug.Log("Akash Demo: Manually disconnected from room");
    }

    /// <summary>
    /// Gets current room information
    /// </summary>
    public string GetRoomInfo()
    {
        if (_room != null)
        {
            return $"Room: {_room.Id}, Players: {CurrentPlayerCount}";
        }
        return "Not connected";
    }

    private void OnDestroy()
    {
        if (_room != null)
        {
            _ = _room.Leave();
        }
    }
}
