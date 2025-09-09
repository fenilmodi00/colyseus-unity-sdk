using System;
using System.Threading.Tasks;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;
using System.Collections.Generic;


/// Simplified player movement for the Akash Colyseus demo.
/// Handles click-to-move controls and demonstrates movement towards the Akash logo.

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement Settings")]
	public float speed = 8f;
	public float smoothTime = 0.3f;

	[Header("Visual Settings")]
	public Color playerColor = Color.blue;
	public Vector2 playerSize = Vector2.one;

	private bool _moving;
	private NetworkManager _networkManager;
	private Vector2 _targetPosition;
	private SpriteRenderer _spriteRenderer;
	private string _mySessionId;
	private Dictionary<string, GameObject> _otherPlayers = new Dictionary<string, GameObject>();
	private AkashLogoDisplay _akashLogo;
	private Vector2 _velocity;

	private void Awake()
	{
		// Ensure only one PlayerMovement exists
		PlayerMovement[] existingPlayers = FindObjectsOfType<PlayerMovement>();
		if (existingPlayers.Length > 1)
		{
			Debug.LogWarning("Akash Demo: Multiple PlayerMovement instances found, destroying duplicate");
			Destroy(gameObject);
			return;
		}
		
		_networkManager = FindObjectOfType<NetworkManager>();
		if (_networkManager == null)
		{
			_networkManager = gameObject.AddComponent<NetworkManager>();
		}

		SetupPlayerVisual();
		_akashLogo = FindObjectOfType<AkashLogoDisplay>();
		
		Debug.Log("Akash Demo: PlayerMovement initialized");
	}

	private async void Start()
	{
		// Set initial position near logo
		if (_akashLogo != null)
		{
			Vector2 logoPos = _akashLogo.GetLogoPosition();
			Vector2 spawnOffset = UnityEngine.Random.insideUnitCircle.normalized * 5f;
			transform.position = logoPos + spawnOffset;
			_targetPosition = transform.position;
		}
		else
		{
			// Default spawn position if no logo found
			transform.position = Vector3.zero;
			_targetPosition = transform.position;
		}

		// Try to connect to network (but don't break if it fails)
		try
		{
			bool connected = await _networkManager.JoinOrCreateGame();
			if (connected && _networkManager.GameRoom != null)
			{
				_mySessionId = _networkManager.GameRoom.SessionId;
				registerListeners();
				Debug.Log("Akash Demo: Player movement connected to network");
			}
			else
			{
				Debug.LogWarning("Akash Demo: Player movement running in offline mode");
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning($"Akash Demo: Network connection failed, running offline: {e.Message}");
		}
	}

	private void Update()
	{
		HandleInput();
		UpdateMovement();
	}


	/// Handles mouse click input for movement

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0f;

			// Handle Shift+Click for logo movement first
			if (Input.GetKey(KeyCode.LeftShift) && _akashLogo != null)
			{
				Vector2 logoPos = _akashLogo.GetLogoPosition();
				_targetPosition = logoPos;
				_moving = true;
				Debug.Log("Akash Demo: Moving towards Akash logo!");

				// Also send to server if connected
				if (_networkManager != null && _networkManager.GameRoom != null)
				{
					_networkManager.PlayerPosition(logoPos);
				}
			}
			else
			{
				// Regular click movement
				_targetPosition = mouseWorldPos;
				_moving = true;
				Debug.Log($"Akash Demo: Moving to {mouseWorldPos}");

				// Also send to server if connected
				if (_networkManager != null && _networkManager.GameRoom != null)
				{
					_networkManager.PlayerPosition(mouseWorldPos);
				}
			}
		}
	}


	/// Updates smooth movement towards target position

	private void UpdateMovement()
	{
		if (_moving && Vector2.Distance(transform.position, _targetPosition) > 0.1f)
		{
			// Use smooth movement instead of linear
			transform.position = Vector2.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);
		}
		else
		{
			_moving = false;
			_velocity = Vector2.zero;
		}
	}


	/// Sets up the visual representation of the player

	private void SetupPlayerVisual()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (_spriteRenderer == null)
		{
			_spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		}

		// Create a simple circle sprite for the player
		Texture2D texture = new Texture2D(32, 32);
		Color[] pixels = new Color[32 * 32];
		Vector2 center = new Vector2(16, 16);

		for (int y = 0; y < 32; y++)
		{
			for (int x = 0; x < 32; x++)
			{
				float distance = Vector2.Distance(new Vector2(x, y), center);
				pixels[y * 32 + x] = distance <= 15 ? playerColor : Color.clear;
			}
		}

		texture.SetPixels(pixels);
		texture.Apply();

		Sprite playerSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
		_spriteRenderer.sprite = playerSprite;
		transform.localScale = new Vector3(playerSize.x, playerSize.y, 1f);
	}


	/// Registers Colyseus network event listeners

	private void registerListeners()
	{
		if (_networkManager?.GameRoom == null)
		{
			Debug.LogWarning("Akash Demo: Cannot register listeners - GameRoom is null");
			return;
		}

		try
		{
			// Handle welcome message
			_networkManager.GameRoom.OnMessage<string>("welcomeMessage", message =>
			{
				Debug.Log($"Akash Demo: {message}");
			});

			// Handle state changes with simplified approach for beginners
			_networkManager.GameRoom.OnStateChange += (state, firstState) =>
			{
				if (state == null) return;

				// Update all players
				foreach (var key in state.players.Keys)
				{
					string playerId = key as string;
					Player player = state.players[key] as Player;

					if (player != null && playerId != null)
					{
						if (playerId == _mySessionId)
						{
							// Update my position from server
							_targetPosition = new Vector2(player.x, player.y);
							_moving = true;
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

			Debug.Log("Akash Demo: Network listeners registered successfully");
		}
		catch (Exception e)
		{
			Debug.LogError($"Akash Demo: Failed to register network listeners: {e.Message}");
		}
	}


	/// Updates or creates visual representation for other players

	private void UpdateOtherPlayer(string playerId, Vector2 position)
	{
		if (!_otherPlayers.ContainsKey(playerId))
		{
			// Create new player object
			GameObject otherPlayer = new GameObject($"Player_{playerId}");
			SpriteRenderer sr = otherPlayer.AddComponent<SpriteRenderer>();

			// Create different colored sprite for other players
			Color otherColor = GetPlayerColor(playerId);
			sr.sprite = CreatePlayerSprite(otherColor);
			otherPlayer.transform.localScale = new Vector3(playerSize.x, playerSize.y, 1f);

			_otherPlayers[playerId] = otherPlayer;
			Debug.Log($"Akash Demo: Player {playerId} joined the game!");
		}

		// Update position
		_otherPlayers[playerId].transform.position = position;
	}


	/// Removes visual representation of disconnected players

	private void CleanupDisconnectedPlayers(MyRoomState state)
	{
		List<string> playersToRemove = new List<string>();

		foreach (var playerId in _otherPlayers.Keys)
		{
			if (!state.players.ContainsKey(playerId))
			{
				playersToRemove.Add(playerId);
			}
		}

		foreach (var playerId in playersToRemove)
		{
			if (_otherPlayers[playerId] != null)
			{
				Destroy(_otherPlayers[playerId]);
			}
			_otherPlayers.Remove(playerId);
			Debug.Log($"Akash Demo: Player {playerId} left the game!");
		}
	}


	/// Generates a unique color for each player based on their ID

	private Color GetPlayerColor(string playerId)
	{
		int hash = playerId.GetHashCode();
		UnityEngine.Random.State oldState = UnityEngine.Random.state;
		UnityEngine.Random.InitState(hash);

		Color color = new Color(
			UnityEngine.Random.Range(0.3f, 1f),
			UnityEngine.Random.Range(0.3f, 1f),
			UnityEngine.Random.Range(0.3f, 1f),
			1f
		);

		UnityEngine.Random.state = oldState;
		return color;
	}


	/// Creates a sprite with the specified color

	private Sprite CreatePlayerSprite(Color color)
	{
		Texture2D texture = new Texture2D(32, 32);
		Color[] pixels = new Color[32 * 32];
		Vector2 center = new Vector2(16, 16);

		for (int y = 0; y < 32; y++)
		{
			for (int x = 0; x < 32; x++)
			{
				float distance = Vector2.Distance(new Vector2(x, y), center);
				pixels[y * 32 + x] = distance <= 15 ? color : Color.clear;
			}
		}

		texture.SetPixels(pixels);
		texture.Apply();

		return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
	}

	private void OnDestroy()
	{
		// Clean up other player objects
		foreach (var playerObj in _otherPlayers.Values)
		{
			if (playerObj != null)
			{
				Destroy(playerObj);
			}
		}
		_otherPlayers.Clear();
	}
}