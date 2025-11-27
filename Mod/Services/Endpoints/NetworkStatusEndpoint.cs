using System;
using System.Net;
using System.Text;
using System.Linq;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2;
using UnityEngine.Networking;

namespace RoR2DevTool.Services.Endpoints
{
    public class NetworkStatusEndpoint : IApiEndpoint
    {
        private readonly PermissionService permissionService;
        private readonly ManualLogSource logger;

        public string Path => "/api/network/status";

        public NetworkStatusEndpoint(PermissionService permissionService, ManualLogSource logger)
        {
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != "GET")
            {
                response.StatusCode = 405;
                return;
            }

            try
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                string currentUserId = localUser?.currentNetworkUser?.id.value.ToString();
                
                var statusObj = new
                {
                    success = true,
                    isHost = NetworkServer.active,
                    isClient = NetworkClient.active,
                    isConnected = NetworkClient.active || NetworkServer.active,
                    currentUserId = currentUserId,
                    currentUserName = localUser?.currentNetworkUser?.userName,
                    currentPermission = currentUserId != null 
                        ? permissionService.GetPermission(currentUserId).ToString() 
                        : "None",
                    isCurrentUserHost = currentUserId != null && permissionService.IsHost(currentUserId),
                    connectedPlayers = NetworkUser.readOnlyInstancesList.Count,
                    players = NetworkUser.readOnlyInstancesList.Select(u => new
                    {
                        userId = u.id.value.ToString(),
                        userName = u.userName,
                        permission = permissionService.GetPermission(u.id.value.ToString()).ToString(),
                        isHost = permissionService.IsHost(u.id.value.ToString())
                    }).ToList()
                };

                var json = JsonConvert.SerializeObject(statusObj, Formatting.Indented);
                response.ContentType = "application/json";
                var buffer = Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting network status: {ex.Message}");
                response.StatusCode = 500;
            }
        }
    }
}
