using BepInEx;
using RoR2DevTool.Services;

namespace RoR2DevTool
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class RoR2DevToolPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.devtool.ror2";
        public const string PluginName = "RoR2 Development Tool";
        public const string PluginVersion = "1.0.0";

        private HttpServer httpServer;
        private GameStateService gameStateService;
        private CommandProcessor commandProcessor;
        private ESPOverlayService espOverlayService;

        public void Awake()
        {
            Logger.LogInfo($"RoR2 Development Tool loaded!");
            
            // Initialize services
            gameStateService = new GameStateService(Logger);
            espOverlayService = new ESPOverlayService(Logger, gameStateService);
            commandProcessor = new CommandProcessor(Logger, gameStateService);
            httpServer = new HttpServer(Logger, gameStateService, commandProcessor);
            
            // Set ESP overlay service in command processor
            commandProcessor.SetESPOverlayService(espOverlayService);
            
            // Initialize ESP overlay service
            espOverlayService.Initialize();
            
            // Start HTTP server
            httpServer.Start();
        }

        private float lastGodModeUpdate = 0f;
        private float lastESPUpdate = 0f;
        private const float GOD_MODE_UPDATE_INTERVAL = 0.1f; // Update god mode every 100ms instead of every frame
        private const float ESP_UPDATE_INTERVAL = 0.05f; // Update ESP overlays every 50ms (20 FPS)

        public void Update()
        {
            try
            {
                commandProcessor?.ProcessCommands();
                
                // Only apply god mode periodically to reduce performance impact
                if (UnityEngine.Time.time - lastGodModeUpdate > GOD_MODE_UPDATE_INTERVAL)
                {
                    gameStateService?.ApplyGodMode();
                    lastGodModeUpdate = UnityEngine.Time.time;
                }
                
                // Update ESP overlays periodically
                if (UnityEngine.Time.time - lastESPUpdate > ESP_UPDATE_INTERVAL)
                {
                    espOverlayService?.UpdateOverlays();
                    lastESPUpdate = UnityEngine.Time.time;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Error in Update loop: {ex.Message}");
            }
        }

        public void OnDestroy()
        {
            espOverlayService?.Cleanup();
            httpServer?.Stop();
        }
    }
}