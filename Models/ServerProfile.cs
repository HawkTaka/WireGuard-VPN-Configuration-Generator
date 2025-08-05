using System;

namespace VPNConfigGen.Models
{
    public class ServerProfile
    {
        public string ProfileName { get; set; }
        public string ServerPublicKey { get; set; }
        public string ServerEndpoint { get; set; }
        public int ServerPort { get; set; }
        public string Subnet { get; set; }
        public string DNS { get; set; }
        public string AllowedIPs { get; set; }
        public int PersistentKeepalive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        
        public ServerProfile()
        {
            ProfileName = "New Server";
            ServerPublicKey = string.Empty;
            ServerEndpoint = string.Empty;
            ServerPort = 51820;
            Subnet = "10.0.0.0/24";
            DNS = "1.1.1.1";
            AllowedIPs = "10.0.0.0/24";
            PersistentKeepalive = 25;
            CreatedAt = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }
}