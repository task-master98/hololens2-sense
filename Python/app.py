import streamlit as st
import asyncio
import websockets

async def send_command(command):
    try:
        async with websockets.connect("ws://172.20.10.8:8080") as websocket:
            await websocket.send(command)
            print(f"Sent command : {command}")
    except Exception as e:
        print(f"Error sending command: {e}")

def send_command_async(command):
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    loop.run_until_complete(send_command(command))

st.title("Remote Control for HoloLens")

if st.button("Increase Distance"):
    send_command_async("increase_distance")

if st.button("Decrease Distance"):
    send_command_async("decrease_distance")

if st.button("Toggle Left"):
    send_command_async("toggle_left")

if st.button("Toggle Right"):
    send_command_async("toggle_right")

st.subheader("Increase Quad Size")

left_slider = st.slider("Select aspect ratio of Quad", min_value=0.0, max_value=1.0, value=0.5, step=0.01)
st.write(f"Quad Aspect Ratio: {left_slider}")

send_command_async(f"slider_value:{left_slider}")