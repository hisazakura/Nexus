# Nexus Project
Nexus Project aimed to help Minecraft Developer to deploy their own public server for development using a local Windows machine.
> **_NOTE:_**  This project is still in heavy development and testing. Expect bugs, missing/wrong components, bad configurations, performance issues, etc.

# Lineup
## Nexus Server Panel
A WPF application that integrates core processes. Made to easily start, maintain, and stop processes needed to achieve the main goal.

## Nexus Minecraft Server
A wrapper for Minecraft Server process that provides the necessary communication between Nexus processes.

## Nexus Web Panel
A web application interface that provides developer access to the console.

# Installation
## Prerequisite
- ASP.NET Core Runtime 8
- .NET Desktop Runtime 8
## Bundle
1. Download the bundled app in the [Release](https://github.com/hisazakura/Nexus/releases) tab
2. Executables for Nexus Minecraft Server and Nexus Web Panel are in `/Executables` folder

# Configuration
> **_NOTE:_**  You must be an administrator to configure. I forgot which of these require elevated permission so just be an admin for convenience.
## ngrok Tunnel
1. Make sure you have ngrok installed.
2. Feel free to change the tunnel identifiers although it is recommended to stick with the default.
3. Generate ngrok configuration using the button labeled `Generate ngrok Configuration`.
4. Copy the ngrok configuration and configure [ngrok Agent Configuration File](https://ngrok.com/docs/agent/config/) using the generated configuration.
5. Ensure you have entered your authentication token and save.
6. Click `Start ngrok Tunnel` button to start ngrok Tunnel.

## Minecraft Server
1. Configure `Nexus Server Executable` to the Nexus Minecraft Server executable. If you installed the bundled version, the executable should be in the `/Executables` folder.
2. Configure `Server Location` to your Minecraft Server JAR file.
3. Configure `Arguments` if you require additional server argument such as `Xms` and `Xms`.
4. Click `Start Minecraft Server` button to start Nexus Minecraft Server.
5. `Server logs` are also provides visibility to the console logs and the text box under it are used to send standard input such as commands.

## Web Panel
1. Configure `Web Panel Server Executable` to the Nexus Web Panel executable. If you installed the bundled version, the executable should be in the `/Executables` folder.
2. Click `Start Web Panel` button to start Nexus Web Panel.

## SFTP Server
1. Ensure you have installed SFTP/SSH server on your Windows machine. Refer to [Guide by WinSCP](https://winscp.net/eng/docs/guide_windows_openssh_server) for installation.
2. Optionally, it is highly recommended to create a dedicated non-admin user for accessing Minecraft files. This can be done by setting the chroot of a matched user. Refer to [Guide by Codenotary](https://codenotary.com/blog/openssh-leveraging-the-match-directive).
3. Configure both `Username` and `Password` to properly check SFTP status.

# Contributing
Clone this project and open `Nexus.sln` using Visual Studio.

## Dependencies
- .NET 8
- ASP.NET
- .NET desktop development
