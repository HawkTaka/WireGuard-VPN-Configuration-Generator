using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using VPNConfigGen.Models;

namespace VPNConfigGen.Services
{
    public class ProfileService
    {
        private readonly string _profilesDirectory;
        private readonly string _profilesFile;
        
        public ProfileService()
        {
            _profilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles");
            _profilesFile = Path.Combine(_profilesDirectory, "profiles.json");
            
            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
            
            if (!File.Exists(_profilesFile))
            {
                File.WriteAllText(_profilesFile, "[]");
            }
        }
        
        public void SaveProfile(ServerProfile profile)
        {
            profile.LastModified = DateTime.Now;
            var profiles = GetAllProfiles().ToList();
            
            var existingProfile = profiles.FirstOrDefault(p => p.ProfileName == profile.ProfileName);
            if (existingProfile != null)
            {
                profiles.Remove(existingProfile);
            }
            
            profiles.Add(profile);
            
            var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(_profilesFile, json);
        }
        
        public IEnumerable<ServerProfile> GetAllProfiles()
        {
            var json = File.ReadAllText(_profilesFile);
            return JsonSerializer.Deserialize<List<ServerProfile>>(json) ?? new List<ServerProfile>();
        }
        
        public ServerProfile GetProfile(string profileName)
        {
            return GetAllProfiles().FirstOrDefault(p => p.ProfileName == profileName);
        }
        
        public void DeleteProfile(string profileName)
        {
            var profiles = GetAllProfiles().Where(p => p.ProfileName != profileName).ToList();
            
            var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(_profilesFile, json);
            
            // Also delete the associated configuration file
            var configFile = GetConfigurationFilePath(profileName);
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
        }
        
        public string GetConfigurationFilePath(string profileName)
        {
            var safeFileName = string.Join("_", profileName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_profilesDirectory, $"{safeFileName}_configurations.json");
        }
        
        public bool ProfileExists(string profileName)
        {
            return GetAllProfiles().Any(p => p.ProfileName == profileName);
        }
    }
}