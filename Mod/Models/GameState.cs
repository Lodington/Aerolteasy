using System.Collections.Generic;

namespace RoR2DevTool.Models
{
    [System.Serializable]
    public class PlayerData
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsAlive { get; set; }
        public string CurrentCharacter { get; set; }
        public string CharacterIcon { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public bool GodModeEnabled { get; set; }
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();
        public float Level { get; set; }
        public float Experience { get; set; }
        
        // Advanced stats
        public float BaseDamage { get; set; }
        public float Armor { get; set; }
        public float AttackSpeed { get; set; }
        public float CritChance { get; set; }
        public float MoveSpeed { get; set; }
        public float JumpPower { get; set; }
        public float MaxShield { get; set; }
        public float HealthRegen { get; set; }
    }

    [System.Serializable]
    public class TeleporterData
    {
        public bool IsActive { get; set; }
        public bool IsCharged { get; set; }
        public float ChargeProgress { get; set; }
        public bool BossEventActive { get; set; }
        public int EnemiesRemaining { get; set; }
    }

    [System.Serializable]
    public class MonsterData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public bool IsAlive { get; set; }
        public bool IsElite { get; set; }
        public string EliteType { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Distance { get; set; }
        public int NetworkId { get; set; }
    }

    [System.Serializable]
    public class InteractableData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActivated { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Distance { get; set; }
        public int NetworkId { get; set; }
        public string Category { get; set; } // "Chest", "Shrine", "Barrel", etc.
    }

    [System.Serializable]
    public class ItemData
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ItemTier { get; set; } // "White", "Green", "Red", "Lunar", "Boss", "Void"
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Distance { get; set; }
        public int NetworkId { get; set; }
        public bool IsPickupable { get; set; }
    }

    [System.Serializable]
    public class GameState
    {
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public uint TeamMoney { get; set; }
        public string CurrentStage { get; set; }
        public int StageNumber { get; set; }
        public float DifficultyCoefficient { get; set; }
        public float GameTime { get; set; }
        public bool IsInRun { get; set; }
        
        // Enhanced game state data
        public TeleporterData Teleporter { get; set; } = new TeleporterData();
        public int TotalEnemiesAlive { get; set; }
        public bool IsPaused { get; set; }
        public float TimeScale { get; set; }
        public string WeatherType { get; set; }
        public int SurvivorCount { get; set; }
        public float AverageDamage { get; set; }
        public float AverageHealing { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public string Seed { get; set; }
        
        // ESP Data
        public List<MonsterData> Monsters { get; set; } = new List<MonsterData>();
        public List<InteractableData> Interactables { get; set; } = new List<InteractableData>();
        public List<ItemData> Items { get; set; } = new List<ItemData>();
    }
}