using System.Collections.Generic;

namespace RoR2DevTool.Models
{
    [System.Serializable]
    public class NetworkedCommand
    {
        public string SenderId { get; set; }           // Steam/Network ID of sender
        public string SenderName { get; set; }         // Display name
        public string CommandType { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public long Timestamp { get; set; }
    }
}
