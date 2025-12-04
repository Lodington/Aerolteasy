using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using RoR2DevTool.Models;
using RoR2DevTool.Services.Helpers;
using RoR2DevTool.Services.Interfaces;

namespace RoR2DevTool.Services
{
    public class GameStateService
    {
        private readonly ManualLogSource logger;
        private readonly IPlayerDataCollector playerCollector;
        private readonly IWorldDataCollector worldCollector;
        private readonly IRunDataCollector runCollector;
        private bool verboseLogging = false;
        private List<object> itemCatalog = new List<object>();
        private List<object> characterDefaults = new List<object>();

        public GameStateService(ManualLogSource logger)
        {
            this.logger = logger;
            this.playerCollector = new PlayerDataCollector(logger);
            this.worldCollector = new WorldDataCollector(logger, verboseLogging);
            this.runCollector = new RunDataCollector(logger, verboseLogging);
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

        public List<object> GetItemCatalog() => itemCatalog;

        public void SetCharacterDefaults(List<object> defaults)
        {
            characterDefaults = defaults;
            logger.LogInfo($"Character defaults updated with {defaults.Count} characters");
        }

        public List<object> GetCharacterDefaults() => characterDefaults;

        public GameState GetGameState()
        {
            var gameState = new GameState();

            try
            {
                gameState.Players = playerCollector.GetAllPlayers();
                gameState.IsInRun = runCollector.IsInRun();

                if (gameState.IsInRun)
                {
                    var runData = runCollector.GetRunData();
                    gameState.CurrentStage = runData.CurrentStage;
                    gameState.StageNumber = runData.StageNumber;
                    gameState.DifficultyCoefficient = runData.DifficultyCoefficient;
                    gameState.GameTime = runData.GameTime;
                    gameState.IsPaused = runData.IsPaused;
                    gameState.TimeScale = runData.TimeScale;
                    gameState.Seed = runData.Seed;

                    gameState.Teleporter = worldCollector.GetTeleporterData();
                    gameState.Monsters = worldCollector.GetMonsters();
                    gameState.Interactables = worldCollector.GetInteractables();
                    gameState.Items = worldCollector.GetWorldItems();

                    gameState.TotalEnemiesAlive = gameState.Monsters.Count;
                }

                gameState.SurvivorCount = gameState.Players.Count(p => p.IsAlive);
                CalculateAverageStats(gameState);
                gameState.WeatherType = "Clear";
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

        private void CalculateAverageStats(GameState gameState)
        {
            if (gameState.Players.Count > 0)
            {
                try
                {
                    gameState.AverageDamage = gameState.Players
                        .Where(p => p.Level > 0)
                        .DefaultIfEmpty()
                        .Average(p => p?.Level * 100 ?? 0);

                    gameState.AverageHealing = gameState.Players
                        .Where(p => p.MaxHealth > 0)
                        .DefaultIfEmpty()
                        .Average(p => Math.Max(0, (p?.MaxHealth ?? 0) - (p?.Health ?? 0)));
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Error calculating stats: {ex.Message}");
                }
            }
        }

        public void SetPlayerGodMode(int playerId, bool enabled)
        {
            playerCollector.SetPlayerGodMode(playerId, enabled);
        }

        public void ApplyGodMode()
        {
            playerCollector.ApplyGodMode();
        }
    }
}