using System;
using System.Net;
using BepInEx.Logging;

namespace RoR2DevTool.Services.Endpoints
{
    public class SSEEndpoint : IApiEndpoint
    {
        private readonly SSEService sseService;
        private readonly ManualLogSource logger;

        public string Path => "/api/events";

        public SSEEndpoint(SSEService sseService, ManualLogSource logger)
        {
            this.sseService = sseService;
            this.logger = logger;
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != "GET")
            {
                response.StatusCode = 405;
                response.Close();
                return;
            }

            try
            {
                // This will keep the connection open and stream events
                sseService.HandleSSEConnection(response);
                // Note: response is NOT closed here - SSE keeps it open
            }
            catch (Exception ex)
            {
                logger.LogError($"SSE endpoint error: {ex.Message}");
                response.StatusCode = 500;
                response.Close();
            }
        }
    }
}
