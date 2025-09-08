# Colyseus Unity SDK Server

A multiplayer game server built with Colyseus framework for Unity games. This server provides real-time multiplayer functionality with room management, state synchronization, and WebSocket communication.

## What This Server Provides

- **Colyseus Framework** - Multiplayer game server framework
- **WebSocket Communication** - Real-time messaging on port 2567
- **HTTP Endpoints** - Health checks and monitoring (same port as WebSocket)
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
- **WebSocket:** ws://localhost:2567 (for Unity clients)
- **HTTP:** http://localhost:2567 (for health checks)

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

### Basic Connection

```csharp
using Colyseus;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ColyseusClient client;
    private ColyseusRoom<MyRoomState> room;

    void Start()
    {
        // Connect to your server
        client = new ColyseusClient("ws://localhost:2567");
        ConnectToRoom();
    }

    async void ConnectToRoom()
    {
        try
        {
            room = await client.JoinOrCreate<MyRoomState>("my_room");
            Debug.Log("Connected to room: " + room.Id);

            // Setup event listeners
            room.OnJoin += () => Debug.Log("Joined room!");
            room.OnLeave += (code) => Debug.Log("Left room: " + code);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection failed: " + e.Message);
        }
    }

    public void SendMessage(string type, object data)
    {
        room?.Send(type, data);
    }
}
```

### Connection Settings

```csharp
// Local development
string serverUrl = "ws://localhost:2567";

// Production server
string serverUrl = "ws://your-server.com:2567";

// Secure connection (recommended for production)
string serverUrl = "wss://your-server.com:443";
```

## Server Endpoints

The server provides these endpoints:

### WebSocket (Port 2567)
- **ws://localhost:2567** - Unity client connections
- Available rooms: `my_room`, `lobby`

### HTTP Endpoints (Port 2567)
All HTTP endpoints are available on the same port as WebSocket (2567):

- **GET http://localhost:2567/health** - Server health status
- **GET http://localhost:2567/** - Server information
- **GET http://localhost:2567/metrics** - Server metrics
- **GET http://localhost:2567/colyseus** - Monitor interface (production: password protected)
- **GET http://localhost:2567/playground** - Development playground (development only)

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
  "environment": "development",
  "ports": {
    "http": 80,
    "websocket": 2567
  }
}
```

## Configuration

### Environment Variables (.env)

```bash
# Server Configuration
NODE_ENV=production
PORT=80                    # HTTP server port
WS_PORT=2567              # WebSocket server port

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

# Manual health check (specify port 2567)
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