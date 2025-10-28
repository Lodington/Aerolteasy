using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2DevTool.Commands;
using RoR2DevTool.Commands.DebugCommands;
using RoR2DevTool.Commands.GameCommands;
using RoR2DevTool.Commands.ItemCommands;
using RoR2DevTool.Commands.MonsterCommands;
using RoR2DevTool.Commands.PlayerCommands;
using RoR2DevTool.Commands.SpawningCommands;
using RoR2DevTool.Commands.TeleporterCommands;
using RoR2DevTool.Commands.ESPCommands;
using RoR2DevTool.Core;

namespace RoR2DevTool.Services
{
    public class CommandProcessor
    {
        private readonly ManualLogSource logger;
        private readonly GameStateService gameStateService;
        private ESPOverlayService espOverlayService;
        private readonly Queue<DevCommand> commandQueue = new Queue<DevCommand>();
        private readonly object commandLock = new object();
        private readonly Dictionary<string, IDevCommand> commands = new Dictionary<string, IDevCommand>();

        public CommandProcessor(ManualLogSource logger, GameStateService gameStateService)
        {
            this.logger = logger;
            this.gameStateService = gameStateService;
            RegisterCommands();
        }

        public void SetESPOverlayService(ESPOverlayService espOverlayService)
        {
            this.espOverlayService = espOverlayService;
            ESPOverlayServiceHolder.Instance = espOverlayService;
        }

        private void RegisterCommands()
        {
            // Player Commands
            RegisterCommand(new GodModeCommand());
            RegisterCommand(new ChangePlayerCommand());
            RegisterCommand(new SetHealthCommand());
            RegisterCommand(new SetLevelCommand());
            RegisterCommand(new KillPlayerCommand());
            RegisterCommand(new RevivePlayerCommand());
            RegisterCommand(new SetPlayerStatsCommand());
            RegisterCommand(new TeleportPlayerCommand());
            
            // Item Commands
            RegisterCommand(new SpawnItemCommand());
            
            // Game Commands
            RegisterCommand(new ChangeStageCommand());
            RegisterCommand(new SetMoneyCommand());
            
            // Spawning Commands
            RegisterCommand(new SpawnMonsterCommand());
            RegisterCommand(new SpawnInteractableCommand());
            
            // Monster Commands
            RegisterCommand(new GiveMonsterItemCommand());
            RegisterCommand(new GiveMonsterBuffCommand());
            
            // Teleporter Commands
            RegisterCommand(new ChargeTeleporterCommand());
            RegisterCommand(new ActivateTeleporterCommand());
            RegisterCommand(new SkipTeleporterEventCommand());
            RegisterCommand(new SpawnTeleporterCommand());
            
            // Debug Commands
            RegisterCommand(new DebugPlayerItemsCommand());
            RegisterCommand(new DebugItemsCommand());
            RegisterCommand(new DebugInteractablesCommand());
            RegisterCommand(new DebugCharacterIconsCommand());
            RegisterCommand(new DebugMonstersCommand());
            RegisterCommand(new DebugInteractableSpawnCardsCommand());
            RegisterCommand(new DebugItemCatalogCommand());
            RegisterCommand(new DebugCharacterDefaultsCommand());
            RegisterCommand(new DebugESPDataCommand());
            RegisterCommand(new TestESPDataCommand());
            RegisterCommand(new TestESPOverlayCommand());
            RegisterCommand(new DisableESPOverlayCommand());
            RegisterCommand(new MockGameStateCommand());
            RegisterCommand(new ClearMockDataCommand());

            // ESP Overlay Commands
            RegisterCommand(new ToggleESPOverlayCommand());
            RegisterCommand(new ConfigureESPOverlayCommand());

            RegisterCommand(new RefreshStateCommand());
            RegisterCommand(new SetVerboseLoggingCommand());
            
            logger.LogInfo($"Registered {commands.Count} commands");
        }

        private void RegisterCommand(IDevCommand command)
        {
            commands[command.CommandName.ToLower()] = command;
            logger.LogDebug($"Registered command: {command.CommandName}");
        }

        public void EnqueueCommand(DevCommand command)
        {
            lock (commandLock)
            {
                commandQueue.Enqueue(command);
            }
        }

        public void ProcessCommands()
        {
            lock (commandLock)
            {
                while (commandQueue.Count > 0)
                {
                    var command = commandQueue.Dequeue();
                    ExecuteCommand(command);
                }
            }
        }

        private void ExecuteCommand(DevCommand command)
        {
            logger.LogInfo($"Executing command: {command.Type}");
            
            var commandKey = command.Type.ToLower();
            if (commands.TryGetValue(commandKey, out var commandHandler))
            {
                try
                {
                    commandHandler.Execute(command, logger, gameStateService);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error executing command {command.Type}: {ex.Message}");
                    logger.LogError(ex.StackTrace);
                }
            }
            else
            {
                logger.LogWarning($"Unknown command: {command.Type}");
            }
        }

    }
}