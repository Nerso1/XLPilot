# XLPilot

![image](https://github.com/user-attachments/assets/d9f6f8ea-91b5-4286-8798-359acff0bdf9)

## Overview

XLPilot is a lightweight utility for managing Comarch ERP XL installations. It provides a centralized dashboard to organize and launch multiple XL configurations, manage paths, and quickly access key system tools. Designed for administrators and power users, XLPilot simplifies routine tasks through customizable shortcut buttons and smart configuration management.

## Features

- **Multi-Installation Management**: Easily switch between different Comarch ERP XL installations
- **Customizable Shortcuts**: Create and organize buttons for frequently used tools and actions
- **XL Configuration**: Manage database connections, license keys, and server settings
- **Environmental Variables**: Configure PATH variables for XL installations
- **Registry Access**: Quick access to relevant registry entries
- **Run-As-Admin**: Launch applications with elevated privileges when needed
- **Drag & Drop Interface**: Intuitively organize your shortcuts

## Screenshots

![image](https://github.com/user-attachments/assets/723ef798-8930-47e7-9aa5-6425125456b5)
![image](https://github.com/user-attachments/assets/9c4b0843-9ec3-43f3-974e-341c9f6514ae)
![image](https://github.com/user-attachments/assets/372de108-16e4-49aa-9d2b-a4665013922e)
![image](https://github.com/user-attachments/assets/de201e2e-ef4c-45ee-8b98-b3c970a8df3d)


## System Requirements

- Windows 8/10/11
- .NET Framework 4.8
- Administrator rights (for some features)
- Comarch ERP XL (for full functionality)

## Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/xlpilot/releases) page
2. Extract the ZIP file to a location of your choice
3. Run `XLPilot.exe`

No installation is required - XLPilot is portable and can be run from any location.

## Usage

### Adding XL Installations

1. Go to the "Konfiguracja XL-e" tab
2. Enter the name and path of your XL installation
3. Optionally specify database, license server, and license key information
4. Click "Add" to save the configuration

### Preparing shortcuts

1. Go to the "Konfiguracja inne" tab for general tools or "Konfiguracja XL-e" for XL-specific tools
2. Drag your choosen standard buttons from "Przyciski do wyboru" panel, to the "Twoje przyciski" panel
3. Additionally you can click the "➕ Stwórz nowy przycisk" button:
3.1. Select the application or folder to launch
3.2. Configure options like "Run as Administrator" as needed
3.3. Click "OK" to add the shortcut

### Launching Applications

1. Go to the "XLPilot" tab
2. Click on any shortcut button to launch the associated application

## Configuration

XLPilot stores its configuration in a `config.xml` file in the same directory as the application. This file contains:

- Saved XL installation paths
- Custom shortcut button configurations
- Other application settings

## Known Issues

- XLPilot requires administrator rights for some operations, such as modifying system PATH variables
- When running ERP XL applications, ensure compatible versions are used

## Acknowledgments

- Icon resources from [icons8](https://icons8.com/)

## Contact

Created by Dominik Ciastoń - [dominik.ciaston@gmail.com]

Project Link: [https://github.com/Nerso1/xlpilot](https://github.com/Nerso1/xlpilot)
