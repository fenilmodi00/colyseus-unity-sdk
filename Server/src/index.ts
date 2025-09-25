/**
 * Colyseus Unity SDK Server - Akash Network Optimized
 * Single port configuration for Akash Network compatibility
 */
import { listen } from "@colyseus/tools";
import app from "./app.config";

// Akash Network: Use single port for both HTTP and WebSocket
const port = Number(process.env.WS_PORT) || 2567;

console.log(`🚀 Starting Colyseus server for Akash Network...`);
console.log(`📡 Environment: ${process.env.NODE_ENV || 'development'}`);
console.log(`🌐 Server Port: ${port} (HTTP + WebSocket)`);
console.log(`🎮 Ready for Unity clients!`);
console.log(`📊 Health Check: http://localhost:${port}/health`);

// Start server on single port (Akash compatible)
listen(app, port);
