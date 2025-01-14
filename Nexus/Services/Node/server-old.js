import { spawn } from 'child_process';
import { createInterface } from 'readline';
import WebSocket, { WebSocketServer } from 'ws';
import path from 'path';
import fs from 'fs';

const args = process.argv.slice(2);
const SERVER_JAR = args[0] || './server.jar';
const WORKING_DIR = path.dirname(SERVER_JAR);
const JAVA_ARGS = (args.slice(1).length > 0 ? args.slice(1) : ['-Xmx2G', '-Xms2G'])
    .concat(['-jar', path.basename(SERVER_JAR), '--nogui']);

let serverProcess = null;
let wss;

// Validate if SERVER_JAR exists
if (!fs.existsSync(SERVER_JAR)) {
    console.error(`[Error] Server JAR not found at ${SERVER_JAR}`);
    pressAnyKeyToContinue(() => process.exit(1));
}

// Initialize WebSocket Server with error handling
try {
    wss = new WebSocketServer({ port: 8080 }, () => {
        console.log('WebSocket server started on ws://localhost:8080');
    });
} catch (error) {
    console.error(`[Error] Failed to start WebSocket server: ${error.message}`);
    pressAnyKeyToContinue(() => process.exit(1));
}

function broadcast(message) {
    wss.clients.forEach((client) => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(message);
        }
    });
}

function log(message) {
    console.log(message);
    broadcast(message);
}

function err(message) {
    console.error(message);
    broadcast(message);
}

function startServer() {
    if (serverProcess) {
        log("[Nexus] Server is already running!");
        return;
    }

    log("[Nexus] Starting Minecraft server...");
    try {
        serverProcess = spawn('java', JAVA_ARGS, { cwd: WORKING_DIR, stdio: 'pipe' });
    } catch (error) {
        err(`[Error] Failed to start Minecraft server: ${error.message}`);
        return;
    }

    serverProcess.stdout.on('data', (data) => {
        log(`${data}`);
    });

    serverProcess.stderr.on('data', (data) => {
        const message = `[Error] ${data}`.trim();
        err(message);
    });

    serverProcess.on('error', (error) => {
        err(`[Error] Minecraft server process error: ${error.message}`);
    });

    serverProcess.on('close', (code) => {
        log(`[Nexus] Minecraft server exited with code ${code}`);
        log("[Nexus] Server is not running.");
        serverProcess = null;
    });
}

function stopServer() {
    if (!serverProcess) {
        log("[Nexus] Server is not running!");
        return;
    }

    log("[Nexus] Stopping Minecraft server...");
    try {
        serverProcess.stdin.write('stop\n');
    } catch (error) {
        err(`[Error] Failed to send stop command to Minecraft server: ${error.message}`);
    }
}

const rl = createInterface({
    input: process.stdin,
    output: process.stdout,
    prompt: '> ',
});

rl.prompt();
rl.on('line', (line) => {
    handleUserCommand(line.trim());
    rl.prompt();
});

function handleBackendCommand(ws, command) {
    switch (command) {
        case 'servercheck':
            if (serverProcess) {
                ws.send("[Nexus] Server is running!");
            } else {
                ws.send("[Nexus] Server is not running.");
            }
            return true;
        default:
            break;
    }
}

function handleUserCommand(command) {
    switch (command) {
        case 'start':
            startServer();
            break;
        case 'stop':
            stopServer();
            break;
        default:
            if (serverProcess) {
                try {
                    serverProcess.stdin.write(command + '\n');
                    broadcast(`[Command] ${command}`);
                } catch (error) {
                    err(`[Error] Failed to send command to Minecraft server: ${error.message}`);
                }
            } else {
                const message = "Unknown command or server is not running.";
                console.log(message);
                broadcast(message);
            }
    }
}

wss.on('connection', (ws) => {
    console.log('New WebSocket client connected!');
    ws.send('Welcome to the Minecraft server controller!');

    ws.on('message', (message) => {
        if (!handleBackendCommand(ws, message.toString()))
            handleUserCommand(message.toString());
    });

    ws.on('close', () => {
        console.log('WebSocket client disconnected.');
    });
});

process.on('SIGINT', () => {
    if (serverProcess) {
        stopServer();
        setTimeout(() => pressAnyKeyToContinue(() => process.exit(0)), 3000);
    } else {
        pressAnyKeyToContinue(() => process.exit(0));
    }
});

/**
 * Waits for the user to press any key before executing a callback.
 * @param {Function} callback Function to call after key press.
 */
function pressAnyKeyToContinue(callback) {
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
