using System.Collections.Generic;
using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class TestESPDataCommand : BaseCommand
    {
        public override string CommandName => "testespdata";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Creating test ESP data...");

            try
            {
                // Create test monster data
                var testMonsters = new List<object>
                {
                    new
                    {
                        Name = "TestBeetle",
                        DisplayName = "Test Beetle",
                        Health = 150f,
                        MaxHealth = 200f,
                        IsAlive = true,
                        IsElite = false,
                        EliteType = "",
                        PositionX = 10f,
                        PositionY = 5f,
                        PositionZ = 15f,
                        Distance = 25f,
                        NetworkId = 1001
                    },
                    new
                    {
                        Name = "TestWisp",
                        DisplayName = "Test Elite Wisp",
                        Health = 300f,
                        MaxHealth = 400f,
                        IsAlive = true,
                        IsElite = true,
                        EliteType = "Fire",
                        PositionX = -20f,
                        PositionY = 8f,
                        PositionZ = 30f,
                        Distance = 45f,
                        NetworkId = 1002
                    }
                };

                // Create test interactable data
                var testInteractables = new List<object>
                {
                    new
                    {
                        Name = "TestChest",
                        DisplayName = "Test Large Chest",
                        IsAvailable = true,
                        IsActivated = false,
                        PositionX = 5f,
                        PositionY = 2f,
                        PositionZ = 8f,
                        Distance = 12f,
                        NetworkId = 2001,
                        Category = "Chest"
                    },
                    new
                    {
                        Name = "TestShrine",
                        DisplayName = "Test Shrine of Blood",
                        IsAvailable = true,
                        IsActivated = false,
                        PositionX = -10f,
                        PositionY = 3f,
                        PositionZ = 20f,
                        Distance = 28f,
                        NetworkId = 2002,
                        Category = "Shrine"
                    },
                    new
                    {
                        Name = "TestBarrel",
                        DisplayName = "Test Used Barrel",
                        IsAvailable = false,
                        IsActivated = true,
                        PositionX = 15f,
                        PositionY = 1f,
                        PositionZ = -5f,
                        Distance = 18f,
                        NetworkId = 2003,
                        Category = "Barrel"
                    }
                };

                // Create test item data with all tiers to showcase new color scheme
                var testItems = new List<object>
                {
                    new
                    {
                        Name = "SoldiersSyringe",
                        DisplayName = "Soldier's Syringe",
                        ItemTier = "White",
                        PositionX = 3f,
                        PositionY = 1f,
                        PositionZ = 5f,
                        Distance = 8f,
                        NetworkId = 3001,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "AtGMissile",
                        DisplayName = "AtG Missile Mk. 1",
                        ItemTier = "Green",
                        PositionX = -8f,
                        PositionY = 2f,
                        PositionZ = 12f,
                        Distance = 16f,
                        NetworkId = 3002,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "AlienHead",
                        DisplayName = "Alien Head",
                        ItemTier = "Red",
                        PositionX = 12f,
                        PositionY = 4f,
                        PositionZ = -8f,
                        Distance = 22f,
                        NetworkId = 3003,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "GestureOfTheDrowned",
                        DisplayName = "Gesture of the Drowned",
                        ItemTier = "Lunar",
                        PositionX = -15f,
                        PositionY = 6f,
                        PositionZ = 25f,
                        Distance = 35f,
                        NetworkId = 3004,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "QueensGland",
                        DisplayName = "Queen's Gland",
                        ItemTier = "Boss",
                        PositionX = 20f,
                        PositionY = 3f,
                        PositionZ = -12f,
                        Distance = 28f,
                        NetworkId = 3005,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "LostSeersLenses",
                        DisplayName = "Lost Seer's Lenses",
                        ItemTier = "Void White",
                        PositionX = -5f,
                        PositionY = 2f,
                        PositionZ = -10f,
                        Distance = 14f,
                        NetworkId = 3006,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "PlasmaShrimp",
                        DisplayName = "Plasma Shrimp",
                        ItemTier = "Void Green",
                        PositionX = 8f,
                        PositionY = 5f,
                        PositionZ = 18f,
                        Distance = 24f,
                        NetworkId = 3007,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "Polylute",
                        DisplayName = "Polylute",
                        ItemTier = "Void Red",
                        PositionX = -12f,
                        PositionY = 7f,
                        PositionZ = -15f,
                        Distance = 32f,
                        NetworkId = 3008,
                        IsPickupable = true
                    },
                    new
                    {
                        Name = "PlanetaryDevastator",
                        DisplayName = "Planetary Devastator",
                        ItemTier = "Void Boss",
                        PositionX = 25f,
                        PositionY = 8f,
                        PositionZ = 30f,
                        Distance = 45f,
                        NetworkId = 3009,
                        IsPickupable = true
                    }
                };

                // Store test data in game state service (we'll need to add a method for this)
                logger.LogInfo($"Created test ESP data: {testMonsters.Count} monsters, {testInteractables.Count} interactables, {testItems.Count} items");
                logger.LogInfo("Test data includes all item tiers with new improved color scheme:");
                logger.LogInfo("- White items: Light gray (improved readability)");
                logger.LogInfo("- Green items: Bright green");
                logger.LogInfo("- Red items: Bright red");
                logger.LogInfo("- Lunar items: Bright cyan");
                logger.LogInfo("- Boss items: Bright orange");
                logger.LogInfo("- Void items: Purple variants (White->Red->Boss)");
                
                // Log sample data for verification
                foreach (var monster in testMonsters)
                {
                    logger.LogInfo($"Test Monster: {monster}");
                }
                
                foreach (var interactable in testInteractables)
                {
                    logger.LogInfo($"Test Interactable: {interactable}");
                }
                
                foreach (var item in testItems)
                {
                    logger.LogInfo($"Test Item: {item}");
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error creating test ESP data: {ex.Message}");
            }
        }
    }
}