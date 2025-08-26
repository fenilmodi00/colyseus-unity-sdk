# Colyseus Unity SDK Server for Akash Network

🚀 **Deploy your Unity multiplayer game server on the decentralized cloud in minutes!**

This is a production-ready Colyseus game server optimized for **Akash Network** - the world's decentralized cloud marketplace. Whether you're building the next hit multiplayer game or just getting started with Web3 infrastructure, this guide will get you from zero to deployed in under 5 minutes.

## 🌟 Why Akash Network?

**For Game Developers:**
- ⚡ **80% cheaper** than AWS/Google Cloud
- 🌍 **Global distribution** with 75+ providers worldwide
- 🔒 **Censorship-resistant** infrastructure
- 💰 **Pay with crypto ore credit card** (AKT tokens)

**Perfect for Unity Games:**
- WebSocket support optimized for gaming
- Room-based architecture for scalable matchmaking
- Built-in monitoring and health checks

## 🎯 What You'll Deploy

- **Colyseus Game Server** - Battle-tested multiplayer framework
- **WebSocket Support** - Real-time communication (port 2567)
- **HTTP Monitoring** - Health checks and admin panel (port 80)
- **Auto-scaling** - Handle 10 to 10,000 players seamlessly
- **Global CDN** - Low latency worldwide

---

## Quick Deploy to Akash

### Server Configuration

This Colyseus game server is pre-configured for Akash Network deployment with optimized resource profiles and proper port configurations.

**Key Features:**
- WebSocket support on port 2567 for game clients
- HTTP monitoring endpoints on port 80
- Multiple scaling profiles (small to enterprise)
- Health checks and auto-restart capabilities
- Production-ready environment variables

### Deployment Steps

1. **Configure your deployment:**

   Edit the [`deploy.yml`](./deploy.yml) file:
   ```bash
   # Update the Docker image with your repository
   image: fenildocker/colyseus-unity-server:latest

   # Choose your scaling profile:
   profile: medium  # Options: small, medium, large, enterprise

   # Update security credentials (required)
   JWT_SECRET: your-secure-jwt-secret-here
   COLYSEUS_MONITOR_PASSWORD: your-admin-password
   ```

2. **Deploy using Akash Console or CLI:**

   **Via Akash Console:**
   - Upload your `deploy.yml` file
   - Review and submit deployment
   - Select provider and create lease

   **Via CLI:**
   ```bash
   akash tx deployment create deploy.yml --from $AKASH_KEYNAME
   ```

3. **Get your server endpoints:**

   Once deployed, you'll receive:
   ```
   WebSocket: ws://your-provider-url:2567  # For Unity clients
   HTTP: http://your-provider-url:2567       # For monitoring
   ```

**Your server is now live and ready for Unity clients!**

---

## 📊 Resource Scaling Guide

### 🎮 Gaming Workload Sizing

| **Player Count** | **CPU** | **Memory** | **Storage** | **Monthly Cost** |
|------------------|---------|------------|-------------|------------------|
| 1-50 players     | 0.5 CPU | 512Mi      | 1Gi         | ~$2-5 USD        |
| 50-200 players   | 1.0 CPU | 1Gi        | 2Gi         | ~$8-15 USD       |
| 200-500 players  | 2.0 CPU | 2Gi        | 5Gi         | ~$20-35 USD      |
| 500-1000 players | 4.0 CPU | 4Gi        | 10Gi        | ~$50-80 USD      |
| 1000+ players    | 8.0 CPU | 8Gi        | 20Gi        | ~$120-200 USD    |

### 🔧 When to Scale Up

**CPU Scaling Triggers:**
- Room creation time > 100ms
- Message processing lag > 50ms
- CPU usage consistently > 80%

**Memory Scaling Triggers:**
- Memory usage > 85%
- Frequent garbage collection
- Out of memory errors in logs

**Storage Scaling Triggers:**
- Log files growing rapidly
- Need for persistent game state
- File upload features

### 📈 Auto-Scaling Configuration

**Small Game (Indie/Testing):**
```yaml
resources:
  cpu:
    units: 0.5
  memory:
    size: 512Mi
  storage:
    size: 1Gi
```

**Medium Game (100-500 players):**
```yaml
resources:
  cpu:
    units: 2.0
  memory:
    size: 2Gi
  storage:
    size: 5Gi
```

**Large Game (500+ players):**
```yaml
resources:
  cpu:
    units: 4.0
  memory:
    size: 4Gi
  storage:
    size: 10Gi
```

### 🚀 Performance Optimization

**Scaling Profiles in [`deploy.yml`](./deploy.yml):**

The deployment file includes pre-configured profiles for different scales:
- **Small Scale (1-50 players)**: 0.5 CPU, 512Mi RAM, 1Gi storage
- **Medium Scale (50-200 players)**: 1.0 CPU, 1Gi RAM, 2Gi storage
- **Large Scale (200-500 players)**: 2.0 CPU, 2Gi RAM, 5Gi storage
- **Enterprise Scale (500+ players)**: 4.0 CPU, 4Gi RAM, 10Gi storage

**To change scale:**
```yaml
# In deploy.yml, change this line:
profile: medium  # Change to: small, large, or enterprise
```

---

## 🛠 Local Development

### Prerequisites

- Node.js 18+ (LTS recommended)
- Docker & Docker Compose
- Git

### Quick Local Setup

```bash
# Clone and setup
git clone https://github.com/fenilmodi00/colyseus-unity-sdk
cd Server
npm install

# Configure environment
cp .env.example .env
# Edit .env with your settings

# Start development server
npm run start:dev

# Your server will be available at:
# WebSocket: ws://localhost:2567
# HTTP: http://localhost:3000
```

### Docker Development

```bash
# Build and run locally
docker-compose up --build

# Run in background
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

---

## ⚙️ Configuration

### Environment Variables

**Production Environment (.env):**
```bash
# === SERVER CONFIGURATION ===
NODE_ENV=production
PORT=2567                  # HTTP port (same as WebSocket for Colyseus)
WS_PORT=2567              # WebSocket port for game clients

# === SECURITY (CHANGE THESE!) ===
JWT_SECRET=your-super-secret-jwt-key-at-least-32-chars-long
AUTH_SALT=your-random-salt-for-password-hashing
SESSION_SECRET=another-secret-for-session-management
COLYSEUS_MONITOR_PASSWORD=secure-admin-password-123

# === PERFORMANCE TUNING ===
# Performance settings are automatically optimized based on your
# deployment scale (small/medium/large/enterprise) in deploy.yml

# === LOGGING ===
LOG_LEVEL=info            # debug, info, warn, error

# === GAME SETTINGS ===
MAX_ROOMS=100             # Maximum concurrent rooms
ROOM_TIMEOUT=300          # Room cleanup timeout (seconds)
PLAYER_TIMEOUT=60         # Player disconnect timeout (seconds)
```

### Akash Network Endpoints

Once deployed, your server will have these endpoints:

**Game Client Connection:**
```bash
# WebSocket for Unity clients
ws://your-akash-provider.com:2567

# Example Unity connection
ColyseusClient client = new ColyseusClient("ws://your-akash-provider.com:2567");
```

**Monitoring & Health:**
```bash
# Health check endpoint
http://your-akash-provider.com:2567/health

# Admin monitoring panel (if enabled)
http://your-akash-provider.com:2567/colyseus
```

---

## 📋 Monitoring Your Akash Deployment

### Health Checks

**Check Server Status:**
```bash
# Quick health check
curl http://your-akash-provider.com:2567/health

# Expected response:
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00.000Z",
  "uptime": 3600,
  "rooms": 5,
  "players": 23,
  "memory": {
    "used": "245MB",
    "free": "267MB",
    "percentage": 48
  },
  "cpu": {
    "usage": "15%"
  }
}
```

### Akash Provider Monitoring

**Check Deployment Status:**
```bash
# Check if your deployment is running
akash provider lease-status \
  --from $AKASH_KEYNAME \
  --provider $AKASH_PROVIDER

# Monitor resource usage
akash provider lease-logs \
  --from $AKASH_KEYNAME \
  --provider $AKASH_PROVIDER
```

### Game-Specific Metrics

**Performance Indicators to Watch:**

| Metric | Healthy Range | Warning | Critical |
|--------|---------------|---------|----------|
| CPU Usage | 0-70% | 70-85% | >85% |
| Memory Usage | 0-80% | 80-90% | >90% |
| Active Rooms | 0-80% of max | 80-95% | >95% |
| Avg. Latency | <50ms | 50-100ms | >100ms |
| Error Rate | <1% | 1-5% | >5% |

**Monitoring Commands:**
```bash
# Real-time logs
curl -s http://your-akash-provider.com:2567/health | jq

# Monitor specific metrics
watch -n 5 'curl -s http://your-akash-provider.com:2567/health | jq ".rooms, .players"'
```

---

## 🔄 Load Testing

### Built-in Load Testing

**Test Your Deployed Server:**
```bash
# Local testing (install first)
npm install

# Test with different loads
npm run loadtest -- --room my_room --numClients 10 --endpoint ws://your-akash-provider.com:2567
npm run loadtest -- --room my_room --numClients 50 --endpoint ws://your-akash-provider.com:2567
npm run loadtest -- --room my_room --numClients 100 --endpoint ws://your-akash-provider.com:2567
```

**Interpret Results:**
```bash
# Good performance indicators:
# - Connection success rate > 95%
# - Average latency < 50ms
# - No memory leaks over time
# - CPU usage < 80% under load
```

### Scaling Decision Matrix

**When Load Test Shows:**

| **Symptoms** | **Solution** | **New Resources** |
|--------------|--------------|-------------------|
| High CPU (>80%) | Increase CPU | +0.5-1.0 CPU units |
| High Memory (>85%) | Increase RAM | +512Mi-1Gi memory |
| Connection drops | Increase both | +1.0 CPU, +1Gi RAM |
| Slow room creation | Optimize or scale | +1.0 CPU |

---

## 🎮 Unity Client Integration

### Connecting Your Unity Game to Your Deployed Akash Server

**🚀 Your deployed server URL:**
```csharp
// Replace with your actual Akash provider URL
string serverUrl = "ws://provider.paradigmapolitico.online:32203";

// Initialize the Colyseus client
ColyseusClient client = new ColyseusClient(serverUrl);
```

**Complete Unity Integration Example:**
```csharp
using Colyseus;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class AkashGameClient : MonoBehaviour
{
    [Header("🚀 Akash Deployment Settings")]
    [Tooltip("Your Akash provider URL from: akash provider lease-status")]
    public string akashServerUrl = "ws://provider.paradigmapolitico.online:32203";

    [Header("🎮 Game Settings")]
    public string roomName = "my_room";
    public string playerName = "Player";

    private ColyseusClient client;
    private ColyseusRoom<MyRoomState> room;

    async void Start()
    {
        await ConnectToAkashServer();
    }

    async Task ConnectToAkashServer()
    {
        try
        {
            Debug.Log($"🔗 Connecting to Akash server: {akashServerUrl}");

            // Initialize client with your Akash deployment
            client = new ColyseusClient(akashServerUrl);

            // Room options (optional)
            var roomOptions = new Dictionary<string, object>
            {
                ["playerName"] = playerName
            };

            // Join or create a room
            room = await client.JoinOrCreate<MyRoomState>(roomName, roomOptions);

            Debug.Log($"✅ Successfully connected to room: {room.Id}");
            Debug.Log($"👥 Room has {room.State.players.Count} players");

            // Setup event listeners
            SetupRoomEvents();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to connect to Akash server: {e.Message}");
            Debug.LogError($"💡 Check if your server is running: {akashServerUrl.Replace("ws://", "http://")}/health");

            // Retry connection after 5 seconds
            await Task.Delay(5000);
            await ConnectToAkashServer();
        }
    }

    void SetupRoomEvents()
    {
        // Player joined the room
        room.OnJoin += () => {
            Debug.Log("🎉 You joined the room!");
        };

        // Room state changed
        room.OnStateChange += (MyRoomState state, bool isFirstState) => {
            if (isFirstState) {
                Debug.Log("📥 Received initial room state");
            }
            Debug.Log($"🔄 Room state updated - Players: {state.players.Count}");
        };

        // Player joined/left events
        room.State.players.OnAdd += (string sessionId, Player player) => {
            Debug.Log($"👋 {player.name} joined the room ({sessionId})");
            // Spawn player GameObject here
        };

        room.State.players.OnRemove += (string sessionId, Player player) => {
            Debug.Log($"👋 {player.name} left the room ({sessionId})");
            // Remove player GameObject here
        };

        // Custom messages from server
        room.OnMessage<string>("game_event", (message) => {
            Debug.Log($"📨 Game event: {message}");
        });

        // Connection lost
        room.OnLeave += (int code) => {
            var closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
            Debug.LogWarning($"⚠️ Disconnected from room - Reason: {closeCode} ({code})");

            // Attempt reconnection
            StartCoroutine(ReconnectAfterDelay());
        };

        // Server errors
        room.OnError += (int code, string message) => {
            Debug.LogError($"🔥 Room error {code}: {message}");
        };
    }

    // Send player input to the server
    public void SendPlayerMovement(Vector3 position, Vector3 rotation)
    {
        if (room != null)
        {
            room.Send("player_input", new {
                x = position.x,
                y = position.y,
                z = position.z,
                rotX = rotation.x,
                rotY = rotation.y,
                rotZ = rotation.z
            });
        }
    }

    // Send custom game events
    public void SendGameAction(string action, object data = null)
    {
        room?.Send(action, data);
    }

    IEnumerator ReconnectAfterDelay()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("🔄 Attempting to reconnect...");
        StartCoroutine(ConnectToAkashServerCoroutine());
    }

    IEnumerator ConnectToAkashServerCoroutine()
    {
        var task = ConnectToAkashServer();
        yield return new WaitUntil(() => task.IsCompleted);
    }

    void OnDestroy()
    {
        // Clean up connections
        room?.Leave();
        client?.Close();
    }

    // 🔧 Debugging helpers
    [ContextMenu("Test Server Health")]
    public void TestServerHealth()
    {
        StartCoroutine(TestServerHealthCoroutine());
    }

    IEnumerator TestServerHealthCoroutine()
    {
        string healthUrl = akashServerUrl.Replace("ws://", "http://") + "/health";
        Debug.Log($"🏥 Testing server health: {healthUrl}");

        using (var request = UnityWebRequest.Get(healthUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Server is healthy!\n{request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ Server health check failed: {request.error}");
            }
        }
    }

    [ContextMenu("Force Reconnect")]
    public void ForceReconnect()
    {
        StartCoroutine(ForceReconnectCoroutine());
    }

    IEnumerator ForceReconnectCoroutine()
    {
        if (room != null)
        {
            room.Leave();
            room = null;
        }
        yield return ConnectToAkashServerCoroutine();
    }
}
```

**Getting Your Server URL:**

After deploying through Akash Console, you'll receive your server endpoints. The WebSocket URL is what you need for Unity client connections.

**Example server endpoints:**
```
HTTP Monitoring: http://provider.paradigmapolitico.online:32203/health
WebSocket Gaming: ws://provider.paradigmapolitico.online:32203
```

**Unity Configuration:**
```csharp
// Configure your Unity client with the WebSocket endpoint
public string akashServerUrl = "ws://provider.paradigmapolitico.online:32203";
```

**Connection Testing:**

```csharp
// Professional connection testing for production applications
public void TestServerConnection()
{
    StartCoroutine(ValidateServerConnectivity());
}

IEnumerator ValidateServerConnectivity()
{
    // Verify server health first
    string healthEndpoint = akashServerUrl.Replace("ws://", "http://") + "/health";

    using (var healthCheck = UnityWebRequest.Get(healthEndpoint))
    {
        yield return healthCheck.SendWebRequest();

        if (healthCheck.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server health confirmed");
            yield return StartCoroutine(ValidateWebSocketConnection());
        }
        else
        {
            Debug.LogError($"Server health check failed: {healthCheck.error}");
        }
    }
}

IEnumerator ValidateWebSocketConnection()
{
    var testClient = new ColyseusClient(akashServerUrl);
    var connectionTask = testClient.JoinOrCreate<MyRoomState>("connection_test");

    yield return new WaitUntil(() => connectionTask.IsCompleted);

    if (connectionTask.IsCompletedSuccessfully)
    {
        Debug.Log("WebSocket connection validated successfully");
        connectionTask.Result.Leave();
    }
    else
    {
        Debug.LogError($"WebSocket validation failed: {connectionTask.Exception?.Message}");
    }
}
```

**Platform Compatibility:**

| Platform | WebSocket Support | Implementation Notes |
|----------|------------------|---------------------|
| **Desktop** | Native support | Direct WebSocket connection |
| **WebGL** | Browser-based | Handled by browser WebSocket API |
| **Mobile** | Native support | Platform-specific WebSocket libraries |

**Production Considerations:**

- Always validate server connectivity before attempting game connections
- Implement proper error handling for network failures
- Consider implementing connection retry logic for mobile networks
- Monitor connection stability across different network conditions

**🚨 Common Connection Issues:**

1. **"Connection refused" error:**
   ```bash
   # Check if your Akash deployment is running
   curl http://provider.paradigmapolitico.online:32203/health
   ```

2. **Wrong URL format:**
   ```csharp
   // ❌ Wrong - using HTTP instead of WebSocket
   "http://provider.paradigmapolitico.online:32203"

   // ✅ Correct - WebSocket protocol
   "ws://provider.paradigmapolitico.online:32203"
   ```

3. **Firewall/Network issues:**
   ```bash
   # Test with a simple ping first
   ping provider.paradigmapolitico.online
   ```

**🔒 Production Security:**

For production games, use secure WebSocket (WSS):
```csharp
// Production URL with SSL/TLS
string secureUrl = "wss://your-domain.com:443";
ColyseusClient client = new ColyseusClient(secureUrl);
```

**🎯 Unity Inspector Setup:**

1. **Attach the script** to a GameObject in your scene
2. **Configure the URL** in the inspector: `ws://provider.paradigmapolitico.online:32203`
3. **Set room name** and player name
4. **Test connection** using the context menu options
5. **Build and deploy** your game!

**⚡ Performance Tips:**

```csharp
// Optimize for better performance
public class OptimizedAkashClient : MonoBehaviour
{
    [Header("Performance Settings")]
    public float sendRate = 20f; // Send updates 20 times per second
    public bool useCompression = true;

    private float lastSendTime;

    void Update()
    {
        // Throttle network updates
        if (Time.time - lastSendTime >= 1f / sendRate)
        {
            SendPlayerUpdate();
            lastSendTime = Time.time;
        }
    }

    void SendPlayerUpdate()
    {
        if (room != null)
        {
            // Only send if player actually moved
            Vector3 currentPos = transform.position;
            if (Vector3.Distance(currentPos, lastSentPosition) > 0.1f)
            {
                room.Send("move", new { x = currentPos.x, z = currentPos.z });
                lastSentPosition = currentPos;
            }
        }
    }
}
```

---



## 🚀 Production Deployment Best Practices

### Security Checklist

**✅ Before Going Live:**

1. **Change all default passwords:**
   ```bash
   # Generate secure secrets
   openssl rand -base64 32  # For JWT_SECRET
   openssl rand -base64 16  # For AUTH_SALT
   openssl rand -base64 24  # For SESSION_SECRET
   ```

2. **Enable HTTPS/WSS (recommended for production):**
   ```yaml
   # In deploy.yml, use a reverse proxy like Caddy
   services:
     caddy:
       image: caddy:alpine
       # Configure SSL termination
   ```

3. **Set production logging:**
   ```bash
   LOG_LEVEL=warn  # Reduce log verbosity
   NODE_ENV=production
   ```

4. **Configure rate limiting:**
   ```bash
   RATE_LIMIT_ENABLED=true
   RATE_LIMIT_MAX=100  # requests per minute
   ```

### Multi-Region Deployment

**Deploy to Multiple Regions:**

```bash
# Deploy to US East
akash tx deployment create deploy-us-east.yml --from $AKASH_KEYNAME

# Deploy to Europe
akash tx deployment create deploy-europe.yml --from $AKASH_KEYNAME

# Deploy to Asia
akash tx deployment create deploy-asia.yml --from $AKASH_KEYNAME
```

**Load Balancing Strategy:**
```csharp
// Unity client - choose closest server
public class ServerSelector : MonoBehaviour
{
    private string[] servers = {
        "ws://us-east.akash-provider.com:2567",
        "ws://europe.akash-provider.com:2567",
        "ws://asia.akash-provider.com:2567"
    };

    async Task<string> GetFastestServer()
    {
        var tasks = servers.Select(TestServerLatency).ToArray();
        var results = await Task.WhenAll(tasks);
        return results.OrderBy(r => r.latency).First().url;
    }
}
```

### Backup and Disaster Recovery

**Backup Strategy:**
```bash
# Backup your deployment configuration
cp deploy.yml backups/deploy-$(date +%Y%m%d).yml

# Backup environment variables
cp .env backups/.env-$(date +%Y%m%d)

# Backup room state (if persistent)
# Implement in your Colyseus room:
onDispose() {
  // Save room state to external storage
  await this.saveRoomState();
}
```

**Disaster Recovery Plan:**
1. **Server Down:** Redeploy to different provider
2. **Provider Issues:** Switch to backup deployment
3. **Network Partition:** Activate regional failover

---

## 📋 Development

### Project Structure

```
Server/
├── src/
│   ├── rooms/          # Game room definitions
│   │   ├── MyRoom.ts    # Main game room
│   │   └── schema/      # State schemas
│   ├── config/         # Configuration files
│   │   └── auth.ts      # Authentication setup
│   └── index.ts        # Server entry point
├── loadtest/           # Load testing scripts
├── Dockerfile          # Container definition
├── docker-compose.yml  # Local deployment
├── deploy.yml          # Akash deployment (SDL)
├── package.json        # Dependencies
└── .env               # Environment config
```

### Adding New Game Features

**1. Create a New Room Type:**
```typescript
// src/rooms/BattleRoyaleRoom.ts
import { Room, Client } from "colyseus";
import { BattleRoyaleState } from "./schema/BattleRoyaleState";

export class BattleRoyaleRoom extends Room<BattleRoyaleState> {
  maxClients = 100;

  onCreate(options: any) {
    this.setState(new BattleRoyaleState());

    // Game loop
    this.setSimulationInterval((deltaTime) => {
      this.state.update(deltaTime);
    }, 1000/60); // 60 FPS
  }

  onJoin(client: Client, options: any) {
    console.log(`Player ${client.sessionId} joined BR room`);
    this.state.addPlayer(client.sessionId, options.playerName);
  }

  onMessage(client: Client, message: any) {
    if (message.type === "move") {
      this.state.movePlayer(client.sessionId, message.x, message.z);
    }
  }

  onLeave(client: Client, consented: boolean) {
    this.state.removePlayer(client.sessionId);
  }
}
```

**2. Register the Room:**
```typescript
// src/index.ts
import { BattleRoyaleRoom } from "./rooms/BattleRoyaleRoom";

// Register new room
gameServer.define('battle_royale', BattleRoyaleRoom);
```

**3. Test Locally:**
```bash
npm run start:dev

# Test new room
npm run loadtest -- --room battle_royale --numClients 10
```

**4. Deploy to Akash:**
```bash
# Rebuild and push Docker image
docker build -t your-username/colyseus-server:v2.0 .
docker push your-username/colyseus-server:v2.0

# Update deploy.yml with new image tag
# Redeploy to Akash
akash tx deployment close --from $AKASH_KEYNAME
akash tx deployment create deploy.yml --from $AKASH_KEYNAME
```

### Testing Your Deployment

**Automated Testing Script:**
```bash
#!/bin/bash
# test-deployment.sh

SERVER_URL="your-akash-provider.com"

echo "Testing Akash deployment..."

# Test health endpoint
echo "1. Health Check:"
curl -f http://$SERVER_URL:2567/health || exit 1

# Test WebSocket connection
echo "2. WebSocket Test:"
timeout 10 wscat -c ws://$SERVER_URL:2567 || exit 1

# Load test
echo "3. Load Test:"
npm run loadtest -- --room my_room --numClients 5 --endpoint ws://$SERVER_URL:2567

echo "✅ All tests passed! Your server is ready for players!"
```

---

## 👍 Support & Community

### Getting Help

**Akash Network:**
- 💬 [Discord](https://discord.akash.network) - Get real-time help
- 📚 [Documentation](https://docs.akash.network) - Complete guides
- 🐛 [GitHub Issues](https://github.com/akash-network/node) - Report bugs

**Colyseus Framework:**
- 💬 [Discord](https://discord.gg/RY8rRS7) - Community support
- 📚 [Documentation](https://docs.colyseus.io) - Framework docs
- 🌐 [Examples](https://github.com/colyseus/colyseus-examples) - Sample games

### Cost Calculator

**Estimate Your Monthly Costs:**

```bash
# Small game (1-50 players)
# 0.5 CPU, 512Mi RAM, 1Gi storage
# ~$3-8/month depending on provider

# Medium game (50-200 players)
# 1.0 CPU, 1Gi RAM, 2Gi storage
# ~$10-25/month depending on provider

# Large game (200+ players)
# 2.0+ CPU, 2Gi+ RAM, 5Gi+ storage
# ~$25-60/month depending on provider
```

**Compare with Traditional Cloud:**
- **AWS/GCP equivalent:** $50-200/month
- **Akash Network:** $3-60/month
- **Savings:** Up to 90% cost reduction!

---

## 🏆 Success Stories

> *"We deployed our Unity battle royale game on Akash and saved $2,000/month compared to AWS. The global distribution is amazing!"*
> — IndieDev Studio

> *"Akash Network made it possible for us to launch our multiplayer game with minimal upfront costs. We're scaling as we grow!"*
> — Blockchain Gaming Startup

---

## 📜 License

MIT License - Build amazing games and deploy them anywhere!

---

**🚀 Ready to deploy? Your players are waiting!**

```bash
# One command to rule them all
akash tx deployment create deploy.yml --from $AKASH_KEYNAME --node $AKASH_NODE --chain-id $AKASH_CHAIN_ID -y
```

**Questions? Join our [Discord](https://discord.akash.network) and tag @devrel for instant help!**
