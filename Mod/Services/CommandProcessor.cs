using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using RoR2DevTool.Commands;
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
            // Automatically discover and register all commands using reflection
            var assembly = typeof(IDevCommand).Assembly;
            var commandTypes = assembly.GetTypes()
                .Where(t => typeof(IDevCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var commandType in commandTypes)
            {
                try
                {
                    var command = (IDevCommand)Activator.CreateInstance(commandType);
                    RegisterCommand(command);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to register command {commandType.Name}: {ex.Message}");
                }
            }
            
            logger.LogInfo($"Registered {commands.Count} commands automatically");
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

        public Dictionary<string, IDevCommand> GetAllCommands()
        {
            return new Dictionary<string, IDevCommand>(commands);
        }

        public bool CommandExists(string commandName)
        {
            return commands.ContainsKey(commandName.ToLower());
        }

    }
}