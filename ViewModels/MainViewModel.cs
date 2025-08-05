using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VPNConfigGen.Models;
using VPNConfigGen.Services;
using VPNConfigGen.Views;

namespace VPNConfigGen.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly WireGuardConfigurationService _configService;
        private readonly IPAddressService _ipAddressService;
        private readonly ConfigurationStorageService _storageService;
        private readonly ProfileService _profileService;
        
        private string _clientName;
        private string _clientPublicKey;
        private string _serverPublicKey;
        private string _serverEndpoint;
        private string _assignedIPAddress;
        private string _generatedClientConfig;
        private string _generatedServerConfig;
        private ConfigurationRecord _selectedConfiguration;
        private ServerProfile _selectedProfile;
        
        public ObservableCollection<ConfigurationRecord> Configurations { get; }
        public ObservableCollection<ServerProfile> ServerProfiles { get; }
        
        public ServerProfile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (SetProperty(ref _selectedProfile, value) && value != null)
                {
                    LoadProfile(value);
                }
            }
        }
        
        public string ClientName
        {
            get => _clientName;
            set => SetProperty(ref _clientName, value);
        }
        
        public string ClientPublicKey
        {
            get => _clientPublicKey;
            set => SetProperty(ref _clientPublicKey, value);
        }
        
        public string ServerPublicKey
        {
            get => _serverPublicKey;
            set => SetProperty(ref _serverPublicKey, value);
        }
        
        public string ServerEndpoint
        {
            get => _serverEndpoint;
            set => SetProperty(ref _serverEndpoint, value);
        }
        
        public string AssignedIPAddress
        {
            get => _assignedIPAddress;
            set => SetProperty(ref _assignedIPAddress, value);
        }
        
        public string GeneratedClientConfig
        {
            get => _generatedClientConfig;
            set => SetProperty(ref _generatedClientConfig, value);
        }
        
        public string GeneratedServerConfig
        {
            get => _generatedServerConfig;
            set => SetProperty(ref _generatedServerConfig, value);
        }
        
        public ConfigurationRecord SelectedConfiguration
        {
            get => _selectedConfiguration;
            set => SetProperty(ref _selectedConfiguration, value);
        }
        
        public ICommand GenerateConfigurationCommand { get; }
        public ICommand GetNextIPCommand { get; }
        public ICommand ExportClientConfigCommand { get; }
        public ICommand ExportServerConfigCommand { get; }
        public ICommand RefreshConfigurationsCommand { get; }
        public ICommand GenerateKeyPairCommand { get; }
        public ICommand NewProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand LoadProfileCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        
        public MainViewModel()
        {
            _profileService = new ProfileService();
            _storageService = new ConfigurationStorageService();
            _ipAddressService = new IPAddressService(_storageService);
            _configService = new WireGuardConfigurationService(_ipAddressService, _storageService);
            
            Configurations = new ObservableCollection<ConfigurationRecord>();
            ServerProfiles = new ObservableCollection<ServerProfile>();
            
            GenerateConfigurationCommand = new RelayCommand(ExecuteGenerateConfiguration, CanExecuteGenerateConfiguration);
            GetNextIPCommand = new RelayCommand(ExecuteGetNextIP);
            ExportClientConfigCommand = new RelayCommand(ExecuteExportClientConfig, CanExecuteExport);
            ExportServerConfigCommand = new RelayCommand(ExecuteExportServerConfig, CanExecuteExport);
            RefreshConfigurationsCommand = new RelayCommand(ExecuteRefreshConfigurations);
            GenerateKeyPairCommand = new RelayCommand(ExecuteGenerateKeyPair);
            NewProfileCommand = new RelayCommand(ExecuteNewProfile);
            SaveProfileCommand = new RelayCommand(ExecuteSaveProfile);
            LoadProfileCommand = new RelayCommand(ExecuteLoadProfile);
            DeleteProfileCommand = new RelayCommand(ExecuteDeleteProfile, CanExecuteDeleteProfile);
            
            LoadProfiles();
            LoadConfigurations();
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            if (SelectedProfile != null)
            {
                try
                {
                    AssignedIPAddress = _ipAddressService.GetNextAvailableIPAddress();
                }
                catch
                {
                    AssignedIPAddress = string.Empty;
                }
            }
        }
        
        private void LoadConfigurations()
        {
            Configurations.Clear();
            foreach (var config in _storageService.GetAllConfigurations().OrderByDescending(c => c.GeneratedAt))
            {
                Configurations.Add(config);
            }
        }
        
        private bool CanExecuteGenerateConfiguration(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ClientName) &&
                   !string.IsNullOrWhiteSpace(ClientPublicKey) &&
                   !string.IsNullOrWhiteSpace(ServerPublicKey) &&
                   !string.IsNullOrWhiteSpace(ServerEndpoint) &&
                   !string.IsNullOrWhiteSpace(AssignedIPAddress);
        }
        
        private void ExecuteGenerateConfiguration(object parameter)
        {
            try
            {
                var privateKey = _configService.GeneratePrivateKey();
                
                var config = new VPNConfiguration
                {
                    ClientName = ClientName,
                    ClientPublicKey = ClientPublicKey,
                    ClientPrivateKey = privateKey,
                    ClientIPAddress = AssignedIPAddress,
                    ServerPublicKey = ServerPublicKey,
                    ServerEndpoint = ServerEndpoint,
                    ServerPort = SelectedProfile?.ServerPort ?? 51820,
                    DNS = SelectedProfile?.DNS ?? "1.1.1.1",
                    AllowedIPs = SelectedProfile?.AllowedIPs ?? "10.0.0.0/24",
                    PersistentKeepalive = SelectedProfile?.PersistentKeepalive ?? 25
                };
                
                var (clientConfig, serverConfig) = _configService.GenerateConfiguration(config);
                
                GeneratedClientConfig = clientConfig;
                GeneratedServerConfig = serverConfig;
                
                LoadConfigurations();
                
                ClientName = string.Empty;
                ClientPublicKey = string.Empty;
                AssignedIPAddress = _ipAddressService.GetNextAvailableIPAddress();
            }
            catch (Exception ex)
            {
                GeneratedClientConfig = $"Error: {ex.Message}";
                GeneratedServerConfig = string.Empty;
            }
        }
        
        private void ExecuteGetNextIP(object parameter)
        {
            try
            {
                AssignedIPAddress = _ipAddressService.GetNextAvailableIPAddress();
            }
            catch (Exception ex)
            {
                AssignedIPAddress = $"Error: {ex.Message}";
            }
        }
        
        private bool CanExecuteExport(object parameter)
        {
            return !string.IsNullOrWhiteSpace(GeneratedClientConfig) && 
                   !string.IsNullOrWhiteSpace(GeneratedServerConfig);
        }
        
        private void ExecuteExportClientConfig(object parameter)
        {
            try
            {
                var clientName = Configurations.FirstOrDefault()?.ClientName ?? "client";
                _storageService.ExportClientConfiguration(GeneratedClientConfig, clientName);
            }
            catch (Exception ex)
            {
                GeneratedClientConfig = $"Export Error: {ex.Message}";
            }
        }
        
        private void ExecuteExportServerConfig(object parameter)
        {
            try
            {
                var clientName = Configurations.FirstOrDefault()?.ClientName ?? "client";
                _storageService.ExportServerPeerConfiguration(GeneratedServerConfig, clientName);
            }
            catch (Exception ex)
            {
                GeneratedServerConfig = $"Export Error: {ex.Message}";
            }
        }
        
        private void ExecuteRefreshConfigurations(object parameter)
        {
            LoadConfigurations();
        }
        
        private void ExecuteGenerateKeyPair(object parameter)
        {
            var privateKey = _configService.GeneratePrivateKey();
            var publicKey = _configService.DerivePublicKey(privateKey);
            
            GeneratedClientConfig = $"Generated Key Pair:\n\nPrivate Key: {privateKey}\nPublic Key: {publicKey}\n\nNote: Save the private key securely!";
            GeneratedServerConfig = string.Empty;
        }
        
        private void LoadProfiles()
        {
            ServerProfiles.Clear();
            foreach (var profile in _profileService.GetAllProfiles())
            {
                ServerProfiles.Add(profile);
            }
            
            if (!ServerProfiles.Any())
            {
                ShowNewProfileDialog();
            }
            
            SelectedProfile = ServerProfiles.FirstOrDefault();
        }
        
        private void ShowNewProfileDialog()
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Profile.ProfileName = "Default";
            
            if (profileWindow.ShowDialog() == true)
            {
                var profile = profileWindow.Profile;
                if (!string.IsNullOrWhiteSpace(profile.ProfileName))
                {
                    _profileService.SaveProfile(profile);
                    ServerProfiles.Add(profile);
                }
            }
            else
            {
                // If user cancels, create a minimal default profile so app doesn't crash
                var defaultProfile = new ServerProfile 
                { 
                    ProfileName = "Default"
                };
                _profileService.SaveProfile(defaultProfile);
                ServerProfiles.Add(defaultProfile);
            }
        }
        
        private void LoadProfile(ServerProfile profile)
        {
            if (profile == null) return;
            
            ServerPublicKey = profile.ServerPublicKey ?? string.Empty;
            ServerEndpoint = profile.ServerEndpoint ?? string.Empty;
            
            _storageService.SetProfile(profile.ProfileName);
            _ipAddressService.SetSubnet(profile.Subnet);
            
            LoadConfigurations();
            
            try
            {
                AssignedIPAddress = _ipAddressService.GetNextAvailableIPAddress();
            }
            catch
            {
                AssignedIPAddress = string.Empty;
            }
        }
        
        private void ExecuteNewProfile(object parameter)
        {
            var profileWindow = new ProfileWindow();
            if (profileWindow.ShowDialog() == true)
            {
                var profile = profileWindow.Profile;
                if (!string.IsNullOrWhiteSpace(profile.ProfileName))
                {
                    _profileService.SaveProfile(profile);
                    LoadProfiles();
                    SelectedProfile = ServerProfiles.FirstOrDefault(p => p.ProfileName == profile.ProfileName);
                }
            }
        }
        
        private void ExecuteSaveProfile(object parameter)
        {
            if (SelectedProfile != null)
            {
                SelectedProfile.ServerPublicKey = ServerPublicKey;
                SelectedProfile.ServerEndpoint = ServerEndpoint;
                _profileService.SaveProfile(SelectedProfile);
            }
        }
        
        private void ExecuteLoadProfile(object parameter)
        {
            if (SelectedProfile != null)
            {
                var profileWindow = new ProfileWindow(SelectedProfile);
                if (profileWindow.ShowDialog() == true)
                {
                    _profileService.SaveProfile(profileWindow.Profile);
                    LoadProfiles();
                    SelectedProfile = ServerProfiles.FirstOrDefault(p => p.ProfileName == profileWindow.Profile.ProfileName);
                }
            }
        }
        
        private void ExecuteDeleteProfile(object parameter)
        {
            if (SelectedProfile != null && ServerProfiles.Count > 1)
            {
                var profileToDelete = SelectedProfile.ProfileName;
                _profileService.DeleteProfile(profileToDelete);
                LoadProfiles();
            }
        }
        
        private bool CanExecuteDeleteProfile(object parameter)
        {
            return SelectedProfile != null && ServerProfiles?.Count > 1;
        }
    }
}