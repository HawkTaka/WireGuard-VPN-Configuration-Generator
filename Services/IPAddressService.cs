using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VPNConfigGen.Services
{
    public class IPAddressService
    {
        private readonly ConfigurationStorageService _storageService;
        private string _subnet = "10.0.0.0";
        private int _subnetBits = 24;
        private readonly int _startingHost = 2;
        
        public IPAddressService(ConfigurationStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public void SetSubnet(string subnet)
        {
            if (subnet.Contains("/"))
            {
                var parts = subnet.Split('/');
                _subnet = parts[0];
                _subnetBits = int.Parse(parts[1]);
            }
            else
            {
                _subnet = subnet;
            }
        }
        
        public string GetNextAvailableIPAddress()
        {
            var usedAddresses = _storageService.GetAllConfigurations()
                .Select(c => c.ClientIPAddress)
                .Where(ip => !string.IsNullOrEmpty(ip))
                .ToHashSet();
            
            var subnetParts = _subnet.Split('.');
            var baseAddress = (uint)((int.Parse(subnetParts[0]) << 24) |
                                     (int.Parse(subnetParts[1]) << 16) |
                                     (int.Parse(subnetParts[2]) << 8) |
                                     int.Parse(subnetParts[3]));
            
            var maxHosts = Math.Pow(2, 32 - _subnetBits) - 2;
            
            for (int i = _startingHost; i <= maxHosts; i++)
            {
                var address = baseAddress + (uint)i;
                var ipAddress = new IPAddress(BitConverter.GetBytes(address).Reverse().ToArray()).ToString();
                
                if (!usedAddresses.Contains(ipAddress))
                {
                    return ipAddress;
                }
            }
            
            throw new InvalidOperationException("No available IP addresses in the subnet");
        }
        
        public bool IsIPAddressAvailable(string ipAddress)
        {
            var usedAddresses = _storageService.GetAllConfigurations()
                .Select(c => c.ClientIPAddress)
                .Where(ip => !string.IsNullOrEmpty(ip))
                .ToHashSet();
            
            return !usedAddresses.Contains(ipAddress);
        }
        
        public bool IsValidIPAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;
            
            if (!IPAddress.TryParse(ipAddress, out var ip))
                return false;
            
            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;
            
            var subnetParts = _subnet.Split('.');
            for (int i = 0; i < _subnetBits / 8; i++)
            {
                if (parts[i] != subnetParts[i])
                    return false;
            }
            
            return true;
        }
    }
}