using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using VPNConfigGen.Models;

namespace VPNConfigGen.Services
{
    public class ConfigurationStorageService
    {
        private readonly string _dataDirectory;
        private string _configurationFile;
        private readonly ProfileService _profileService;
        private string _currentProfile;
        
        public ConfigurationStorageService()
        {
            _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            _profileService = new ProfileService();
            SetProfile("Default");
        }
        
        public void SetProfile(string profileName)
        {
            _currentProfile = profileName;
            _configurationFile = _profileService.GetConfigurationFilePath(profileName);
            
            if (!File.Exists(_configurationFile))
            {
                File.WriteAllText(_configurationFile, "[]");
            }
        }
        
        public void SaveConfiguration(ConfigurationRecord config)
        {
            var configurations = GetAllConfigurations().ToList();
            configurations.Add(config);
            
            var json = JsonSerializer.Serialize(configurations, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(_configurationFile, json);
        }
        
        public IEnumerable<ConfigurationRecord> GetAllConfigurations()
        {
            var json = File.ReadAllText(_configurationFile);
            return JsonSerializer.Deserialize<List<ConfigurationRecord>>(json) ?? new List<ConfigurationRecord>();
        }
        
        public ConfigurationRecord GetConfiguration(string id)
        {
            return GetAllConfigurations().FirstOrDefault(c => c.Id == id);
        }
        
        public void DeleteConfiguration(string id)
        {
            var configurations = GetAllConfigurations().Where(c => c.Id != id).ToList();
            
            var json = JsonSerializer.Serialize(configurations, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(_configurationFile, json);
        }
        
        public void ExportClientConfiguration(string configContent, string clientName)
        {
            var fileName = $"{clientName}_{DateTime.Now:yyyyMMdd_HHmmss}.conf";
            var filePath = Path.Combine(_dataDirectory, "Exports", fileName);
            
            var exportDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }
            
            File.WriteAllText(filePath, configContent);
        }
        
        public void ExportServerPeerConfiguration(string configContent, string clientName)
        {
            var fileName = $"peer_{clientName}_{DateTime.Now:yyyyMMdd_HHmmss}.conf";
            var filePath = Path.Combine(_dataDirectory, "Exports", fileName);
            
            var exportDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(exportDirectory))
            {
                Directory.CreateDirectory(exportDirectory);
            }
            
            File.WriteAllText(filePath, configContent);
        }
    }
}