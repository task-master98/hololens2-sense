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

6. **Navigate to the Device Portal**
    - Switch on the HoloLens and unlock it with your PIN. Make sure it is connected to the Wifi. [Note: this should usually be a home-Wifi or your phone hotspot as they do not block such IP addresses].
    - Once the device is connected, the next step is to obtain a connection to the device portal. For this we need the IP address of the HoloLens. Simply speak into the device `What's my IP?`.
    - This should open up a small window which gives the IP address. All IP addresses are a set of 4 numbers separated by dots (.) All the numbers are
    between 0 to 255. For example, `173.XXX.XXX.X`. Please make a note of this IP address.
    - On your desktop or laptop which is also connected to the same network as your HoloLens device, open a browser such as `Google Chrome` and enter the IP address. This should direct you to the following screen.
    - ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/DevicePortalWarning.png)
    - Simply click on `Advanced` and this will bring you the following webpage ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/AdvancedSettingsWarning.png)
    - This will prompt you to enter the user credentials. Please enter them as you have set them up and it would navigate to the `Windows Device Portal Home` ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/DevicePortalHome.png)
    - Finally click on `Views` > `Apps` which opens up the applications window ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/DevicePortal.png). Click on `Choose File` in the applications window.

7. **App Package Installation**
    - The app package will be prepared by the developer and shipped off. Given below is an organization of the app package folder. ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/AppPackageFolder.JPG)
    - Click on the given folder named `hl2da_unity_v2_<VERSION>` and navigate to the `.appx` file as highlighted in the image above.
    - If this application is being installed for the first time, we also need to install the dependencies. Simply click on `Install additional dependencies` once the `.appx` file has been selected. This will again prompt you to open the app package folder. Navigate to the dependencies folder which is highlighted in this image ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/Dependencies-1.JPG)
    - Next navigate to the required architecture which for the HoloLens device is ARM64 ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/Dependencies-2.JPG)
    - Select the dependency file and click `open`, ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/Dependencies-3.JPG)
    - Finally click `install` and wait until the progress bar is completely filled and says `Done`. Now the application has been successfully installed on the HoloLens.

8. **Open the Application**
    - Navigate to the application in the HoloLens and click it. It should be named `hl2da_unity_v2`.
    - Once it's open, the user should see this on startup, ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/AppStartup.jpg)
    - The user can move around and use the application as intended. Using the UI components like the button and the slider should manipulate the different configurations of the app such as the distance and size of each window.
    - ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/LeftViewMin.jpg)
    - ![Image](https://github.com/task-master98/hololens2-sense/blob/version-frame_rate_fix/Images/LeftViewMax.jpg)

