import streamlit as st
import asyncio
import websockets

async def send_command(command):
    async with websockets.connect("ws://192.168.137.1:8080") as websocket:
        await websocket.send(command)

def send_command_async(command):
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    loop.run_until_complete(send_command(command))

st.title("Remote Control for HoloLens")

if st.button("Increase Distance"):
    send_command_async("increase_distance")

if st.button("Decrease Distance"):
    send_command_async("decrease_distance")
