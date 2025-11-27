using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2DevTool.Models;
using RoR2;

namespace RoR2DevTool.Services.Endpoints
{
    public class PermissionsEndpoint : IApiEndpoint
    {
        private readonly PermissionService permissionService;
        private readonly ManualLogSource logger;

        public string Path => "/api/permissions";

        public PermissionsEndpoint(PermissionService permissionService, ManualLogSource logger)
        {
            this.permissionService = permissionService;
            this.logger = logger;
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod == "GET")
            {
                HandleGetPermissions(response);
            }
            else if (request.HttpMethod == "POST")
            {
                HandleSetPermission(request, response);
            }
            else if (request.HttpMethod == "DELETE")
            {
                HandleRevokePermission(request, response);
            }
            else
            {
                response.StatusCode = 405;
            }
        }

        private void HandleGetPermissions(HttpListenerResponse response)
        {
            var permissions = permissionService.GetAllPermissions();
            var users = NetworkUser.readOnlyInstancesList
                .Select(u => new
                {
                    userId = u.id.value.ToString(),
                    userName = u.userName,
                    permission = permissions.ContainsKey(u.id.value.ToString()) 
                        ? permissions[u.id.value.ToString()].ToString() 
                        : "None",
                    isHost = permissionService.IsHost(u.id.value.ToString())
                })
                .ToList();

            var responseObj = new
            {
                success = true,
                users = users,
                hostId = permissionService.GetHostUserId()
            };

            var json = JsonConvert.SerializeObject(responseObj, Formatting.Indented);
            response.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        private void HandleSetPermission(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                string json = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<SetPermissionDto>(json);
                
                if (data == null || string.IsNullOrEmpty(data.userId) || string.IsNullOrEmpty(data.level))
                {
                    response.StatusCode = 400;
                    var errorObj = new { success = false, message = "Missing userId or level parameter" };
                    var errorJson = JsonConvert.SerializeObject(errorObj);
                    var buffer = Encoding.UTF8.GetBytes(errorJson);
                    response.ContentType = "application/json";
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    return;
                }
                
                if (Enum.TryParse<PermissionLevel>(data.level, true, out var level))
                {
                    permissionService.SetPermission(data.userId, level);
                    
                    var responseObj = new
                    {
                        success = true,
                        message = $"Permission set to {level} for user {data.userId}"
                    };
                    
                    var responseJson = JsonConvert.SerializeObject(responseObj);
                    response.ContentType = "application/json";
                    var buffer = Encoding.UTF8.GetBytes(responseJson);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = 400;
                    var errorObj = new { success = false, message = "Invalid permission level" };
                    var errorJson = JsonConvert.SerializeObject(errorObj);
                    var buffer = Encoding.UTF8.GetBytes(errorJson);
                    response.ContentType = "application/json";
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private class SetPermissionDto
        {
            public string userId { get; set; }
            public string level { get; set; }
        }

        private void HandleRevokePermission(HttpListenerRequest request, HttpListenerResponse response)
        {
            string userId = request.QueryString["userId"];
            
            if (!string.IsNullOrEmpty(userId))
            {
                permissionService.RevokePermission(userId);
                
                var responseObj = new
                {
                    success = true,
                    message = $"Permission revoked for user {userId}"
                };
                
                var json = JsonConvert.SerializeObject(responseObj);
                response.ContentType = "application/json";
                var buffer = Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = 400;
            }
        }
    }
}
