using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services.Endpoints
{
    public class CommandInfo
    {
        public string name { get; set; }
        public string category { get; set; }
        public string permission { get; set; }
        public string description { get; set; }
    }

    public class CommandEndpoint : IApiEndpoint
    {
        private readonly NetworkingService networkingService;
        private readonly PermissionService permissionService;
        private readonly CommandProcessor commandProcessor;
        private readonly ManualLogSource logger;

        public string Path => "/api/command";

        public CommandEndpoint(NetworkingService networkingService, PermissionService permissionService, CommandProcessor commandProcessor, ManualLogSource logger)
        {
            this.networkingService = networkingService;
            this.permissionService = permissionService;
            this.commandProcessor = commandProcessor;
            this.logger = logger;
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                if (request.HttpMethod == "GET")
                {
                    HandleGetCommands(response);
                    return;
                }

                if (request.HttpMethod != "POST")
                {
                    SendErrorResponse(response, 405, "Method not allowed. Use POST to execute commands or GET to list available commands.");
                    return;
                }

                HandlePostCommand(request, response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error handling command request: {ex.Message}");
                SendErrorResponse(response, 500, "Internal server error", ex.Message);
            }
        }

        private void HandleGetCommands(HttpListenerResponse response)
        {
            var commands = GetAvailableCommands();
            var responseObj = new
            {
                success = true,
                commands = commands,
                count = commands.Count
            };

            SendJsonResponse(response, responseObj, 200);
        }

        private void HandlePostCommand(HttpListenerRequest request, HttpListenerResponse response)
        {
            string json;
            using (var reader = new StreamReader(request.InputStream))
            {
                json = reader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                SendErrorResponse(response, 400, "Request body is empty");
                return;
            }

            DevCommand command;
            try
            {
                command = JsonConvert.DeserializeObject<DevCommand>(json);
            }
            catch (JsonException ex)
            {
                SendErrorResponse(response, 400, "Invalid JSON format", ex.Message);
                return;
            }

            // Validate command
            if (command == null || string.IsNullOrWhiteSpace(command.Type))
            {
                SendErrorResponse(response, 400, "Command type is required");
                return;
            }

            // Get current user info
            var localUser = LocalUserManager.GetFirstLocalUser();
            string userId = localUser?.currentNetworkUser?.id.value.ToString() ?? "local";
            string userName = localUser?.currentNetworkUser?.userName ?? "LocalUser";

            // Check if command exists
            var availableCommands = GetAvailableCommands();
            var commandInfo = availableCommands.FirstOrDefault(c => 
                c.name.Equals(command.Type, StringComparison.OrdinalIgnoreCase));

            if (commandInfo == null)
            {
                SendErrorResponse(response, 404, $"Unknown command: {command.Type}", 
                    $"Available commands: {string.Join(", ", availableCommands.Select(c => c.name))}");
                return;
            }

            // Check permissions
            var userPermission = permissionService.GetPermission(userId);
            var requiredPermission = commandInfo.permission;
            
            if (!permissionService.HasPermission(userId, command.Type))
            {
                SendErrorResponse(response, 403, 
                    $"Insufficient permissions for command: {command.Type}",
                    $"Your permission: {userPermission}, Required: {requiredPermission}");
                return;
            }

            // Send command through networking service
            try
            {
                networkingService.SendCommand(command, userId, userName);
                
                var responseObj = new
                {
                    success = true,
                    message = "Command executed successfully",
                    command = new
                    {
                        type = command.Type,
                        executedBy = userName,
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                };

                SendJsonResponse(response, responseObj, 200);
                logger.LogInfo($"Command '{command.Type}' executed by {userName} ({userId})");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error executing command '{command.Type}': {ex.Message}");
                SendErrorResponse(response, 500, "Command execution failed", ex.Message);
            }
        }

        private List<CommandInfo> GetAvailableCommands()
        {
            // Dynamically get all registered commands from CommandProcessor
            var registeredCommands = commandProcessor.GetAllCommands();
            var commandInfoList = new List<CommandInfo>();

            foreach (var kvp in registeredCommands)
            {
                var commandName = kvp.Key;
                var command = kvp.Value;
                
                // Determine category and permission based on command type/namespace
                string category = DetermineCategory(command);
                string permission = DeterminePermission(commandName);
                string description = $"{command.CommandName} command";

                commandInfoList.Add(new CommandInfo
                {
                    name = commandName,
                    category = category,
                    permission = permission,
                    description = description
                });
            }

            return commandInfoList;
        }

        private string DetermineCategory(Commands.IDevCommand command)
        {
            var typeName = command.GetType().Namespace ?? "";
            
            if (typeName.Contains("PlayerCommands")) return "Player";
            if (typeName.Contains("ItemCommands")) return "Items";
            if (typeName.Contains("GameCommands")) return "Game";
            if (typeName.Contains("MonsterCommands")) return "Monsters";
            if (typeName.Contains("TeleporterCommands")) return "Teleporter";
            if (typeName.Contains("SpawningCommands")) return "Spawning";
            if (typeName.Contains("ESPCommands")) return "ESP";
            if (typeName.Contains("DebugCommands")) return "Debug";
            
            return "Other";
        }

        private string DeterminePermission(string commandName)
        {
            // Admin commands
            if (commandName.Contains("kill") || commandName.Contains("changestage") || 
                commandName.Contains("teleporter") || commandName.Contains("revive"))
                return "Admin";
            
            // Advanced commands
            if (commandName.Contains("godmode") || commandName.Contains("changeplayer") || 
                commandName.Contains("stats") || commandName.Contains("monster") || 
                commandName.Contains("teleport"))
                return "Advanced";
            
            // Debug/ReadOnly commands
            if (commandName.Contains("debug") || commandName.Contains("refresh") || 
                commandName.Contains("test") || commandName.Contains("mock") || commandName.Contains("clear"))
                return "ReadOnly";
            
            // Everything else is Basic
            return "Basic";
        }

        private void SendJsonResponse(HttpListenerResponse response, object data, int statusCode = 200)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void SendErrorResponse(HttpListenerResponse response, int statusCode, string message, string details = null)
        {
            var errorObj = new
            {
                success = false,
                error = message,
                details = details,
                statusCode = statusCode,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            SendJsonResponse(response, errorObj, statusCode);
        }
    }
}
