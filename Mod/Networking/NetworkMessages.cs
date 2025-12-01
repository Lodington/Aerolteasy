using RoR2DevTool.Models;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking
{
    // Base message for all dev tool commands
    public class DevToolCommandMessage : MessageBase
    {
        public string senderId;
        public string senderName;
        public string commandType;
        public string commandDataJson; // Serialized command data
    }

    // Response message for command execution
    public class DevToolCommandResponseMessage : MessageBase
    {
        public bool success;
        public string message;
        public string commandType;
    }

    // Permission request message
    public class PermissionRequestMessage : MessageBase
    {
        public string userId;
        public string userName;
        public PermissionLevel requestedLevel;
    }

    // Permission grant message (host -> client)
    public class PermissionGrantMessage : MessageBase
    {
        public string userId;
        public PermissionLevel grantedLevel;
    }

    // Permission update broadcast (host -> all clients)
    public class PermissionUpdateMessage : MessageBase
    {
        public string userId;
        public PermissionLevel level;
    }

    // Game state sync message (host -> clients)
    public class GameStateSyncMessage : MessageBase
    {
        public string gameStateJson; // Serialized game state
    }

    // Network message type IDs (must be unique and > 1000)
    public static class DevToolMessageTypes
    {
        public const short Command = 2000;
        public const short CommandResponse = 2001;
        public const short PermissionRequest = 2002;
        public const short PermissionGrant = 2003;
        public const short PermissionUpdate = 2004;
        public const short GameStateSync = 2005;
    }
}
