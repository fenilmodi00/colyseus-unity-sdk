# Colyseus Unity SDK Server

A multiplayer game server for Unity clients using the Colyseus framework. This server provides real-time multiplayer functionality with WebSocket communication, optimized for cloud deployment.

## Features

- Real-time multiplayer connectivity
- Room management and matchmaking
- State synchronization
- WebSocket communication (port 2567)
- HTTP monitoring endpoints (port 2567)
- Health checks and monitoring
- Docker containerization ready
- Cloud deployment optimized (Akash Network compatible)

## Quick Start

### Prerequisites

- Node.js 18+
- Docker and Docker Compose
- npm or yarn

### Local Development

1. **Install dependencies:**
```bash
npm install
```

2. **Configure environment:**
```bash
# Copy and edit the .env file
cp .env.example .env
# Edit .env with your configuration
```

3. **Start development server:**
```bash
npm run start:dev
```

### Docker Deployment

#### Using Docker Compose (Recommended)

1. **Build and start:**
```bash
docker-compose up --build
```

2. **Run in background:**
```bash
docker-compose up -d --build
```

3. **Stop the server:**
```bash
docker-compose down
```

#### Using Docker Commands

1. **Build the image:**
```bash
docker build -t colyseus-server .
```

2. **Run the container:**
```bash
docker run -p 80:80 -p 2567:2567 --env-file .env colyseus-server
```

3. **Run with custom environment:**
```bash
docker run -p 80:80 -p 2567:2567 \
  -e NODE_ENV=production \
  -e PORT=80 \
  -e WS_PORT=2567 \
  colyseus-server
```

## Configuration

### Environment Variables

Edit the `.env` file to configure your server:

```bash
# Server Configuration
NODE_ENV=production
PORT=80                    # HTTP port
WS_PORT=2567              # WebSocket port

# Security
JWT_SECRET=your-secret-key
AUTH_SALT=your-auth-salt
SESSION_SECRET=your-session-secret
COLYSEUS_MONITOR_PASSWORD=admin-password

# Performance
NODE_OPTIONS=--max-old-space-size=512 --optimize-for-size
UV_THREADPOOL_SIZE=4
LOG_LEVEL=info
```

## Endpoints

### WebSocket
- **ws://localhost:2567** - Game client connections

### HTTP
- **http://localhost:2567/health** - Health check
- **http://localhost:2567/colyseus** - Colyseus monitor (if enabled)


## Load Testing

Test your server performance:

```bash
# Built-in load test
npm run loadtest -- --room my_room --numClients 50 --endpoint ws://localhost:2567

# Custom endpoint
npm run loadtest -- --room my_room --numClients 100 --endpoint ws://your-server:2567
```

## Monitoring

### Health Checks

The server includes built-in health checks:

```bash
# Check server health
curl http://localhost:2567/health

# Response
{
  "status": "healthy",
  "timestamp": "2023-...",
  "uptime": 3600,
  "rooms": 5
}
```

### Docker Health Checks

Docker automatically monitors container health:

```bash
# Check container health
docker ps
# Look for "healthy" status
```

## Troubleshooting

### Common Issues

1. **Port conflicts:**
```bash
# Check what's using the port
netstat -tulpn | grep :2567
# Kill the process or change port in .env
```

2. **Permission errors:**
```bash
# Fix file permissions
sudo chown -R $USER:$USER .
```

3. **Docker build issues:**
```bash
# Clean Docker cache
docker system prune -a
# Rebuild without cache
docker-compose build --no-cache
```

### Logs

View server logs:

```bash
# Docker Compose logs
docker-compose logs -f

# Container logs
docker logs colyseus-server -f

# Local development
npm run start:dev
```

## Development

### Project Structure

```
Server/
├── src/
│   ├── rooms/          # Game room definitions
│   ├── config/         # Configuration files
│   └── index.ts        # Server entry point
├── loadtest/           # Load testing scripts
├── Dockerfile          # Container definition
├── docker-compose.yml  # Local deployment
├── package.json        # Dependencies
└── .env               # Environment config
```

### Adding New Rooms

1. Create room class in `src/rooms/`
2. Register room in `src/index.ts`
3. Update schema if needed
4. Test with Unity client

## Unity Client Integration

Connect your Unity client to this server:

```csharp
// Unity C# example
using Colyseus;

ColyseusClient client = new ColyseusClient("ws://localhost:2567");
ColyseusRoom<MyRoomState> room = await client.JoinOrCreate<MyRoomState>("my_room");
```

## Security

- Change default passwords in `.env`
- Use HTTPS/WSS in production
- Enable authentication if needed
- Regular security updates
- Monitor for suspicious activity

## License

MIT
