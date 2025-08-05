using System;

namespace VPNConfigGen.Models
{
    public class VPNConfiguration
    {
        public string Id { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string ClientPublicKey { get; set; }
        public string ClientPrivateKey { get; set; }
        public string ClientIPAddress { get; set; }
        public string ServerPublicKey { get; set; }
        public string ServerEndpoint { get; set; }
        public int ServerPort { get; set; }
        public string DNS { get; set; }
        public string AllowedIPs { get; set; }
        public int PersistentKeepalive { get; set; }
        public string ClientName { get; set; }
        
        public VPNConfiguration()
        {
            Id = Guid.NewGuid().ToString();
            GeneratedAt = DateTime.Now;
            ServerPort = 51820;
            DNS = "1.1.1.1";
            AllowedIPs = "10.0.0.0/24";
            PersistentKeepalive = 25;
        }
    }
}