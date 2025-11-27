using System;
using System.IO;
using System.Net;
using System.Text;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services.Endpoints
{
    public class RequestPermissionEndpoint : IApiEndpoint
    {
        private readonly NetworkingService networkingService;
        private readonly ManualLogSource logger;

        public string Path => "/api/permissions/request";

        public RequestPermissionEndpoint(NetworkingService networkingService, ManualLogSource logger)
        {
            this.networkingService = networkingService;
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
                var data = JsonConvert.DeserializeObject<PermissionRequestDto>(json);
                
                if (data == null || string.IsNullOrEmpty(data.level))
                {
                    response.StatusCode = 400;
                    var errorObj = new { success = false, message = "Missing level parameter" };
                    var errorJson = JsonConvert.SerializeObject(errorObj);
                    var buffer = Encoding.UTF8.GetBytes(errorJson);
                    response.ContentType = "application/json";
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    return;
                }
                
                if (Enum.TryParse<PermissionLevel>(data.level, true, out var level))
                {
                    networkingService.RequestPermission(level);
                    
                    var responseObj = new
                    {
                        success = true,
                        message = $"Permission request sent for level: {level}"
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

        private class PermissionRequestDto
        {
            public string level { get; set; }
        }
    }
}
