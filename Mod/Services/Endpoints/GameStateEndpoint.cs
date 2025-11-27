using System;
using System.Net;
using System.Text;
using BepInEx.Logging;
using Newtonsoft.Json;

namespace RoR2DevTool.Services.Endpoints
{
    public class GameStateEndpoint : IApiEndpoint
    {
        private readonly GameStateService gameStateService;
        private readonly ManualLogSource logger;

        public string Path => "/api/gamestate";

        public GameStateEndpoint(GameStateService gameStateService, ManualLogSource logger)
        {
            this.gameStateService = gameStateService;
            this.logger = logger;
        }

        public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != "GET")
            {
                response.StatusCode = 405;
                return;
            }

            var gameState = gameStateService.GetGameState();
            var json = JsonConvert.SerializeObject(gameState, Formatting.Indented);
            
            response.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
