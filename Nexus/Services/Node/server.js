import { 
    ServerConfig, 
    MinecraftServer, 
    CommandHandler, 
    WebSocketManager, 
    UserInputHandler, 
    pressAnyKeyToContinue } from './lib.js';

try {
    const config = new ServerConfig(process.argv.slice(2));
    config.validate();

    const server = new MinecraftServer(config);
    CommandHandler.initialize(server);

    new WebSocketManager(8080);
    new UserInputHandler();

    process.on('SIGINT', () => {
        if (server.process) {
            server.stop();
            setTimeout(() => pressAnyKeyToContinue(() => process.exit(0)), 3000);
        } else {
            pressAnyKeyToContinue(() => process.exit(0));
        }
    });
} catch (error) {
    console.error(`[Error] ${error.message}`);
    pressAnyKeyToContinue(() => process.exit(1));
}

