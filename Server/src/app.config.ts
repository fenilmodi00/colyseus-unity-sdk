import config from "@colyseus/tools";

import { WebSocketTransport } from "@colyseus/ws-transport";
import { monitor } from "@colyseus/monitor";
import { playground } from "@colyseus/playground";

// import { RedisDriver } from "@colyseus/redis-driver";
// import { RedisPresence } from "@colyseus/redis-presence";

/**
 * Import your Room files
 */
import { MyRoom } from "./rooms/MyRoom";
import auth from "./config/auth";
import { LobbyRoom } from "colyseus";

export default config({
    options: {
        // devMode: true,
        // driver: new RedisDriver(),
        // presence: new RedisPresence(),
    },

    initializeTransport: (options) => new WebSocketTransport(options),

    initializeGameServer: (gameServer) => {
        /**
         * Define your room handlers:
         */
        gameServer.define('my_room', MyRoom);

        gameServer.define('lobby', LobbyRoom);
    },

    initializeExpress: (app) => {
        /**
         * Health check endpoint for container orchestration
         * Essential for Akash Network deployment monitoring
         */
        app.get("/health", (req, res) => {
            res.status(200).json({
                status: "healthy",
                timestamp: new Date().toISOString(),
                uptime: process.uptime(),
                memory: process.memoryUsage(),
                environment: process.env.NODE_ENV || "development",
                ports: {
                    http: process.env.PORT || 80,
                    websocket: process.env.WS_PORT || 2567
                }
            });
        });

        /**
         * Root endpoint with instance information
         */
        app.get("/", (req, res) => {
            res.json({
                service: "Colyseus Unity Server",
                version: "0.16.0",
                instance: process.env.NODE_APP_INSTANCE ?? "NONE",
                timestamp: new Date().toISOString(),
                endpoints: {
                    health: "/health",
                    monitor: "/colyseus",
                    playground: "/playground"
                }
            });
        });

        /**
         * Metrics endpoint for monitoring
         */
        app.get("/metrics", (req, res) => {
            const gameServer = req.app.locals.gameServer;
            res.json({
                rooms: gameServer?.rooms?.size || 0,
                connections: Array.from(gameServer?.rooms?.values() || [])
                    .reduce((total, room: any) => total + (room.clients?.length || 0), 0),
                uptime: process.uptime(),
                memory: process.memoryUsage(),
                cpu: process.cpuUsage()
            });
        });

        /**
         * Bind @colyseus/monitor
         * Protected with password in production
         */
        const monitorOptions: any = process.env.NODE_ENV === 'production' ? {
            auth: {
                login: process.env.MONITOR_USER || 'admin',
                password: process.env.MONITOR_PASSWORD || 'admin123'
            }
        } : {};

        app.use("/colyseus", monitor(monitorOptions));

        // Bind "playground" (disable in production for security)
        if (process.env.NODE_ENV !== 'production') {
            app.use("/playground", playground());
        }

        // Bind auth routes
        app.use(auth.prefix, auth.routes());
    },


    beforeListen: () => {
        /**
         * Before before gameServer.listen() is called.
         */
    }
});
