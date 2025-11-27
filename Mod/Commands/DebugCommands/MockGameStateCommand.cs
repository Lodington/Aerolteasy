using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Models;
using RoR2DevTool.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class MockGameStateCommand : BaseCommand
    {
        public override string CommandName => "mockgamestate";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                logger.LogInfo("[RoR2DevTool] Generating mock game state data...");

                // Create mock game state
                var mockGameState = new GameState
                {
                    IsInRun = true,
                    CurrentStage = "golemplains",
                    StageNumber = 1,
                    GameTime = 1234.56f,
                    DifficultyCoefficient = 2.75f,
                    TotalEnemiesAlive = 12,
                    TeamMoney = 1250,
                    Players = GenerateMockPlayers(),
                    Teleporter = new TeleporterData
                    {
                        IsActive = true,
                        IsCharged = false,
                        ChargeProgress = 0.65f,
                        BossEventActive = false,
                        EnemiesRemaining = 5
                    },
                    Interactables = GenerateMockInteractables(),
                    Monsters = GenerateMockMonsters()
                };

                // Note: GameStateService always reads live game state
                // This command is primarily for UI testing via the browser console
                logger.LogInfo("[RoR2DevTool] Mock game state data generated (for reference)");
                logger.LogInfo("[RoR2DevTool] Note: GameStateService reads live game state, not mock data");
                logger.LogInfo($"[RoR2DevTool] - {mockGameState.Players.Count} mock players created");
                logger.LogInfo($"[RoR2DevTool] - {mockGameState.Interactables.Count} mock interactables created");
                logger.LogInfo($"[RoR2DevTool] - {mockGameState.Monsters.Count} mock monsters created");
            }
            catch (Exception ex)
            {
                logger.LogError($"[RoR2DevTool] Failed to generate mock game state: {ex.Message}");
            }
        }

        private List<PlayerData> GenerateMockPlayers()
        {
            var players = new List<PlayerData>();
            var characterBodies = new[] { "CommandoBody", "HuntressBody", "Bandit2Body", "ToolbotBody" };
            var playerNames = new[] { "TestPlayer1", "DevUser", "MockPlayer", "UITester" };

            for (int i = 0; i < 4; i++)
            {
                var isAlive = i < 3; // Make one player dead for testing
                var health = isAlive ? UnityEngine.Random.Range(50f, 100f) : 0f;
                var maxHealth = 100f + (i * 25f);

                players.Add(new PlayerData
                {
                    PlayerId = i + 1,
                    PlayerName = playerNames[i],
                    CurrentCharacter = characterBodies[i],
                    CharacterIcon = GetCharacterIcon(characterBodies[i]),
                    Level = UnityEngine.Random.Range(1, 15),
                    Experience = UnityEngine.Random.Range(0f, 1000f),
                    Health = health,
                    MaxHealth = maxHealth,
                    MaxShield = isAlive ? UnityEngine.Random.Range(0f, 25f) : 0f,
                    HealthRegen = UnityEngine.Random.Range(1f, 5f),
                    IsAlive = isAlive,
                    GodModeEnabled = i == 0, // Give first player god mode
                    BaseDamage = UnityEngine.Random.Range(12f, 25f),
                    Armor = UnityEngine.Random.Range(0f, 20f),
                    AttackSpeed = UnityEngine.Random.Range(1f, 2.5f),
                    CritChance = UnityEngine.Random.Range(5f, 25f),
                    MoveSpeed = UnityEngine.Random.Range(7f, 12f),
                    JumpPower = UnityEngine.Random.Range(15f, 20f),
                    Items = GenerateMockItems(i)
                });
            }

            return players;
        }

        private Dictionary<string, int> GenerateMockItems(int playerIndex)
        {
            var items = new Dictionary<string, int>();
            var commonItems = new[] { "Syringe", "Bear", "Crowbar", "Mushroom", "Hoof" };
            var uncommonItems = new[] { "Whip", "ATG", "Ukulele", "Infusion", "Bandolier" };
            var legendaryItems = new[] { "Dagger", "Behemoth", "Brainstalks", "Clover", "Headstompers" };

            // Add some common items
            for (int i = 0; i < UnityEngine.Random.Range(3, 6); i++)
            {
                var item = commonItems[UnityEngine.Random.Range(0, commonItems.Length)];
                items[item] = UnityEngine.Random.Range(1, 8);
            }

            // Add some uncommon items
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
            {
                var item = uncommonItems[UnityEngine.Random.Range(0, uncommonItems.Length)];
                items[item] = UnityEngine.Random.Range(1, 4);
            }

            // Maybe add a legendary item
            if (UnityEngine.Random.Range(0f, 1f) < 0.3f)
            {
                var item = legendaryItems[UnityEngine.Random.Range(0, legendaryItems.Length)];
                items[item] = 1;
            }

            return items;
        }

        private List<InteractableData> GenerateMockInteractables()
        {
            var interactables = new List<InteractableData>();
            var interactableTypes = new[] 
            { 
                "Chest1", "Chest2", "GoldChest", "EquipmentBarrel", 
                "Scrapper", "Duplicator", "ShrineBoss", "ShrineChance" 
            };

            for (int i = 0; i < UnityEngine.Random.Range(8, 15); i++)
            {
                interactables.Add(new InteractableData
                {
                    Name = interactableTypes[UnityEngine.Random.Range(0, interactableTypes.Length)],
                    PositionX = UnityEngine.Random.Range(-50f, 50f),
                    PositionY = UnityEngine.Random.Range(0f, 20f),
                    PositionZ = UnityEngine.Random.Range(-50f, 50f),
                    IsAvailable = UnityEngine.Random.Range(0f, 1f) > 0.3f,
                    DisplayName = interactableTypes[UnityEngine.Random.Range(0, interactableTypes.Length)],
                    Distance = UnityEngine.Random.Range(10f, 100f),
                    NetworkId = i
                });
            }

            return interactables;
        }

        private List<MonsterData> GenerateMockMonsters()
        {
            var monsters = new List<MonsterData>();
            var monsterTypes = new[] 
            { 
                "Beetle", "BeetleGuard", "Wisp", "Lemurian", "Golem",
                "ClayBruiser", "Vulture", "Bison", "Parent" 
            };

            for (int i = 0; i < UnityEngine.Random.Range(10, 20); i++)
            {
                var maxHealth = UnityEngine.Random.Range(100f, 500f);
                var health = UnityEngine.Random.Range(maxHealth * 0.2f, maxHealth);

                monsters.Add(new MonsterData
                {
                    Name = monsterTypes[UnityEngine.Random.Range(0, monsterTypes.Length)],
                    PositionX = UnityEngine.Random.Range(-100f, 100f),
                    PositionY = UnityEngine.Random.Range(0f, 30f),
                    PositionZ = UnityEngine.Random.Range(-100f, 100f),
                    Health = health,
                    MaxHealth = maxHealth,
                    IsElite = UnityEngine.Random.Range(0f, 1f) < 0.2f,
                    DisplayName = monsterTypes[UnityEngine.Random.Range(0, monsterTypes.Length)],
                    IsAlive = true,
                    Distance = UnityEngine.Random.Range(20f, 150f),
                    NetworkId = i
                });
            }

            return monsters;
        }

        private string GetCharacterIcon(string characterBody)
        {
            return characterBody switch
            {
                "CommandoBody" => "🔫",
                "HuntressBody" => "🏹",
                "Bandit2Body" => "🔪",
                "ToolbotBody" => "🤖",
                "EngiBody" => "🔧",
                "MageBody" => "🔮",
                "MercBody" => "⚔️",
                "TreebotBody" => "🌱",
                "LoaderBody" => "👊",
                "CrocoBody" => "🦎",
                "CaptainBody" => "⚓",
                _ => "👤"
            };
        }
    }
}