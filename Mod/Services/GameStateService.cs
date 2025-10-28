using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2DevTool.Services
{
    public class GameStateService
    {
        private ManualLogSource logger;
        private Dictionary<int, bool> playerGodModes = new Dictionary<int, bool>();
        private bool verboseLogging = false; // Control verbose logging
        private Dictionary<string, string> characterIconCache = new Dictionary<string, string>(); // Cache for character icons
        private List<object> itemCatalog = new List<object>(); // Cache for item catalog
        private List<object> characterDefaults = new List<object>(); // Cache for character default stats

        public GameStateService(ManualLogSource logger)
        {
            this.logger = logger;
        }

        public void SetVerboseLogging(bool enabled)
        {
            verboseLogging = enabled;
            logger.LogInfo($"Verbose logging {(enabled ? "enabled" : "disabled")}");
        }

        public void SetItemCatalog(List<object> items)
        {
            itemCatalog = items;
            logger.LogInfo($"Item catalog updated with {items.Count} items");
        }

        public List<object> GetItemCatalog()
        {
            return itemCatalog;
        }

        public void SetCharacterDefaults(List<object> defaults)
        {
            characterDefaults = defaults;
            logger.LogInfo($"Character defaults updated with {defaults.Count} characters");
        }

        public List<object> GetCharacterDefaults()
        {
            return characterDefaults;
        }

        private string GetCharacterIcon(string characterName)
        {
            // Check cache first
            if (characterIconCache.ContainsKey(characterName))
            {
                return characterIconCache[characterName];
            }

            // Use emoji icons only (safe and reliable)
            var iconData = GetFallbackIcon(characterName);
            characterIconCache[characterName] = iconData;
            return iconData;
        }

        private string GetFallbackIcon(string characterName)
        {
            // Emoji icons for each character
            return characterName switch
            {
                "CommandoBody" => "🔫", // Commando - Gun
                "HuntressBody" => "🏹", // Huntress - Bow
                "Bandit2Body" => "🔪", // Bandit - Knife
                "ToolbotBody" => "🤖", // MUL-T - Robot
                "EngiBody" => "🔧", // Engineer - Wrench
                "MageBody" => "🔮", // Artificer - Crystal Ball
                "MercBody" => "⚔️", // Mercenary - Sword
                "TreebotBody" => "🌱", // REX - Plant
                "LoaderBody" => "👊", // Loader - Fist
                "CrocoBody" => "🦎", // Acrid - Lizard
                "CaptainBody" => "⚓", // Captain - Anchor
                "RailgunnerBody" => "🎯", // Railgunner - Target
                "VoidSurvivorBody" => "🕳️", // Void Fiend - Void
                "SeekerBody" => "👁️", // Seeker - Eye
                "FalseSonBody" => "👤", // False Son - Silhouette
                "ChefBody" => "👨‍🍳", // Chef - Cook
                _ => "👤" // Default - Generic person
            };
        }

        public GameState GetGameState()
        {
            var gameState = new GameState();
            
            try
            {
                // Get all players - with safe access
                if (NetworkUser.readOnlyInstancesList != null)
                {
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser?.gameObject != null)
                        {
                            try
                            {
                                var playerId = (int)networkUser.id.value;
                                var playerData = new PlayerData
                                {
                                    PlayerId = playerId,
                                    PlayerName = networkUser.userName ?? $"Player {playerId}",
                                    IsAlive = networkUser.master?.GetBody() != null,
                                    GodModeEnabled = playerGodModes.ContainsKey(playerId) && playerGodModes[playerId]
                                };

                                var body = networkUser.master?.GetBody();
                                if (body?.gameObject != null)
                                {
                                    var characterName = body.name?.Replace("(Clone)", "") ?? "Unknown";
                                    playerData.CurrentCharacter = characterName;
                                    playerData.CharacterIcon = GetCharacterIcon(characterName);
                                    playerData.Health = body.healthComponent?.health ?? 0;
                                    playerData.MaxHealth = body.healthComponent?.fullHealth ?? 0;
                                    
                                    // Safe team level access
                                    if (TeamManager.instance != null && networkUser.master != null)
                                    {
                                        playerData.Level = TeamManager.instance.GetTeamLevel(networkUser.master.teamIndex);
                                        playerData.Experience = TeamManager.instance.GetTeamCurrentLevelExperience(networkUser.master.teamIndex);
                                    }
                                    
                                    // Try multiple ways to get the inventory - with safety
                                    Inventory inventory = null;
                                    
                                    // Method 1: From body
                                    if (body.inventory != null)
                                    {
                                        inventory = body.inventory;
                                    }
                                    // Method 2: From master
                                    else if (networkUser.master?.inventory != null)
                                    {
                                        inventory = networkUser.master.inventory;
                                    }
                                    
                                    if (inventory != null)
                                    {
                                        playerData.Items = GetPlayerItems(inventory);
                                    }
                                    else
                                    {
                                        playerData.Items = new Dictionary<string, int>();
                                    }
                                    
                                    // Collect advanced stats
                                    playerData.BaseDamage = body.baseDamage;
                                    playerData.Armor = body.armor;
                                    playerData.AttackSpeed = body.attackSpeed;
                                    playerData.CritChance = body.crit;
                                    playerData.MoveSpeed = body.moveSpeed;
                                    playerData.JumpPower = body.jumpPower;
                                    playerData.MaxShield = body.maxShield;
                                    playerData.HealthRegen = body.regen;
                                }
                                else
                                {
                                    playerData.Items = new Dictionary<string, int>();
                                }

                                gameState.Players.Add(playerData);
                            }
                            catch (Exception playerEx)
                            {
                                logger.LogWarning($"Error processing player: {playerEx.Message}");
                            }
                        }
                    }
                }
                
                // Get team and run info - with safe access
                // More robust run detection - check multiple conditions
                gameState.IsInRun = IsActuallyInRun();
                
                if (Run.instance != null)
                {
                    try
                    {
                        // Safe team money access
                       // if (TeamManager.instance?.GetTeamComponent(TeamIndex.Player) != null)
                        //{
                        //    gameState.TeamMoney = TeamManager.instance.GetTeamComponent(TeamIndex.Player).money;
                     //   }
                        
                        gameState.CurrentStage = SceneCatalog.GetSceneDefForCurrentScene()?.baseSceneName ?? "Unknown";
                        gameState.StageNumber = Run.instance.stageClearCount + 1;
                        gameState.DifficultyCoefficient = Run.instance.difficultyCoefficient;
                        gameState.GameTime = Run.instance.GetRunStopwatch();
                        gameState.IsPaused = Time.timeScale == 0f;
                        gameState.TimeScale = Time.timeScale;
                        gameState.Seed = Run.instance.seed.ToString();
                    }
                    catch (Exception runEx)
                    {
                        logger.LogWarning($"Error getting run info: {runEx.Message}");
                    }
                }

                // Get teleporter info - with safe access
                try
                {
                    var teleporter = TeleporterInteraction.instance;
                    if (teleporter != null && teleporter.gameObject != null)
                    {
                        gameState.Teleporter.IsActive = teleporter.isActiveAndEnabled;
                        gameState.Teleporter.IsCharged = teleporter.isCharged;
                        gameState.Teleporter.ChargeProgress = teleporter.chargeFraction;
                        gameState.Teleporter.BossEventActive = teleporter.bossDirector?.enabled ?? false;
                    }
                }
                catch (Exception teleEx)
                {
                    logger.LogWarning($"Error getting teleporter info: {teleEx.Message}");
                }

                // Count enemies and collect monster data - with safe access
                try
                {
                    if (CharacterMaster.readOnlyInstancesList != null)
                    {
                        var monsters = new List<MonsterData>();
                        var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                        var playerPosition = playerBody?.transform?.position ?? Vector3.zero;

                        foreach (var master in CharacterMaster.readOnlyInstancesList)
                        {
                            if (master != null && master.teamIndex == TeamIndex.Monster && master.hasBody)
                            {
                                var body = master.GetBody();
                                if (body != null && body.gameObject != null)
                                {
                                    var position = body.transform.position;
                                    var distance = Vector3.Distance(playerPosition, position);
                                    
                                    var monsterData = new MonsterData
                                    {
                                        Name = body.name?.Replace("(Clone)", "") ?? "Unknown",
                                        DisplayName = body.GetDisplayName() ?? "Unknown Monster",
                                        Health = body.healthComponent?.health ?? 0f,
                                        MaxHealth = body.healthComponent?.fullHealth ?? 0f,
                                        IsAlive = body.healthComponent?.alive ?? false,
                                        IsElite = body.isElite,
                                        EliteType = GetEliteTypeName(body),
                                        PositionX = position.x,
                                        PositionY = position.y,
                                        PositionZ = position.z,
                                        Distance = distance,
                                    };
                                    
                                    monsters.Add(monsterData);
                                }
                            }
                        }
                        
                        gameState.TotalEnemiesAlive = monsters.Count;
                        gameState.Monsters = monsters;
                        
                        if (verboseLogging)
                        {
                            logger.LogInfo($"ESP: Collected {monsters.Count} monsters");
                        }
                    }
                }
                catch (Exception enemyEx)
                {
                    logger.LogWarning($"Error collecting monster data: {enemyEx.Message}");
                }

                // Collect interactables data - with safe access
                try
                {
                    var interactables = new List<InteractableData>();
                    var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                    var playerPosition = playerBody?.transform?.position ?? Vector3.zero;

                    if (InstanceTracker.GetInstancesList<PurchaseInteraction>() != null)
                    {
                        foreach (var interaction in InstanceTracker.GetInstancesList<PurchaseInteraction>())
                        {
                            if (interaction != null && interaction.gameObject != null)
                            {
                                var position = interaction.transform.position;
                                var distance = Vector3.Distance(playerPosition, position);
                                
                                var interactableData = new InteractableData
                                {
                                    Name = interaction.name?.Replace("(Clone)", "") ?? "Unknown",
                                    DisplayName = interaction.GetDisplayName() ?? "Unknown Interactable",
                                    IsAvailable = interaction.available,
                                    IsActivated = !interaction.available,
                                    PositionX = position.x,
                                    PositionY = position.y,
                                    PositionZ = position.z,
                                    Distance = distance,
                                    Category = GetInteractableCategory(interaction)
                                };
                                
                                interactables.Add(interactableData);
                            }
                        }
                    }
                    
                    gameState.Interactables = interactables;
                    
                    if (verboseLogging)
                    {
                        logger.LogInfo($"ESP: Collected {interactables.Count} interactables");
                    }
                }
                catch (Exception interactableEx)
                {
                    logger.LogWarning($"Error collecting interactable data: {interactableEx.Message}");
                }

                // Collect items data - with safe access
                try
                {
                    var items = new List<ItemData>();
                    var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                    var playerPosition = playerBody?.transform?.position ?? Vector3.zero;

                    if (InstanceTracker.GetInstancesList<GenericPickupController>() != null)
                    {
                        foreach (var pickup in InstanceTracker.GetInstancesList<GenericPickupController>())
                        {
                            if (pickup != null && pickup.gameObject != null && pickup.pickupIndex != PickupIndex.none)
                            {
                                var position = pickup.transform.position;
                                var distance = Vector3.Distance(playerPosition, position);
                                
                                var pickupDef = PickupCatalog.GetPickupDef(pickup.pickupIndex);
                                if (pickupDef != null && pickupDef.itemIndex != ItemIndex.None)
                                {
                                    var itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                                    if (itemDef != null)
                                    {
                                        var itemData = new ItemData
                                        {
                                            Name = itemDef.name ?? "Unknown",
                                            DisplayName = itemDef.nameToken != null ? Language.GetString(itemDef.nameToken) : itemDef.name ?? "Unknown Item",
                                            ItemTier = GetItemTierName(itemDef.tier),
                                            PositionX = position.x,
                                            PositionY = position.y,
                                            PositionZ = position.z,
                                            Distance = distance,
                                            NetworkId = pickup.gameObject.GetInstanceID(),
                                            IsPickupable = pickup.gameObject.activeInHierarchy
                                        };
                                        
                                        items.Add(itemData);
                                    }
                                }
                            }
                        }
                    }
                    
                    gameState.Items = items;
                    
                    if (verboseLogging)
                    {
                        logger.LogInfo($"ESP: Collected {items.Count} items");
                    }
                }
                catch (Exception itemEx)
                {
                    logger.LogWarning($"Error collecting item data: {itemEx.Message}");
                }

                
                // Count survivors
                gameState.SurvivorCount = gameState.Players.Count(p => p.IsAlive);

                // Calculate stats - with safe math
                if (gameState.Players.Count > 0)
                {
                    try
                    {
                        gameState.AverageDamage = gameState.Players.Where(p => p.Level > 0).DefaultIfEmpty().Average(p => p?.Level * 100 ?? 0);
                        gameState.AverageHealing = gameState.Players.Where(p => p.MaxHealth > 0).DefaultIfEmpty().Average(p => Math.Max(0, (p?.MaxHealth ?? 0) - (p?.Health ?? 0)));
                    }
                    catch (Exception statsEx)
                    {
                        logger.LogWarning($"Error calculating stats: {statsEx.Message}");
                    }
                }

                // Get weather (if available)
                gameState.WeatherType = "Clear"; // Default, could be enhanced with weather mods
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting game state: {ex.Message}");
                if (verboseLogging)
                {
                    logger.LogError(ex.StackTrace);
                }
            }
            
            return gameState;
        }

        private Dictionary<string, int> GetPlayerItems(Inventory inventory)
        {
            var items = new Dictionary<string, int>();
            
            try
            {
                // Method 1: Try the standard GetItemCount approach first
                for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
                {
                    int count = inventory.GetItemCount(i);
                    if (count > 0)
                    {
                        var itemDef = ItemCatalog.GetItemDef(i);
                        if (itemDef != null)
                        {
                            items[itemDef.name] = count;
                        }
                    }
                }
                
                // Method 2: If method 1 failed, use itemAcquisitionOrder (fallback for sync issues)
                if (items.Count == 0 && inventory.itemAcquisitionOrder != null && inventory.itemAcquisitionOrder.Count > 0)
                {
                    foreach (var itemIndex in inventory.itemAcquisitionOrder)
                    {
                        int count = inventory.GetItemCount(itemIndex);
                        var itemDef = ItemCatalog.GetItemDef(itemIndex);
                        if (itemDef != null)
                        {
                            // Use the count from GetItemCount, or 1 if it returns 0 but item is in acquisition order
                            items[itemDef.name] = count > 0 ? count : 1;
                        }
                    }
                }
                
                // Method 3: Direct access to itemStacks as last resort - with safety checks
                if (items.Count == 0)
                {
                    try
                    {
                        var itemStacksField = typeof(Inventory).GetField("itemStacks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (itemStacksField != null)
                        {
                            var itemStacks = itemStacksField.GetValue(inventory) as int[];
                            if (itemStacks != null && ItemCatalog.itemCount > 0)
                            {
                                int maxItems = Math.Min(itemStacks.Length, ItemCatalog.itemCount);
                                for (int i = 0; i < maxItems; i++)
                                {
                                    if (itemStacks[i] > 0)
                                    {
                                        var itemDef = ItemCatalog.GetItemDef((ItemIndex)i);
                                        if (itemDef != null && !string.IsNullOrEmpty(itemDef.name))
                                        {
                                            items[itemDef.name] = itemStacks[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception reflectionEx)
                    {
                        logger.LogWarning($"Error using reflection for items: {reflectionEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting player items: {ex.Message}");
                if (verboseLogging)
                {
                    logger.LogError(ex.StackTrace);
                }
            }
            
            return items;
        }

        public void SetPlayerGodMode(int playerId, bool enabled)
        {
            if (playerId == -1)
            {
                // Apply to all players
                foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                {
                    var id = (int)networkUser.id.value;
                    playerGodModes[id] = enabled;
                }
            }
            else
            {
                playerGodModes[playerId] = enabled;
            }
        }

        private string GetEliteTypeName(CharacterBody body)
        {
            try
            {
                if (body?.inventory?.currentEquipmentIndex != null)
                {
                    var equipmentDef = EquipmentCatalog.GetEquipmentDef(body.inventory.currentEquipmentIndex);
                    if (equipmentDef != null)
                    {
                        return equipmentDef.name switch
                        {
                            "AffixRed" => "Fire",
                            "AffixBlue" => "Ice", 
                            "AffixYellow" => "Lightning",
                            "AffixPoison" => "Poison",
                            "AffixHaunted" => "Celestine",
                            "AffixWhite" => "Celestine",
                            "AffixSealed" => "Malachite",
                            "AffixLunar" => "Perfected",
                            _ => "Elite"
                        };
                    }
                }
                return "Elite";
            }
            catch
            {
                return "Elite";
            }
        }

        private string GetInteractableCategory(PurchaseInteraction interaction)
        {
            try
            {
                var name = interaction.name?.ToLower() ?? "";
                
                if (name.Contains("chest") || name.Contains("crate"))
                    return "Chest";
                if (name.Contains("shrine"))
                    return "Shrine";
                if (name.Contains("barrel") || name.Contains("pot"))
                    return "Barrel";
                if (name.Contains("drone") || name.Contains("turret"))
                    return "Drone";
                if (name.Contains("shop") || name.Contains("vendor"))
                    return "Shop";
                if (name.Contains("printer") || name.Contains("duplicator"))
                    return "Printer";
                if (name.Contains("portal") || name.Contains("teleporter"))
                    return "Portal";
                if (name.Contains("scrap"))
                    return "Scrap";
                
                return "Other";
            }
            catch
            {
                return "Other";
            }
        }

        private string GetItemTierName(ItemTier tier)
        {
            return tier switch
            {
                ItemTier.Tier1 => "White",
                ItemTier.Tier2 => "Green", 
                ItemTier.Tier3 => "Red",
                ItemTier.Lunar => "Lunar",
                ItemTier.Boss => "Boss",
                ItemTier.VoidTier1 => "Void White",
                ItemTier.VoidTier2 => "Void Green",
                ItemTier.VoidTier3 => "Void Red",
                ItemTier.VoidBoss => "Void Boss",
                _ => "Unknown"
            };
        }

        private bool IsActuallyInRun()
        {
            try
            {
                // Check if Run instance exists
                if (Run.instance == null)
                {
                    return false;
                }
                
                // Check if we have active network users with bodies (actual players in game)
                bool hasActivePlayers = false;
                if (NetworkUser.readOnlyInstancesList != null)
                {
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser?.master?.GetBody() != null)
                        {
                            hasActivePlayers = true;
                            break;
                        }
                    }
                }
                
                // Check if we're in a valid scene (not main menu)
                var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                bool isInGameScene = currentScene != null && 
                                   !currentScene.name.ToLower().Contains("menu") && 
                                   !currentScene.name.ToLower().Contains("lobby") &&
                                   !currentScene.name.ToLower().Contains("title");
                
                // We're in a run if:
                // 1. Run instance exists AND
                // 2. We have active players with bodies AND  
                // 3. We're not in a menu scene
                bool inRun = Run.instance != null && hasActivePlayers && isInGameScene;
                
                if (verboseLogging)
                {
                    logger.LogInfo($"Run detection - Run.instance: {Run.instance != null}, HasActivePlayers: {hasActivePlayers}, InGameScene: {isInGameScene} ({currentScene.name}), Result: {inRun}");
                }
                
                return inRun;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error detecting run state: {ex.Message}");
                // Fallback to simple check
                return Run.instance != null;
            }
        }

        private List<MonsterData> GetMonsterData()
        {
            var monsters = new List<MonsterData>();
            
            try
            {
                if (CharacterMaster.readOnlyInstancesList == null) return monsters;
                
                // Get player position for distance calculation
                Vector3 playerPosition = Vector3.zero;
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.cachedBody != null)
                {
                    playerPosition = localUser.cachedBody.transform.position;
                }
                
                foreach (var master in CharacterMaster.readOnlyInstancesList)
                {
                    if (master == null || master.teamIndex != TeamIndex.Monster || !master.hasBody) continue;
                    
                    var body = master.GetBody();
                    if (body == null || body.gameObject == null) continue;
                    
                    var position = body.transform.position;
                    var distance = Vector3.Distance(playerPosition, position);
                    
                    var monsterData = new MonsterData
                    {
                        Name = body.name?.Replace("(Clone)", "") ?? "Unknown",
                        DisplayName = body.GetDisplayName() ?? "Unknown Monster",
                        Health = body.healthComponent?.health ?? 0,
                        MaxHealth = body.healthComponent?.fullHealth ?? 0,
                        IsAlive = body.healthComponent?.alive ?? false,
                        IsElite = body.isElite,
                        EliteType = body.isElite ? GetEliteTypeName(body) : "",
                        PositionX = position.x,
                        PositionY = position.y,
                        PositionZ = position.z,
                        Distance = distance,
                        NetworkId = body.gameObject.GetInstanceID()
                    };
                    
                    monsters.Add(monsterData);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error collecting monster data: {ex.Message}");
            }
            
            return monsters;
        }
        

        

        

        


        public void ApplyGodMode()
        {
            try
            {
                // Apply god mode for all players - with safe access
                if (NetworkUser.readOnlyInstancesList == null) return;
                
                foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                {
                    if (networkUser?.master?.GetBody() != null)
                    {
                        var playerId = (int)networkUser.id.value;
                        if (playerGodModes.ContainsKey(playerId) && playerGodModes[playerId])
                        {
                            var body = networkUser.master.GetBody();
                            if (body?.healthComponent != null && body.gameObject != null)
                            {
                                var healthComp = body.healthComponent;
                                if (healthComp.fullHealth > 0)
                                {
                                    healthComp.health = healthComp.fullHealth;
                                }
                                if (healthComp.fullShield > 0)
                                {
                                    healthComp.shield = healthComp.fullShield;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error applying god mode: {ex.Message}");
            }
        }
    }
}