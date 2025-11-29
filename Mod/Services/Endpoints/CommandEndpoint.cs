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
        private readonly ManualLogSource logger;

        public string Path => "/api/command";

        public CommandEndpoint(NetworkingService networkingService, PermissionService permissionService, ManualLogSource logger)
        {
            this.networkingService = networkingService;
            this.permissionService = permissionService;
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
            // Define all available commands with their metadata
            return new List<CommandInfo>
            {
                // Player Commands
                new CommandInfo { name = "godmode", category = "Player", permission = "Advanced", description = "Toggle god mode for a player" },
                new CommandInfo { name = "changeplayer", category = "Player", permission = "Advanced", description = "Change player character" },
                new CommandInfo { name = "sethealth", category = "Player", permission = "Basic", description = "Set player health percentage" },
                new CommandInfo { name = "setlevel", category = "Player", permission = "Basic", description = "Set player level" },
                new CommandInfo { name = "setplayerstats", category = "Player", permission = "Advanced", description = "Set player stats (damage, armor, etc.)" },
                new CommandInfo { name = "killplayer", category = "Player", permission = "Admin", description = "Kill a player" },
                new CommandInfo { name = "reviveplayer", category = "Player", permission = "Admin", description = "Revive a dead player" },
                new CommandInfo { name = "teleportplayer", category = "Player", permission = "Advanced", description = "Teleport player to coordinates" },
                
                // Item Commands
                new CommandInfo { name = "spawnitem", category = "Items", permission = "Basic", description = "Spawn items for a player" },
                
                // Game Commands
                new CommandInfo { name = "setmoney", category = "Game", permission = "Basic", description = "Set team money" },
                new CommandInfo { name = "changestage", category = "Game", permission = "Admin", description = "Change to a different stage" },
                
                // Monster Commands
                new CommandInfo { name = "spawnmonster", category = "Monsters", permission = "Advanced", description = "Spawn a monster" },
                new CommandInfo { name = "givemonsteritem", category = "Monsters", permission = "Advanced", description = "Give item to monsters" },
                new CommandInfo { name = "givemonsterbuff", category = "Monsters", permission = "Advanced", description = "Give buff to monsters" },
                
                // Teleporter Commands
                new CommandInfo { name = "chargeteleporter", category = "Teleporter", permission = "Admin", description = "Set teleporter charge" },
                new CommandInfo { name = "activateteleporter", category = "Teleporter", permission = "Admin", description = "Activate the teleporter" },
                new CommandInfo { name = "skipteleporterevent", category = "Teleporter", permission = "Admin", description = "Skip teleporter event" },
                new CommandInfo { name = "spawnteleporter", category = "Teleporter", permission = "Admin", description = "Spawn a teleporter" },
                
                // ESP Commands
                new CommandInfo { name = "toggleespoverlay", category = "ESP", permission = "Basic", description = "Toggle ESP overlay" },
                new CommandInfo { name = "configureespoverlay", category = "ESP", permission = "Basic", description = "Configure ESP settings" },
                new CommandInfo { name = "testespoverlay", category = "ESP", permission = "Basic", description = "Test ESP overlay" },
                new CommandInfo { name = "disableespoverlay", category = "ESP", permission = "Basic", description = "Disable ESP overlay" },
                
                // Debug Commands
                new CommandInfo { name = "refreshstate", category = "Debug", permission = "ReadOnly", description = "Refresh game state" },
                new CommandInfo { name = "debugitems", category = "Debug", permission = "ReadOnly", description = "Log all items to console" },
                new CommandInfo { name = "debuginteractables", category = "Debug", permission = "ReadOnly", description = "Log all interactables" },
                new CommandInfo { name = "debugmonsters", category = "Debug", permission = "ReadOnly", description = "Log all monsters" },
                new CommandInfo { name = "debugplayeritems", category = "Debug", permission = "ReadOnly", description = "Log player items" },
                new CommandInfo { name = "debugcharactericons", category = "Debug", permission = "ReadOnly", description = "Log character icons" },
                new CommandInfo { name = "debugitemcatalog", category = "Debug", permission = "ReadOnly", description = "Refresh item catalog" },
                new CommandInfo { name = "debugcharacterdefaults", category = "Debug", permission = "ReadOnly", description = "Refresh character defaults" }
            };
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
