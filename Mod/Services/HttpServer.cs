using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using BepInEx.Logging;
using RoR2DevTool.Services.Endpoints;

namespace RoR2DevTool.Services
{
    public class HttpServer
    {
        private HttpListener httpListener;
        private Thread httpThread;
        private bool isRunning = false;
        private int serverPort = 8080;
        private ManualLogSource logger;
        private Dictionary<string, IApiEndpoint> endpoints;
        private SSEService sseService;

        public HttpServer(ManualLogSource logger, GameStateService gameStateService, NetworkingService networkingService, PermissionService permissionService, SSEService sseService)
        {
            this.logger = logger;
            this.sseService = sseService;
            
            // Register all endpoints
            endpoints = new Dictionary<string, IApiEndpoint>(StringComparer.OrdinalIgnoreCase)
            {
                { "/api/gamestate", new GameStateEndpoint(gameStateService, logger) },
                { "/api/command", new CommandEndpoint(networkingService, permissionService, logger) },
                { "/api/status", new StatusEndpoint(logger) },
                { "/api/itemcatalog", new ItemCatalogEndpoint(gameStateService, logger) },
                { "/api/characterdefaults", new CharacterDefaultsEndpoint(gameStateService, logger) },
                { "/api/network/status", new NetworkStatusEndpoint(permissionService, logger) },
                { "/api/permissions", new PermissionsEndpoint(permissionService, logger) },
                { "/api/permissions/request", new RequestPermissionEndpoint(networkingService, logger) },
                { "/api/events", new SSEEndpoint(sseService, logger) }
            };
        }

        public void Start()
        {
            try
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://localhost:{serverPort}/");
                httpListener.Start();
                isRunning = true;

                httpThread = new Thread(HandleHttpRequests)
                {
                    IsBackground = true
                };
                httpThread.Start();

                logger.LogInfo($"HTTP Server started on port {serverPort}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to start HTTP server: {ex.Message}");
            }
        }

        public void Stop()
        {
            isRunning = false;
            httpListener?.Stop();
            httpThread?.Join(1000);
        }

        private void HandleHttpRequests()
        {
            while (isRunning && httpListener.IsListening)
            {
                try
                {
                    var context = httpListener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch (Exception ex)
                {
                    if (isRunning)
                    {
                        logger.LogError($"HTTP listener error: {ex.Message}");
                    }
                }
            }
        }

        private void ProcessRequest(object contextObj)
        {
            var context = (HttpListenerContext)contextObj;
            var request = context.Request;
            var response = context.Response;
            bool shouldCloseResponse = true;

            try
            {
                // Enable CORS
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, DELETE, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string path = request.Url.AbsolutePath.ToLower();
                
                if (endpoints.TryGetValue(path, out var endpoint))
                {
                    // SSE connections stay open - don't close them
                    if (path == "/api/events")
                    {
                        shouldCloseResponse = false;
                    }
                    
                    endpoint.HandleRequest(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing request: {ex.Message}");
                response.StatusCode = 500;
            }
            finally
            {
                if (shouldCloseResponse)
                {
                    response.Close();
                }
            }
        }
    }
}