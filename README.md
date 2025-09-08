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


## Quick Start Guide

## 🎯 What This Repository Provides

- **Colyseus Unity SDK** - Complete multiplayer client for Unity
- **Example Server** - Ready-to-deploy multiplayer server with Docker support
- **Unity Example Project** - Functional demo with connection UI and player movement
- **Akash Network Integration** - Decentralized hosting deployment ready

## ⚡ Quick Local Testing (5 minutes)

### Step 1: Start the Server
```bash
# Navigate to the server directory
cd Server

# Install dependencies (first time only)
npm install

# Start the server
npm run start
```

Server will start on:
- **WebSocket**: `ws://localhost:2567` (for Unity connections)
- **Health Check**: `http://localhost:2567/health`
- **Monitor**: `http://localhost:2567/colyseus`

### Step 2: Test Unity Connection

1. **Open Unity Project**
   - Open this folder in Unity 6 (or Unity 2022.3+)
   - Unity will import the project automatically

2. **Load the Menu Scene**
   - In Project window: `Assets → Colyseus → Runtime → Examples → Scenes → Menu`
   - Double-click `Menu.unity` to load it

3. **Test Connection**
   - Click Play button in Unity
   - Menu scene should show connection UI
   - Default settings: `localhost:2567`
   - Click "Connect" or "Play" to test

4. **Verify Connection**
   - Check Unity Console for connection logs
   - Server terminal should show "Client connected"
   - Monitor interface: `http://localhost:2567/colyseus`

## 🐳 Docker Testing

```bash
# Build and run with Docker
cd Server
docker-compose up --build

# Test connection
curl http://localhost:2567/health
```

## 🌐 Deploy to Akash Network

See [Server/README.md](Server/README.md) for detailed Akash deployment instructions.

## 📁 Project Structure

```
├── Assets/Colyseus/          # Unity SDK
│   ├── Runtime/
│   │   ├── Examples/          # Demo scenes and scripts
│   │   │   ├── Scenes/        # Menu.unity, Game.unity
│   │   │   └── Scripts/       # Connection scripts
│   │   └── Colyseus/          # Core SDK
├── Server/                    # Multiplayer server
│   ├── src/                   # Server source code
│   ├── Dockerfile             # Container config
│   └── README.md              # Deployment guide
```

## 🛠️ Integration into Your Project

### Option 1: Copy SDK Folder
```bash
# Copy the SDK into your Unity project
cp -r Assets/Colyseus /path/to/your/project/Assets/
```

### Option 2: Unity Package Manager
1. Copy the `package.json` from `Assets/Colyseus/`
2. Add via Package Manager → Add package from disk
3. Select the `package.json` file

### Basic Usage
```csharp
using Colyseus;

// Connect to server
ColyseusClient client = new ColyseusClient("ws://localhost:2567");
ColyseusRoom room = await client.JoinOrCreate("my_room");

// Send message
room.Send("playerMove", new { x = 10, y = 5 });
```

## 🔧 Troubleshooting

### "Connection Failed"
- ✅ Ensure server is running: `npm run start` in Server folder
- ✅ Check server logs for errors
- ✅ Verify Unity uses correct address: `localhost:2567`
- ✅ Test health endpoint: `curl http://localhost:2567/health`

### "Scene Only Shows Camera"
- ✅ Load correct scene: `Assets/Colyseus/Runtime/Examples/Scenes/Menu.unity`
- ✅ Click Play button in Unity Editor
- ✅ Check Unity Console for script errors

### "Endpoints Not Working"
- ✅ All endpoints use port 2567 (NOT port 80)
- ✅ Health: `http://localhost:2567/health`
- ✅ Monitor: `http://localhost:2567/colyseus`
- ✅ Server info: `http://localhost:2567/`

## 📖 Documentation

- [Server Deployment Guide](Server/README.md)
- [Unity Integration Guide](Assets/Colyseus/Documentation~/GettingStarted.md)
- [Official Colyseus Docs](https://docs.colyseus.io/)

## 🎮 Platform Support

- ✅ WebGL
- ✅ iOS  
- ✅ Android
- ✅ macOS
- ✅ Windows
- ✅ Linux

## License

MIT
