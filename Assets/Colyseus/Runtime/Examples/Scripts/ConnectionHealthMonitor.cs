using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Monitors connection health and provides resilience features for the Akash Colyseus Demo.
/// Handles automatic reconnection, error recovery, and connection diagnostics.
/// </summary>
public class ConnectionHealthMonitor : MonoBehaviour
{
    [Header("Health Monitoring Settings")]
    [SerializeField] private float healthCheckInterval = 5f;
    [SerializeField] private float connectionTimeout = 10f;
    [SerializeField] private int maxReconnectAttempts = 5;
    [SerializeField] private float reconnectBackoffMultiplier = 1.5f;

    [Header("Diagnostics")]
    [SerializeField] private bool enableDiagnostics = true;
    [SerializeField] private bool logDetailedErrors = true;

    // Health tracking
    private NetworkManager _networkManager;
    private bool _isMonitoring = false;
    private int _reconnectAttempts = 0;
    private float _lastHealthCheck = 0f;
    private Queue<float> _connectionLatencyHistory = new Queue<float>();
    private List<string> _errorHistory = new List<string>();

    // Connection state tracking
    private NetworkManager.ConnectionState _lastKnownState;
    private float _connectionStartTime;
    private bool _hasEverConnected = false;

    // Events for external systems
    public System.Action<string> OnHealthStatusChanged;
    public System.Action<float> OnLatencyUpdated;
    public System.Action<int> OnReconnectAttempt;
    public System.Action OnConnectionRecovered;

    // Health status
    public enum HealthStatus
    {
        Unknown,
        Healthy,
        Degraded,
        Unhealthy,
        Recovering
    }

    private HealthStatus _currentHealthStatus = HealthStatus.Unknown;

    public HealthStatus CurrentHealth => _currentHealthStatus;
    public float AverageLatency => CalculateAverageLatency();
    public int ReconnectAttempts => _reconnectAttempts;
    public bool HasEverConnected => _hasEverConnected;

    private void Awake()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogError("ConnectionHealthMonitor: NetworkManager not found!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        SubscribeToNetworkEvents();
        StartHealthMonitoring();
    }

    /// <summary>
    /// Subscribes to network events for health monitoring
    /// </summary>
    private void SubscribeToNetworkEvents()
    {
        if (_networkManager != null)
        {
            _networkManager.OnConnectionStatusChanged += OnConnectionStatusChanged;
            _networkManager.OnErrorOccurred += OnNetworkError;
        }
    }

    /// <summary>
    /// Starts the health monitoring coroutine
    /// </summary>
    private void StartHealthMonitoring()
    {
        if (!_isMonitoring)
        {
            _isMonitoring = true;
            StartCoroutine(HealthMonitoringCoroutine());
            Debug.Log("Akash Demo: Connection health monitoring started");
        }
    }

    /// <summary>
    /// Stops the health monitoring
    /// </summary>
    public void StopHealthMonitoring()
    {
        _isMonitoring = false;
        Debug.Log("Akash Demo: Connection health monitoring stopped");
    }

    /// <summary>
    /// Main health monitoring coroutine
    /// </summary>
    private IEnumerator HealthMonitoringCoroutine()
    {
        while (_isMonitoring)
        {
            PerformHealthCheck();
            yield return new WaitForSeconds(healthCheckInterval);
        }
    }

    /// <summary>
    /// Performs a comprehensive health check
    /// </summary>
    private void PerformHealthCheck()
    {
        _lastHealthCheck = Time.time;

        if (_networkManager == null)
        {
            UpdateHealthStatus(HealthStatus.Unhealthy, "NetworkManager is null");
            return;
        }

        NetworkManager.ConnectionState currentState = _networkManager.CurrentConnectionState;

        switch (currentState)
        {
            case NetworkManager.ConnectionState.Connected:
                HandleConnectedState();
                break;

            case NetworkManager.ConnectionState.Connecting:
            case NetworkManager.ConnectionState.Reconnecting:
                HandleConnectingState();
                break;

            case NetworkManager.ConnectionState.Disconnected:
                HandleDisconnectedState();
                break;

            case NetworkManager.ConnectionState.Failed:
                HandleFailedState();
                break;
        }

        // Update diagnostics
        if (enableDiagnostics)
        {
            UpdateDiagnostics();
        }
    }

    /// <summary>
    /// Handles health check when connected
    /// </summary>
    private void HandleConnectedState()
    {
        if (!_hasEverConnected)
        {
            _hasEverConnected = true;
            _connectionStartTime = Time.time;
            OnConnectionRecovered?.Invoke();
        }

        // Reset reconnect attempts on successful connection
        if (_reconnectAttempts > 0)
        {
            _reconnectAttempts = 0;
            UpdateHealthStatus(HealthStatus.Healthy, "Connection recovered");
        }
        else
        {
            UpdateHealthStatus(HealthStatus.Healthy, "Connection stable");
        }

        // Simulate latency check (in real implementation, you'd ping the server)
        float simulatedLatency = Random.Range(50f, 150f);
        UpdateLatency(simulatedLatency);
    }

    /// <summary>
    /// Handles health check when connecting
    /// </summary>
    private void HandleConnectingState()
    {
        float connectionDuration = Time.time - _connectionStartTime;

        if (connectionDuration > connectionTimeout)
        {
            UpdateHealthStatus(HealthStatus.Unhealthy, "Connection timeout");
            TriggerReconnectAttempt();
        }
        else
        {
            UpdateHealthStatus(HealthStatus.Recovering, "Attempting connection...");
        }
    }

    /// <summary>
    /// Handles health check when disconnected
    /// </summary>
    private void HandleDisconnectedState()
    {
        UpdateHealthStatus(HealthStatus.Unhealthy, "Disconnected from server");

        if (_hasEverConnected)
        {
            TriggerReconnectAttempt();
        }
    }

    /// <summary>
    /// Handles health check when connection failed
    /// </summary>
    private void HandleFailedState()
    {
        UpdateHealthStatus(HealthStatus.Unhealthy, "Connection failed");
        TriggerReconnectAttempt();
    }

    /// <summary>
    /// Triggers a reconnection attempt with exponential backoff
    /// </summary>
    private void TriggerReconnectAttempt()
    {
        if (_reconnectAttempts >= maxReconnectAttempts)
        {
            UpdateHealthStatus(HealthStatus.Unhealthy, "Max reconnect attempts reached");

            // Notify user of persistent connection issues
            if (DemoGameManager.Instance != null)
            {
                DemoGameManager.Instance.ShowError(
                    "Unable to maintain connection. Please check your network and restart the demo.");
            }
            return;
        }

        _reconnectAttempts++;
        OnReconnectAttempt?.Invoke(_reconnectAttempts);

        // Calculate backoff delay
        float backoffDelay = Mathf.Pow(reconnectBackoffMultiplier, _reconnectAttempts - 1);

        StartCoroutine(ReconnectWithDelay(backoffDelay));

        Debug.Log($@"Akash Demo: Triggering reconnect attempt {_reconnectAttempts}/{maxReconnectAttempts} with {backoffDelay}s delay");
    }

    /// <summary>
    /// Attempts reconnection after a delay
    /// </summary>
    private IEnumerator ReconnectWithDelay(float delay)
    {
        UpdateHealthStatus(HealthStatus.Recovering, $@"Reconnecting in {delay:F1}s...");

        yield return new WaitForSeconds(delay);

        if (_networkManager != null)
        {
            _connectionStartTime = Time.time;
            _ = _networkManager.JoinOrCreateGame();
        }
    }

    /// <summary>
    /// Updates the current health status
    /// </summary>
    private void UpdateHealthStatus(HealthStatus newStatus, string reason)
    {
        if (_currentHealthStatus != newStatus)
        {
            _currentHealthStatus = newStatus;

            string statusMessage = $@"Health: {newStatus} - {reason}";
            Debug.Log($@"Akash Demo: {statusMessage}");

            OnHealthStatusChanged?.Invoke(statusMessage);

            // Update UI if available
            if (DemoGameManager.Instance != null)
            {
                DemoGameManager.Instance.UpdateConnectionStatus(newStatus.ToString());
            }
        }
    }

    /// <summary>
    /// Updates connection latency tracking
    /// </summary>
    private void UpdateLatency(float latency)
    {
        _connectionLatencyHistory.Enqueue(latency);

        // Keep only last 10 measurements
        while (_connectionLatencyHistory.Count > 10)
        {
            _connectionLatencyHistory.Dequeue();
        }

        OnLatencyUpdated?.Invoke(latency);
    }

    /// <summary>
    /// Calculates average latency from recent measurements
    /// </summary>
    private float CalculateAverageLatency()
    {
        if (_connectionLatencyHistory.Count == 0) return 0f;

        float total = 0f;
        foreach (float latency in _connectionLatencyHistory)
        {
            total += latency;
        }

        return total / _connectionLatencyHistory.Count;
    }

    /// <summary>
    /// Updates diagnostic information
    /// </summary>
    private void UpdateDiagnostics()
    {
        if (!enableDiagnostics) return;

        // Log periodic diagnostics
        if (Time.time % 30f < healthCheckInterval) // Every 30 seconds
        {
            LogDiagnostics();
        }
    }

    /// <summary>
    /// Logs comprehensive diagnostic information
    /// </summary>
    private void LogDiagnostics()
    {
        if (!logDetailedErrors) return;

        string diagnostics = $@"Akash Demo Diagnostics:
Health: {_currentHealthStatus}
State: {_networkManager?.CurrentConnectionState}
Reconnects: {_reconnectAttempts}/{maxReconnectAttempts}
Avg Latency: {AverageLatency:F1}ms
Uptime: {(Time.time - _connectionStartTime):F1}s";

        Debug.Log(diagnostics);
    }

    /// <summary>
    /// Handles connection status changes from NetworkManager
    /// </summary>
    private void OnConnectionStatusChanged(string status)
    {
        if (System.Enum.TryParse(status, out NetworkManager.ConnectionState newState))
        {
            if (_lastKnownState != newState)
            {
                _lastKnownState = newState;

                // Reset connection start time on new connection attempts
                if (newState == NetworkManager.ConnectionState.Connecting ||
                    newState == NetworkManager.ConnectionState.Reconnecting)
                {
                    _connectionStartTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// Handles network errors
    /// </summary>
    private void OnNetworkError(string error)
    {
        _errorHistory.Add($@"{Time.time:F1}: {error}");

        // Keep only last 20 errors
        while (_errorHistory.Count > 20)
        {
            _errorHistory.RemoveAt(0);
        }

        if (logDetailedErrors)
        {
            Debug.LogError($@"Akash Demo Network Error: {error}");
        }

        UpdateHealthStatus(HealthStatus.Degraded, $@"Network error: {error}");
    }

    /// <summary>
    /// Gets a summary of recent errors
    /// </summary>
    public string GetErrorSummary()
    {
        if (_errorHistory.Count == 0)
        {
            return "No recent errors";
        }

        return string.Join("\n", _errorHistory.ToArray());
    }

    /// <summary>
    /// Forces a manual reconnection attempt
    /// </summary>
    [ContextMenu("Force Reconnect")]
    public void ForceReconnect()
    {
        Debug.Log("Akash Demo: Forcing manual reconnection...");
        _reconnectAttempts = 0; // Reset attempts for manual reconnect
        TriggerReconnectAttempt();
    }

    /// <summary>
    /// Resets the health monitor state
    /// </summary>
    public void ResetHealthMonitor()
    {
        _reconnectAttempts = 0;
        _hasEverConnected = false;
        _connectionLatencyHistory.Clear();
        _errorHistory.Clear();
        _currentHealthStatus = HealthStatus.Unknown;

        Debug.Log("Akash Demo: Health monitor reset");
    }

    private void OnDestroy()
    {
        StopHealthMonitoring();

        if (_networkManager != null)
        {
            _networkManager.OnConnectionStatusChanged -= OnConnectionStatusChanged;
            _networkManager.OnErrorOccurred -= OnNetworkError;
        }
    }

    // Debug GUI for monitoring
    private void OnGUI()
    {
        if (!enableDiagnostics || !Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 100));
        GUILayout.Label($@"Health: {_currentHealthStatus}");
        GUILayout.Label($@"Latency: {AverageLatency:F1}ms");
        GUILayout.Label($@"Reconnects: {_reconnectAttempts}/{maxReconnectAttempts}");

        if (GUILayout.Button("Force Reconnect"))
        {
            ForceReconnect();
        }

        GUILayout.EndArea();
    }
}