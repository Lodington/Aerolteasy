using UnityEngine;

namespace RoR2DevTool.ESP
{
    public class ESPConfig
    {
        // ESP Toggle Settings
        public bool ShowMonsters { get; set; } = true;
        public bool ShowInteractables { get; set; } = true;
        public bool ShowDistances { get; set; } = true;
        public bool ShowHealth { get; set; } = true;
        public bool ShowElites { get; set; } = true;
        public bool ShowBoxes { get; set; } = true;
        public bool ShowLines { get; set; } = false;

        // Filter Settings
        public float MaxDistance { get; set; } = 100f;
        public bool FilterByDistance { get; set; } = true;

        // Visual Settings
        public Color MonsterColor { get; set; } = Color.red;
        public Color EliteColor { get; set; } = Color.magenta;
        public Color InteractableColor { get; set; } = Color.cyan;
        public Color ChestColor { get; set; } = Color.yellow;
        public Color ShrineColor { get; set; } = Color.green;
        
        // Text Settings
        public int FontSize { get; set; } = 12;
        public Color TextColor { get; set; } = Color.white;
        public Color TextOutlineColor { get; set; } = Color.black;
        
        // Box Settings
        public float BoxWidth { get; set; } = 2f;
        public float BoxHeight { get; set; } = 3f;
        
        // Keybinds
        public KeyCode ToggleESPKey { get; set; } = KeyCode.F1;
        public KeyCode ToggleMonstersKey { get; set; } = KeyCode.F2;
        public KeyCode ToggleInteractablesKey { get; set; } = KeyCode.F3;
        public KeyCode ToggleDistanceKey { get; set; } = KeyCode.F4;
    }
}