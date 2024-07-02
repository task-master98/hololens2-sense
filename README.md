# HoloLens 2 Sensor Stream with Ambulatory Projection

This Unity project leverages the Microsoft HoloLens 2 to broadcast data from various sensors onto a main screen. The application includes different UX components such as buttons and sliders to provide visual assistance to visually impaired patients, particularly those suffering from hemianopsia. The hologram moves with the patient, offering continuous guidance.

## Overview

The project captures data from multiple sensors on the HoloLens 2, such as the RGB camera and VLC sensors, and displays this data in a Unity scene. UX components like buttons and sliders are used for interacting with the data. The application is designed to provide visual guidance to patients with visual impairments, such as hemianopsia.

## Features

- **Sensor Data Streaming**: Real-time streaming of RGB camera and VLC sensors data.
- **Distortion Correction**: Corrects lens distortion for accurate visual representation.
- **UX Components**: Includes buttons and sliders for interaction.
- **Ambulatory Projection**: The hologram follows the user, maintaining a fixed distance.
- **Visual Assistance**: Designed to aid visually impaired patients.

## Requirements

- Unity 2020.3 or later
- OpenCV for Unity (available from the Unity Asset Store)
- Mixed Reality Toolkit (MRTK)
- Microsoft HoloLens 2

## Installation

1. **Clone the Repository**:
   ```sh
   git clone https://github.com/task-master98/hololens2-sense.git
2. **Open the Project in Unity**:
    - Launch Unity Hub.
    - Click on "Open" and navigate to the cloned repository's folder.

3. **Install Mixed Reality Toolkit (MRTK)**:

    - Download the MRTK Foundation and SDK packages from the Microsoft Mixed Reality GitHub page.
    - Import the downloaded packages into your Unity project.

4. **Configure XR Settings**:

    - Go to Edit > Project Settings > XR Plug-in Management.
    - Ensure that the Windows Mixed Reality plug-in is installed and enabled for the project.

5. **Set Up HoloLens 2 Capabilities**:

    - Go to Edit > Project Settings > Player.
    - Under Other Settings, set the Scripting Backend to IL2CPP and Target Architectures to ARM64.
    - Under Publishing Settings, select the create app package button and follow the instructions.
