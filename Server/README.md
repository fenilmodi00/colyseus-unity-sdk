# Colyseus Unity SDK Server

A multiplayer game server built with Colyseus framework for Unity games. This server provides real-time multiplayer functionality with room management, state synchronization, and WebSocket communication.

## What This Server Provides

- **Colyseus Framework** - Multiplayer game server framework
- **WebSocket Communication** - Real-time messaging on port 2567
- **Room Management** - Game rooms with player state synchronization
- **Unity Integration** - Easy connection from Unity clients

## Technology Stack

- **Node.js** - Server runtime
- **TypeScript** - Programming language
- **Colyseus 0.16** - Multiplayer game framework
- **Express** - HTTP server
- **WebSocket** - Real-time communication
- **Docker** - Container deployment

## Prerequisites

- Node.js 18+
- npm package manager
- Docker

## Local Development

### Installation

```bash
# Install dependencies
npm install

# Copy environment file
cp .env.example .env

# Edit .env with your settings
nano .env
```

### Running the Server

```bash
# Development mode (with file watching)
npm run start

# Production mode
npm run start:prod

# Build TypeScript
npm run build
```

The server will start on:
- **WebSocket & HTTP:** port 2567 (single port used by both)
- **Health checks:** http://localhost:2567/health

### Using Docker

```bash
# Build and run
docker-compose up --build

# Run in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```
## Unity Client Connection

### Installing the SDK in Your Project

### Using the Included Example Project

This repository includes a complete Unity example project with scenes and scripts ready to test:

**1. Open the Project**
```bash
# Clone the repository
git clone https://github.com/fenilmodi00/colyseus-unity-sdk.git

# Open in Unity 6+ (or Unity 2022.3+)
# File > Open Project > Select colyseus-unity-sdk folder
```

**2. Load the Menu Scene**
- Navigate to: `Assets > Colyseus > Runtime > Examples > Scenes`
- Double-click `Menu.unity` to load it
- Click Play button in Unity Editor

**3. Test Connection**
- Menu scene provides connection UI
- Default settings: `localhost:2567`
- Start the server first (see above), then test in Unity
- Check Unity Console for connection logs

**4. Example Scripts Included**
- `MenuManager.cs` - Connection UI and settings
- `NetworkManager.cs` - Colyseus client management
- `GameplayManager.cs` - Game scene management
- `PlayerMovement.cs` - Synchronized player movement
- `MyRoomState.cs` & `Player.cs` - State schemas

### Installing the SDK in Your Project

**Option 1: Direct Copy (Recommended)**
```bash
# Clone this repository
git clone https://github.com/fenilmodi00/colyseus-unity-sdk.git

# Copy the SDK into your Unity project
cp -r colyseus-unity-sdk/Assets/Colyseus /path/to/your-unity-project/Assets/
```

**Option 2: Unity Package Manager**
1. In Unity: Go to `Window > Package Manager`
2. Click "+" button → "Add package from git URL"
3. Enter: `https://github.com/fenilmodi00/colyseus-unity-sdk.git`
4. Click "ADD"

### Quick Setup

**1. Basic Connection Script**
```csharp
using Colyseus;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    private ColyseusClient client;
    private ColyseusRoom<MyRoomState> room;

    async void Start()
    {
        // Connect to your server (change URL for production)
        client = new ColyseusClient("ws://localhost:2567");
        try
        {
            // Join or create a room
            room = await client.JoinOrCreate<MyRoomState>("my_room");
            Debug.Log("Connected to room: " + room.Id)
            SetupRoomEvents();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection failed: " + e.Message);
        }
    }
    void SetupRoomEvents()
    {
        // Room joined successfully
        room.OnJoin += () => {
            Debug.Log("Successfully joined room!");
        };
        // Room left
        room.OnLeave += (code) => {
            Debug.Log($"Left room with code: {code}");
        };
        // State changed
        room.OnStateChange += (state, isFirstState) => {
            if (isFirstState) {
                Debug.Log("Initial room state received");
            }
        };
        // Listen for custom messages
        room.OnMessage<object>("welcomeMessage", (message) => {
            Debug.Log("Server says: " + message);
        });
    }
    public void SendPlayerPosition(Vector2 position)
    {
        room?.Send("position", new { x = position.x, y = position.y });
    }
}
```

**2. Room State Schema**
```csharp
using Colyseus.Schema;

[System.Serializable]
public partial class MyRoomState : Schema
{
    [Type(0, "map", typeof(MapSchema<Player>))]
    public MapSchema<Player> players = new MapSchema<Player>();
}

[System.Serializable]
public partial class Player : Schema
{
    [Type(0, "string")]
    public string id = default(string);
    [Type(1, "number")]
    public float x = default(float);
    [Type(2, "number")]
    public float y = default(float);
}
```

### Advanced Features

**Room Options**
```csharp
Dictionary<string, object> roomOptions = new Dictionary<string, object>
{
    ["maxPlayers"] = 4,
    ["gameMode"] = "battle",
    ["mapName"] = "desert"
};

room = await client.JoinOrCreate<MyRoomState>("my_room", roomOptions);
```

**Listening to State Changes**
```csharp
void SetupStateListeners()
{
    var callbacks = Colyseus.Schema.Callbacks.Get(room);
    // Player added
    callbacks.OnAdd(state => state.players, (key, player) => {
        Debug.Log($"Player {key} joined the game!");
        SpawnPlayer(key, player);
        // Listen to player changes
        callbacks.OnChange(player, (changes) => {
            UpdatePlayerPosition(key, player);
        });
    });
    // Player removed
    callbacks.OnRemove(state => state.players, (key, player) => {
        Debug.Log($"Player {key} left the game!");
        DestroyPlayer(key);
    });
}
```

**Connection Settings**
```csharp
// Local development
string serverUrl = "ws://localhost:2567";

// Production server (replace with your domain)
string serverUrl = "ws://your-domain.com:2567";

// Secure connection over HTTPS (reverse proxy terminates TLS and forwards to 2567)
string serverUrl = "wss://your-domain.com"; // Default 443 externally → proxy to 2567 internally

// Akash Network deployment
string serverUrl = "ws://provider-address:2567";
```

### Testing in Unity Editor

**Unity 6000.1.0b1+ (Multiplayer Play Mode)**
1. Enable Multiplayer Play Mode in Unity
2. Create multiple virtual players
3. Test multiplayer functionality without building

**Older Unity Versions (ParrelSync)**
1. Install ParrelSync from Package Manager
2. Create cloned projects for testing
3. Run multiple instances simultaneously

### Debugging Tips

**Connection Issues**
```csharp
// Add detailed error handling
try
{
    room = await client.JoinOrCreate<MyRoomState>("my_room");
}
catch (System.Exception e)
{
    Debug.LogError($"Connection failed: {e.Message}");
    // Check server is running: curl http://localhost:2567/health
    // Verify WebSocket connection: ws://localhost:2567
}
```

**Development Mode (Prevent Timeout)**
In your server's `app.config.ts`:
```typescript
initializeTransport: (options) => new WebSocketTransport({
    pingInterval: 0, // Disable timeout during debugging
    ...options
})
```
**⚠️ Important**: Set `pingInterval` > 0 in production!

### Platform-Specific Notes

**WebGL**
- Use secure WebSocket (wss://) for HTTPS sites
- Browser security restrictions may apply

**Mobile (iOS/Android)**
- Test network permissions
- Handle connection drops gracefully

**Desktop (Windows/Mac/Linux)**
- Full WebSocket support
- Best performance for development

## Server Endpoints

**IMPORTANT**: All endpoints use port 2567 (the WebSocket port)

### WebSocket Connection (Port 2567)
- **ws://localhost:2567** - Unity client connections
- Available rooms: `my_room`, `lobby`

### HTTP Endpoints (Port 2567)
- **GET http://localhost:2567/** - Server information
- **GET http://localhost:2567/health** - Health check for monitoring
- **GET http://localhost:2567/metrics** - Server metrics
- **GET http://localhost:2567/colyseus** - Admin monitor interface
- **GET http://localhost:2567/playground** - Development playground (dev mode only)

### Health Check Example

```bash
# Check server status
curl http://localhost:2567/health

# Expected response:
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "uptime": 3600,
  "memory": { "rss": 50331648, "heapUsed": 25165824 },
    "environment": "development"
}
```

## Configuration

### Environment Variables (.env)

```bash
# Server Configuration
NODE_ENV=production
WS_PORT=2567              # Single port for WebSocket + HTTP

# Security (Change these!)
JWT_SECRET=your-jwt-secret-change-this
AUTH_SALT=your-auth-salt-change-this
SESSION_SECRET=your-session-secret-change-this
COLYSEUS_MONITOR_PASSWORD=admin-password-change-this

# Logging
LOG_LEVEL=info            # debug, info, warn, error
```

### Security Notes

- Change all default passwords before deployment
- Use strong, random secrets in production
- Enable HTTPS/WSS for production deployments
- Monitor interface is password-protected in production
## Load Testing

### Running Load Tests

```bash
# Test with different client counts
npm run loadtest

# Custom test (10 clients connecting to my_room)
npx tsx loadtest/example.ts --room my_room --numClients 10

# Test specific endpoint
npx tsx loadtest/example.ts --room my_room --numClients 50 --endpoint ws://your-server.com:2567
```

### Health Check

```bash
# Check if server is running
npm run health-check

# Manual health check (single port 2567)
curl -f http://localhost:2567/health
```

## Available Scripts

- `npm run start` - Development server with file watching
- `npm run start:prod` - Production server
- `npm run build` - Build TypeScript files
- `npm run loadtest` - Run load testing with 99 clients
- `npm run health-check` - Check server health
- `npm run schema-codegen` - Generate C# schemas for Unity

## Project Structure

```
Server/
├── src/
│   ├── index.ts          # Server entry point
│   ├── app.config.ts     # Server configuration
│   ├── rooms/
│   │   ├── MyRoom.ts     # Game room implementation
│   │   └── schema/       # State schemas
│   └── config/
│       └── auth.ts       # Authentication setup
├── loadtest/
│   └── example.ts        # Load testing script
├── Dockerfile            # Container definition
├── docker-compose.yml    # Local development
├── package.json          # Dependencies and scripts
├── .env                  # Environment variables
└── tsconfig.json         # TypeScript config
```
