namespace RoR2DevTool.Models
{
    [System.Serializable]
    public class PermissionRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public PermissionLevel RequestedLevel { get; set; }
    }

    [System.Serializable]
    public class PermissionResponse
    {
        public string UserId { get; set; }
        public PermissionLevel GrantedLevel { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
