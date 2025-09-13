/**
 * Colyseus Unity SDK Server - Akash Network Optimized
 *
 * IMPORTANT:
 * ---------
 * Configured for deployment on Akash Network with HTTP/TCP port support
 *
 * If you're self-hosting (without Arena), you can manually instantiate a
 * Colyseus Server as documented here: 👉 https://docs.colyseus.io/server/api/#constructor-options
 */
import { listen } from "@colyseus/tools";

// Import arena config
import app from "./app.config";

// Configure ports for Akash Network deployment
const httpPort = process.env.PORT || 8080;
const wsPort = process.env.WS_PORT || 2567;

console.log(`🚀 Starting Colyseus server...`);
console.log(`📡 Environment: ${process.env.NODE_ENV || 'development'}`);
console.log(`🌐 HTTP Port: ${httpPort}`);
console.log(`🔗 WebSocket Port: ${wsPort}`);
console.log(`🎮 Ready for Unity clients!`);

// Use the listen function from @colyseus/tools which properly handles the app config
listen(app, Number(wsPort));
