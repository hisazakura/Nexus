import { spawn } from 'child_process';
import { createInterface } from 'readline';
import WebSocket, { WebSocketServer } from 'ws';
import path from 'path';
import fs from 'fs';

/**
 * Separates arguments to fetch server configuration and validates it
 */
export class ServerConfig {
    /**
     * Constructs a new ServerConfig
     * @param {string[]} args - Arguments passed to the script
     */
    constructor(args) {
        this.serverJar = args[0] || './server.jar';
        this.workingDir = path.dirname(this.serverJar);
        this.javaArgs = (args.slice(1).length > 0 ? args.slice(1) : ['-Xmx2G', '-Xms2G'])
            .concat(['-jar', path.basename(this.serverJar), '--nogui']);
    }

    /**
     * Validates the server configuration
     */
    validate() {
        if (!fs.existsSync(this.serverJar)) {
            throw new Error(`Server JAR not found at ${this.serverJar}`);
        }
    }
}

/**
 * Manages Minecraft server process.
 */
export class MinecraftServer {
    /**
     * Constructs a new MinecraftServer with given configuration.
     * @param {ServerConfig} config - Server configuration
     */
    constructor(config) {
        this.config = config;
        this.process = null;
    }

    /**
     * Starts the Minecraft server.
     */
    start() {
        if (this.process) {
            this.log("[Nexus] Server is already running!");
            return;
        }

        this.log("[Nexus] Starting Minecraft server...");
        try {
            this.process = spawn('java', this.config.javaArgs, { cwd: this.config.workingDir, stdio: 'pipe' });
            this.setupProcessListeners();
        } catch (error) {
            this.error(`[Error] Failed to start Minecraft server: ${error.message}`);
        }
    }

    /**
     * Stops the Minecraft server.
     */
    stop() {
        if (!this.process) {
            this.log("[Nexus] Server is not running!");
            return;
        }

        this.log("[Nexus] Stopping Minecraft server...");
        try {
            this.process.stdin.write('stop\n');
        } catch (error) {
            this.error(`[Error] Failed to send stop command to Minecraft server: ${error.message}`);
        }
    }

    setupProcessListeners() {
        this.process.stdout.on('data', (data) => this.log(`${data}`));
        this.process.stderr.on('data', (data) => this.error(`[Error] ${data}`.trim()));
        this.process.on('error', (error) => this.error(`[Error] Minecraft server process error: ${error.message}`));
        this.process.on('close', (code) => {
            this.log(`[Nexus] Minecraft server exited with code ${code}`);
            this.log("[Nexus] Server is not running.");
            this.process = null;
        });
    }

    log(message) {
        console.log(message);
        WebSocketManager.broadcast(message);
    }

    error(message) {
        console.error(message);
        WebSocketManager.broadcast(message);
    }
}

export class WebSocketManager {
    static clients = new Set();

    constructor(port) {
        this.wss = new WebSocketServer({ port }, () => {
            console.log(`WebSocket server started on ws://localhost:${port}`);
        });

        this.setupListeners();
    }

    setupListeners() {
        this.wss.on('connection', (ws) => {
            console.log('New WebSocket client connected!');
            WebSocketManager.clients.add(ws);
            ws.send('Welcome to the Minecraft server controller!');

            ws.on('message', (message) => CommandHandler.handle(ws, message.toString()));
            ws.on('close', () => WebSocketManager.clients.delete(ws));
        });
    }

    static broadcast(message) {
        WebSocketManager.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(message);
            }
        });
    }
}

export class CommandHandler {
    static server = null;

    static initialize(server) {
        CommandHandler.server = server;
    }

    static handle(ws, command) {
        switch (command) {
            case 'start':
                CommandHandler.server.start();
                break;
            case 'stop':
                CommandHandler.server.stop();
                break;
            case 'servercheck':
                ws.send(CommandHandler.server.process ? "[Nexus] Server is running!" : "[Nexus] Server is not running.");
                break;
            default:
                CommandHandler.handleCustomCommand(command);
                break;
        }
    }

    static handleCustomCommand(command) {
        if (CommandHandler.server.process) {
            try {
                CommandHandler.server.process.stdin.write(command + '\n');
                WebSocketManager.broadcast(`[Command] ${command}`);
            } catch (error) {
                CommandHandler.server.error(`[Error] Failed to send command to Minecraft server: ${error.message}`);
            }
        } else {
            const message = "Unknown command or server is not running.";
            console.log(message);
            WebSocketManager.broadcast(message);
        }
    }
}

export class UserInputHandler {
    constructor() {
        this.rl = createInterface({
            input: process.stdin,
            output: process.stdout,
            prompt: '> ',
        });

        this.setupListeners();
    }

    setupListeners() {
        this.rl.prompt();
        this.rl.on('line', (line) => {
            CommandHandler.handle(null, line.trim());
            this.rl.prompt();
        });
    }
}

/**
 * Waits for the user to press any key before executing a callback.
 * @param {Function} callback Function to call after key press.
 */
export function pressAnyKeyToContinue(callback) {
    const readline = createInterface({
        input: process.stdin,
        output: process.stdout,
    });

    console.log("Press any key to continue...");
    process.stdin.setRawMode(true);
    process.stdin.resume();
    process.stdin.once('data', () => {
        process.stdin.setRawMode(false);
        readline.close();
        callback();
    });
}