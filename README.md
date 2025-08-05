# WireGuard VPN Configuration Generator

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A comprehensive WPF desktop application designed for network administrators to efficiently manage WireGuard VPN configurations across multiple servers. This tool streamlines the process of generating, tracking, and exporting WireGuard client configurations while maintaining separate profiles for different server environments.

![Application Screenshot](screenshot.png)

## ✨ Key Features

### 🖥️ **Multi-Server Management**
- Create and manage multiple WireGuard server profiles
- Switch between different server environments seamlessly
- Each profile maintains its own client database and settings

### ⚙️ **Automated Configuration Generation**
- Generate complete WireGuard client configuration files
- Create corresponding server-side peer configurations
- Automatic private key generation for new clients
- Smart IP address assignment with conflict prevention

### 📊 **Comprehensive Tracking**
- Maintain detailed history of all generated configurations
- Track client names, public keys, and assigned IP addresses
- Separate configuration databases per server profile
- Sortable configuration history with timestamps

### 🔧 **Advanced Profile Management**
- Customizable server settings per profile
- Configure subnets, DNS servers, and network parameters
- Import/export profile configurations
- Profile validation and error handling

### 📁 **Export & File Management**
- Export client configurations as `.conf` files
- Export server peer configurations for easy integration
- Organized file structure with timestamp-based naming
- Batch export capabilities

## 🚀 Getting Started

### Prerequisites

- **Operating System**: Windows 10/11
- **.NET Runtime**: .NET 9.0 or later
- **Hardware**: Minimum 4GB RAM, 100MB disk space

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/HawkTaka/WireGuard-VPN-Configuration-Generator.git
   cd wireguard-config-generator
   ```

2. **Build the application**
   ```bash
   dotnet build --configuration Release
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

### First-Time Setup

When you first launch the application, you'll be prompted to create your initial server profile:

1. **Profile Configuration Window** opens automatically
2. **Configure your WireGuard server**:
   - Profile name (e.g., "Production Server", "Office VPN")
   - Server public key
   - Server endpoint (IP address or domain)
   - Network settings (subnet, DNS, port)
3. **Save the profile** to begin generating client configurations

## 📖 Usage Guide

### 🏠 **Main Interface Overview**

The application features a clean, intuitive interface divided into four main sections:

1. **Server Profile Bar** - Select and manage server profiles
2. **Configuration Input Panel** - Enter client details
3. **Generated Configurations** - View client and server configs
4. **Configuration History** - Browse previous generations

### 👥 **Managing Server Profiles**

#### Creating a New Profile
1. Click **"New"** in the profile section
2. Configure server settings:
   - **Profile Name**: Descriptive name for your server
   - **Server Public Key**: Your WireGuard server's public key
   - **Server Endpoint**: IP address or hostname
   - **Server Port**: Default 51820, customize if needed
   - **Subnet**: IP range for clients (e.g., `10.0.0.0/24`)
   - **DNS Server**: Default `1.1.1.1`, customize as needed
   - **Allowed IPs**: Networks clients can access
   - **Persistent Keepalive**: Connection maintenance interval

#### Profile Operations
- **Save**: Store changes to current profile
- **Load**: Edit detailed profile settings
- **Delete**: Remove profiles (minimum one required)

### 🔧 **Generating Client Configurations**

#### Step-by-Step Process
1. **Select Server Profile** from the dropdown
2. **Enter Client Details**:
   - Client name (identifier)
   - Client's public key (provided by the client)
3. **IP Assignment**: Automatically assigned or manually specify
4. **Generate**: Click "Generate Configuration"

#### Configuration Output
- **Client Configuration**: Complete `.conf` file for the client
- **Server Peer Configuration**: Peer block to add to your server

### 💾 **Export and File Management**

#### Export Options
- **Export Client Config**: Save client configuration as `.conf` file
- **Export Server Config**: Save peer configuration for server integration

#### File Organization
```
Data/Exports/
├── ClientName_20241205_143022.conf     # Client configuration
└── peer_ClientName_20241205_143022.conf # Server peer configuration
```

### 📊 **Configuration History**

The history panel displays:
- **Generation timestamp**
- **Client name**
- **Assigned IP address**
- **Client public key**

Use the **Refresh** button to update the history display.

## 🏗️ Architecture & File Structure

### Project Structure
```
VPN Config Gen/
├── 📁 Models/                    # Data models and entities
│   ├── ServerProfile.cs         # Server profile configuration
│   ├── VPNConfiguration.cs      # VPN configuration data
│   └── ConfigurationRecord.cs   # Historical record model
├── 📁 ViewModels/               # MVVM ViewModels
│   ├── MainViewModel.cs         # Main application logic
│   ├── BaseViewModel.cs         # Base MVVM implementation
│   └── RelayCommand.cs          # Command implementation
├── 📁 Views/                    # UI Components
│   ├── MainWindow.xaml          # Main application window
│   └── ProfileWindow.xaml       # Profile configuration dialog
├── 📁 Services/                 # Business logic services
│   ├── WireGuardConfigurationService.cs  # Config generation
│   ├── ProfileService.cs        # Profile management
│   ├── IPAddressService.cs      # IP address management
│   └── ConfigurationStorageService.cs    # Data persistence
├── 📁 Profiles/                 # Generated at runtime
│   ├── profiles.json            # Server profiles database
│   └── {Profile}_configurations.json    # Per-profile configs
└── 📁 Data/Exports/            # Exported configuration files
```

### Data Storage
- **Profiles**: JSON-based profile storage in `Profiles/` directory
- **Configuration History**: Separate JSON files per profile
- **Export Files**: Timestamped `.conf` files in `Data/Exports/`

## ⚙️ Technical Specifications

### Architecture Pattern
- **MVVM (Model-View-ViewModel)**: Clean separation of concerns
- **Dependency Injection**: Service-based architecture
- **Event-driven UI**: Reactive interface updates

### Key Technologies
- **.NET 9.0**: Modern .NET framework
- **WPF (Windows Presentation Foundation)**: Rich desktop UI
- **System.Text.Json**: High-performance JSON serialization
- **XAML Data Binding**: Declarative UI binding

### Performance Features
- **Lazy Loading**: Configurations loaded on demand
- **Memory Efficient**: Minimal memory footprint
- **Fast IP Assignment**: Optimized IP range management
- **Concurrent Operations**: Non-blocking UI operations

## 🔒 Security & Best Practices

### Security Features
- **No Private Key Storage**: Client private keys never persisted
- **Secure Key Generation**: Cryptographically secure random keys
- **Input Validation**: Comprehensive input sanitization
- **Profile Isolation**: Complete separation between server profiles

### Recommended Practices
1. **Secure Key Exchange**: Use secure channels for public key exchange
2. **Regular Backups**: Backup profile and configuration databases
3. **Access Control**: Restrict application access to authorized personnel
4. **Network Security**: Ensure WireGuard server proper configuration

## 🤝 Contributing

We welcome contributions! Here's how you can help:

### Development Setup
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Contribution Guidelines
- Follow existing code style and patterns
- Add unit tests for new functionality
- Update documentation as needed
- Ensure all tests pass before submitting

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Support & Issues

- **Bug Reports**: [GitHub Issues](https://github.com/yourusername/wireguard-config-generator/issues)
- **Feature Requests**: [GitHub Discussions](https://github.com/yourusername/wireguard-config-generator/discussions)
- **Documentation**: [Wiki](https://github.com/yourusername/wireguard-config-generator/wiki)

## 🙏 Acknowledgments

- [WireGuard](https://www.wireguard.com/) team for the excellent VPN protocol
- [Microsoft](https://microsoft.com) for the .NET framework and WPF
- Contributors and testers who helped improve this tool

---

**⭐ If this project helped you, please consider giving it a star on GitHub!**
