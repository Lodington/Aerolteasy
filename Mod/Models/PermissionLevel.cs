namespace RoR2DevTool.Models
{
    public enum PermissionLevel
    {
        None = 0,           // No permissions
        ReadOnly = 1,       // Can view game state only
        Basic = 2,          // Can use basic commands (items, money)
        Advanced = 3,       // Can use advanced commands (teleport, spawn)
        Admin = 4,          // Can use all commands except permission management
        Host = 5            // Full control including permission management
    }
}
