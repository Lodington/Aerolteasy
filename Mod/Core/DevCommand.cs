using System.Collections.Generic;

namespace RoR2DevTool.Core
{
    [System.Serializable]
    public class DevCommand
    {
        public string Type { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}