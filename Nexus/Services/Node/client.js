const ws = new WebSocket('ws://localhost:8080');

ws.onopen = () => {
    console.log('Connected to WebSocket server!');
    ws.send('start'); // Example: Start the server
};

ws.onmessage = (event) => {
    console.log(`Server: ${event.data}`);
};

ws.onclose = () => {
    console.log('Disconnected from WebSocket server.');
};
