using System;
using System.IO;
using System.Net;
using System.Text;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2;
using RoR2DevTool.Core;

namespace RoR2DevTool.Services.Endpoints
{
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
            if (request.HttpMethod != "POST")
            {
                response.StatusCode = 405;
                return;
            }

            using (var reader = new StreamReader(request.InputStream))
            {
                string json = reader.ReadToEnd();
                var command = JsonConvert.DeserializeObject<DevCommand>(json);
                
                // Get current user info
                var localUser = LocalUserManager.GetFirstLocalUser();
                string userId = localUser?.currentNetworkUser?.id.value.ToString() ?? "local";
                string userName = localUser?.currentNetworkUser?.userName ?? "LocalUser";

                // Check permissions
                if (!permissionService.HasPermission(userId, command.Type))
                {
                    response.StatusCode = 403;
                    var errorObj = new 
                    { 
                        success = false, 
                        message = $"Insufficient permissions for command: {command.Type}",
                        requiredPermission = "Higher level required"
                    };
                    var errorJson = JsonConvert.SerializeObject(errorObj);
                    response.ContentType = "application/json";
                    var buffer = Encoding.UTF8.GetBytes(errorJson);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    return;
                }

                // Send command through networking service
                networkingService.SendCommand(command, userId, userName);
                
                var responseObj = new { success = true, message = "Command sent" };
                var responseJson = JsonConvert.SerializeObject(responseObj);
                
                response.ContentType = "application/json";
                var responseBuffer = Encoding.UTF8.GetBytes(responseJson);
                response.ContentLength64 = responseBuffer.Length;
                response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
            }
        }
    }
}
