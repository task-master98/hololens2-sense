import asyncio
import websockets
import json

CONNECTED_CLIENTS = set()

async def handler(websocket, path):
    CONNECTED_CLIENTS.add(websocket)
    try:
        async for message in websocket:
            for client in CONNECTED_CLIENTS:
                if client != websocket and client.open:
                    await client.send(message)
    finally:
        CONNECTED_CLIENTS.remove(websocket)

async def main():
    async with websockets.serve(handler, "0.0.0.0", 8080):
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())