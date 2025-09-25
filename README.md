<div align="center">
  <a href="https://github.com/colyseus/colyseus">
    <img src="https://github.com/colyseus/colyseus/blob/master/media/logo.svg?raw=true" width="40%" height="100" />
  </a>
  <br>
  <br>
  <a href="https://npmjs.com/package/colyseus">
    <img src="https://img.shields.io/npm/dm/colyseus.svg?style=for-the-badge&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAQAAAC1+jfqAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAAHdElNRQfjAgETESWYxR33AAAAtElEQVQoz4WQMQrCQBRE38Z0QoTcwF4Qg1h4BO0sxGOk80iCtViksrIQRRBTewWxMI1mbELYjYu+4rPMDPtn12ChMT3gavb4US5Jym0tcBIta3oDHv4Gwmr7nC4QAxBrCdzM2q6XqUnm9m9r59h7Rc0n2pFv24k4ttGMUXW+sGELTJjSr7QDKuqLS6UKFChVWWuFkZw9Z2AAvAirKT+JTlppIRnd6XgaP4goefI2Shj++OnjB3tBmHYK8z9zAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDE5LTAyLTAxVDE4OjE3OjM3KzAxOjAwGQQixQAAACV0RVh0ZGF0ZTptb2RpZnkAMjAxOS0wMi0wMVQxODoxNzozNyswMTowMGhZmnkAAAAZdEVYdFNvZnR3YXJlAHd3dy5pbmtzY2FwZS5vcmeb7jwaAAAAAElFTkSuQmCC">
  </a>
  <a href="https://discuss.colyseus.io" title="Discuss on Forum">
    <img src="https://img.shields.io/badge/discuss-on%20forum-brightgreen.svg?style=for-the-badge&colorB=0069b8&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAQAAAC1+jfqAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAAHdElNRQfjAgETDROxCNUzAAABB0lEQVQoz4WRvyvEARjGP193CnWRH+dHQmGwKZtFGcSmxHAL400GN95ktIpV2dzlLzDJgsGgGNRdDAzoQueS/PgY3HXHyT3T+/Y87/s89UANBKXBdoZo5J6L4K1K5ZxHfnjnlQUf3bKvkgy57a0r9hS3cXfMO1kWJMza++tj3Ac7/LY343x1NA9cNmYMwnSS/SP8JVFuSJmr44iFqvtmpjhmhBCrOOazCesq6H4P3bPBjFoIBydOk2bUA17I080Es+wSZ51B4DIA2zgjSpYcEe44Js01G0XjRcCU+y4ZMrDeLmfc9EnVd5M/o0VMeu6nJZxWJivLmhyw1WHTvrr2b4+2OFqra+ALwouTMDcqmjMAAAAldEVYdGRhdGU6Y3JlYXRlADIwMTktMDItMDFUMTg6MTM6MTkrMDE6MDAC9f6fAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE5LTAyLTAxVDE4OjEzOjE5KzAxOjAwc6hGIwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAAASUVORK5CYII=" alt="Discussion forum" />
  </a>
  <a href="https://discord.gg/RY8rRS7">
    <img src="https://img.shields.io/discord/525739117951320081.svg?style=for-the-badge&colorB=7581dc&logo=discord&logoColor=white">
  </a>
  <h3>
     Multiplayer Game Client for <a href="https://unity3d.com/">Unity</a>. <br /><a href="https://docs.colyseus.io/getting-started/unity">View documentation</a>
  </h3>
</div>

---

## 📋 Table of Contents

- [🚀 Quick Start](#-quick-start)
- [✨ Features](#-features)
- [📦 Installation](#-installation)
- [🎯 Usage](#-usage)
- [🐳 Docker Deployment](#-docker-deployment)
- [☁️ Akash Network](#️-akash-network)
- [🔧 Configuration](#-configuration)
- [🎮 Unity Setup](#-unity-setup)
- [📚 Examples](#-examples)
- [📄 License](#-license)

---

## 🚀 Quick Start

Get your multiplayer game running in **under 5 minutes**:

### 1️⃣ Start the Server
```
cd Server
npm install
npm start
```
✅ Server running at `ws://localhost:2567`

### 2️⃣ Open Unity
1. Open the project in **Unity 2022.3+**
2. Load `Assets/Colyseus/Runtime/Examples/Scenes/Menu.unity`
3. Click **Play** → Use default settings → Click **Play**

### 3️⃣ Test Multiplayer
- Open multiple Unity Editor windows
- Connect and see synchronized movement!

> **🎉 That's it!** You now have a working multiplayer game.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🔄 **Real-time Sync** | WebSocket-based state synchronization |
| 🎮 **Unity Ready** | Pre-built Unity scenes and scripts |
| 🐳 **Docker Optimized** | Single-command deployment |
| ☁️ **Akash Compatible** | Decentralized cloud deployment |
| 🔒 **Production Ready** | Health checks, monitoring, logging |
| 📱 **Cross-Platform** | WebGL, Mobile, Desktop support |
| ⚡ **Low Latency** | Optimized for real-time gaming |

---

## 📦 Installation

### Option 1: Clone Repository (Recommended)
```
git clone https://github.com/fenilmodi00/colyseus-unity-sdk.git
cd colyseus-unity-sdk
```

### Option 2: Unity Package Manager
1. Open Unity → **Window** → **Package Manager**
2. Click **+** → **Add package from git URL**
3. Enter: `https://github.com/fenilmodi00/colyseus-unity-sdk.git`

---

## 🎯 Usage

### Basic Connection
```
using Colyseus;

// Connect to server
ColyseusClient client = new ColyseusClient("ws://localhost:2567");

// Join or create room
ColyseusRoom<MyRoomState> room = await client.JoinOrCreate<MyRoomState>("my_room");

// Listen for state changes
room.OnStateChange += (state, isFirstState) => {
    Debug.Log("Room state updated!");
};

// Send player position
room.Send("position", new { x = 10, y = 5 });
```

### Connection Presets

| Environment | Host | Port | Secure |
|-------------|------|------|--------|
| **Local Dev** | `localhost` | `2567` | `false` |
| **Akash Network** | `provider.akash.com` | `30986` | `false` |
| **Production** | `yourdomain.com` | `443` | `true` |

---

## 🐳 Docker Deployment

### Local Docker
```
cd Server
docker build -t colyseus-unity .
docker run -p 2567:2567 colyseus-unity
```

### Docker Compose
```
docker-compose up -d
```

### Health Check
```
curl http://localhost:2567/health
```

---

## ☁️ Akash Network

Deploy to decentralized cloud using console.akash.network:

### 1️⃣ Deploy via Console
1. Go to [console.akash.network](https://console.akash.network)
2. Click "Deploy Now"
3. Copy the entire content of `Server/deploy.yaml`
4. Paste into the SDL (Service Definition Language) section
6. Click "Create Deployment"

### 2️⃣ Set Environment Variables
When prompted, set these environment variables with secure values:
- `JWT_SECRET`: A strong, random string (32+ characters)
- `COLYSEUS_MONITOR_PASSWORD`: A secure password for admin access

### 3️⃣ Get Connection Details
After deployment:
1. Wait for the lease to be created
2. Find your provider URL (e.g., `provider.europlots.com`)
3. Note the external port assigned (e.g., `30986`)

### ⚙️ Unity Configuration for Akash
Update your Unity client connection:
```
// Use your provider URL and external port
private static string hostname = "your-provider-url.com";  // e.g., provider.europlots.com
private static string port = "your-external-port";        // e.g., 30986 (NOT 2567)
private static bool secureProtocol = false;               // Use false for HTTP
```



> **📝 Important:** Enable **"Allow downloads over HTTP"** in Unity Player Settings

---

## 🔧 Configuration

### Environment Variables
```
# Server/.env
NODE_ENV=production
WS_PORT=2567
JWT_SECRET=your-secure-secret
COLYSEUS_MONITOR_PASSWORD=your-admin-password
```

### Unity Project Settings
1. **Edit** → **Project Settings** → **Player**
2. **Other Settings** → **Configuration**
3. Set **"Allow downloads over HTTP"** → **Always allow**

---

## 🎮 Unity Setup

### Scene Setup
1. Add both scenes to **Build Settings**:
   - `Assets/Colyseus/Runtime/Examples/Scenes/Menu.unity`
   - `Assets/Colyseus/Runtime/Examples/Scenes/Game.unity`

### Scripts Included
- **`MenuManager.cs`** - Connection UI and settings
- **`NetworkManager.cs`** - Colyseus client management
- **`PlayerMovement.cs`** - Synchronized player movement
- **`MyRoomState.cs`** - Room state schema

### Testing Multiplayer
- **Unity 6+**: Use **Multiplayer Play Mode**
- **Older Unity**: Use [ParrelSync](https://github.com/VeriorPies/ParrelSync) to clone project

---

## 📚 Examples

### Custom Room Logic
```
// Server/src/rooms/MyRoom.ts
export class MyRoom extends Room<MyRoomState> {
  onCreate(options: any) {
    this.setState(new MyRoomState());

    this.onMessage("position", (client, data) => {
      const player = this.state.players.get(client.sessionId);
      player.x = data.x;
      player.y = data.y;
    });
  }
}
```

### Unity State Handling
```
void SetupStateListeners() {
    var callbacks = Colyseus.Schema.Callbacks.Get(room);

    // Player joined
    callbacks.OnAdd(state => state.players, (key, player) => {
        SpawnPlayer(key, player);
    });

    // Player moved
    callbacks.OnChange(player, (changes) => {
        UpdatePlayerPosition(player);
    });
}
```

---

## 🚨 Troubleshooting

| Issue | Solution |
|-------|----------|
| **Connection Failed** | Check server is running: `curl http://localhost:2567/health` |
| **Unity HTTP Error** | Enable "Allow HTTP" in Player Settings |
| **Akash Port Issues** | Use external port from lease (e.g., `30986`, not `2567`) |
| **Docker Build Fails** | Ensure Docker daemon is running |

### Common Connection Errors
```
WebSocketException: Unable to connect
```
**Fix:** Verify server URL and ensure server is running

```
HTTP error 'Insecure connection not allowed'
```
**Fix:** Enable HTTP downloads in Unity Player Settings

---

## 📊 Project Stats

- **Unity Version:** 2022.3+
- **Colyseus Version:** 0.16
- **Docker Image Size:** ~120MB
- **Deployment Time:** < 5 minutes
- **Platform Support:** Windows, macOS, Linux, WebGL, Mobile

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [Colyseus](https://colyseus.io/) - Multiplayer framework
- [Unity Technologies](https://unity.com/) - Game engine
- [Akash Network](https://akash.network/) - Decentralized cloud
- [Docker](https://www.docker.com/) - Containerization

---


