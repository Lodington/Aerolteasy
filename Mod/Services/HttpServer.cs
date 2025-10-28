using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2DevTool.Core;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services
{
    public class HttpServer
    {
        private HttpListener httpListener;
        private Thread httpThread;
        private bool isRunning = false;
        private int serverPort = 8080;
        private ManualLogSource logger;
        private GameStateService gameStateService;
        private CommandProcessor commandProcessor;

        public HttpServer(ManualLogSource logger, GameStateService gameStateService, CommandProcessor commandProcessor)
        {
            this.logger = logger;
            this.gameStateService = gameStateService;
            this.commandProcessor = commandProcessor;
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

            try
            {
                // Enable CORS
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string responseText = "";
                
                switch (request.Url.AbsolutePath.ToLower())
                {
                    case "/api/gamestate":
                        if (request.HttpMethod == "GET")
                        {
                            responseText = JsonConvert.SerializeObject(gameStateService.GetGameState(), Formatting.Indented);
                            response.ContentType = "application/json";
                        }
                        break;

                    case "/api/command":
                        if (request.HttpMethod == "POST")
                        {
                            using (var reader = new StreamReader(request.InputStream))
                            {
                                string json = reader.ReadToEnd();
                                var command = JsonConvert.DeserializeObject<DevCommand>(json);
                                
                                commandProcessor.EnqueueCommand(command);
                                
                                responseText = JsonConvert.SerializeObject(new { success = true });
                                response.ContentType = "application/json";
                            }
                        }
                        break;

                    case "/api/status":
                        try
                        {
                            responseText = JsonConvert.SerializeObject(new 
                            { 
                                connected = true, 
                                gameRunning = RoR2.Run.instance != null,
                                playerAlive = RoR2.LocalUserManager.GetFirstLocalUser()?.cachedBody != null,
                                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            response.ContentType = "application/json";
                        }
                        catch (Exception statusEx)
                        {
                            logger.LogWarning($"Error getting status: {statusEx.Message}");
                            responseText = JsonConvert.SerializeObject(new { connected = true, error = "Status unavailable" });
                            response.ContentType = "application/json";
                        }
                        break;

                    case "/api/itemcatalog":
                        if (request.HttpMethod == "GET")
                        {
                            var catalog = gameStateService.GetItemCatalog();
                            responseText = JsonConvert.SerializeObject(new 
                            { 
                                success = true,
                                items = catalog,
                                count = catalog.Count
                            }, Formatting.Indented);
                            response.ContentType = "application/json";
                        }
                        break;

                    case "/api/characterdefaults":
                        if (request.HttpMethod == "GET")
                        {
                            var defaults = gameStateService.GetCharacterDefaults();
                            responseText = JsonConvert.SerializeObject(new 
                            { 
                                success = true,
                                characters = defaults,
                                count = defaults.Count
                            }, Formatting.Indented);
                            response.ContentType = "application/json";
                        }
                        break;

                    default:
                        response.StatusCode = 404;
                        responseText = "Not Found";
                        break;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing request: {ex.Message}");
                response.StatusCode = 500;
            }
            finally
            {
                response.Close();
            }
        }
    }
}