using System;

namespace VPNConfigGen.Models
{
    public class ConfigurationRecord
    {
        public string Id { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string ClientPublicKey { get; set; }
        public string ClientIPAddress { get; set; }
        public string ClientName { get; set; }
        
        public ConfigurationRecord()
        {
            Id = Guid.NewGuid().ToString();
            GeneratedAt = DateTime.Now;
        }
        
        public ConfigurationRecord(VPNConfiguration config)
        {
            Id = config.Id;
            GeneratedAt = config.GeneratedAt;
            ClientPublicKey = config.ClientPublicKey;
            ClientIPAddress = config.ClientIPAddress;
            ClientName = config.ClientName;
        }
    }
}