using System;
using System.Net;
using System.Text;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace RoR2DevTool.Services.Endpoints
{
    public class StatusEndpoint : IApiEndpoint
    {
        private readonly ManualLogSource logger;

        public string Path => "/api/status";

        public StatusEndpoint(ManualLogSource logger)
        {
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
                var statusObj = new 
                { 
                    connected = true, 
                    gameRunning = RoR2.Run.instance != null,
                    playerAlive = RoR2.LocalUserManager.GetFirstLocalUser()?.cachedBody != null,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                var json = JsonConvert.SerializeObject(statusObj);
                response.ContentType = "application/json";
                var buffer = Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error getting status: {ex.Message}");
                var errorObj = new { connected = true, error = "Status unavailable" };
                var json = JsonConvert.SerializeObject(errorObj);
                
                response.ContentType = "application/json";
                var buffer = Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
