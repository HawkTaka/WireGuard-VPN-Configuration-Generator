using System;
using System.Text;
using VPNConfigGen.Models;

namespace VPNConfigGen.Services
{
    public class WireGuardConfigurationService
    {
        private readonly IPAddressService _ipAddressService;
        private readonly ConfigurationStorageService _storageService;
        
        public WireGuardConfigurationService(IPAddressService ipAddressService, ConfigurationStorageService storageService)
        {
            _ipAddressService = ipAddressService;
            _storageService = storageService;
        }
        
        public (string clientConfig, string serverPeerConfig) GenerateConfiguration(VPNConfiguration config)
        {
            var clientConfig = GenerateClientConfiguration(config);
            var serverPeerConfig = GenerateServerPeerConfiguration(config);
            
            _storageService.SaveConfiguration(new ConfigurationRecord(config));
            
            return (clientConfig, serverPeerConfig);
        }
        
        private string GenerateClientConfiguration(VPNConfiguration config)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("[Interface]");
            sb.AppendLine($"PrivateKey = {config.ClientPrivateKey}");
            sb.AppendLine($"Address = {config.ClientIPAddress}/32");
            sb.AppendLine($"DNS = {config.DNS}");
            sb.AppendLine();
            sb.AppendLine("[Peer]");
            sb.AppendLine($"PublicKey = {config.ServerPublicKey}");
            sb.AppendLine($"AllowedIPs = {config.AllowedIPs}");
            sb.AppendLine($"Endpoint = {config.ServerEndpoint}:{config.ServerPort}");
            sb.AppendLine($"PersistentKeepalive = {config.PersistentKeepalive}");
            
            return sb.ToString();
        }
        
        private string GenerateServerPeerConfiguration(VPNConfiguration config)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("[Peer]");
            sb.AppendLine($"PublicKey = {config.ClientPublicKey}");
            sb.AppendLine($"AllowedIPs = {config.ClientIPAddress}/32");
            
            return sb.ToString();
        }
        
        public string GeneratePrivateKey()
        {
            var random = new Random();
            var keyBytes = new byte[32];
            random.NextBytes(keyBytes);
            
            keyBytes[0] &= 0xF8;
            keyBytes[31] = (byte)((keyBytes[31] & 0x7F) | 0x40);
            
            return Convert.ToBase64String(keyBytes);
        }
        
        public string DerivePublicKey(string privateKey)
        {
            var keyBytes = Convert.FromBase64String(privateKey);
            var publicKeyBytes = new byte[32];
            Array.Copy(keyBytes, publicKeyBytes, Math.Min(32, keyBytes.Length));
            
            return Convert.ToBase64String(publicKeyBytes);
        }
    }
}