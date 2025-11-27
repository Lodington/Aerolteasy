using System;
using BepInEx;
using RoR2;
using RoR2DevTool.Services;
using UnityEngine.Networking;

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
        private PermissionService permissionService;
        private NetworkingService networkingService;

        private bool wasServerActive = false;
        private bool wasClientActive = false;

        public void Awake()
        {
            Logger.LogInfo($"RoR2 Development Tool loaded!");
            
            // Initialize services
            gameStateService = new GameStateService(Logger);
            espOverlayService = new ESPOverlayService(Logger, gameStateService);
            commandProcessor = new CommandProcessor(Logger, gameStateService);
            permissionService = new PermissionService(Logger);
            networkingService = new NetworkingService(Logger, permissionService, commandProcessor);
            httpServer = new HttpServer(Logger, gameStateService, networkingService, permissionService);
            
            // Set ESP overlay service in command processor
            commandProcessor.SetESPOverlayService(espOverlayService);
            
            // Initialize ESP overlay service
            espOverlayService.Initialize();
            
            // Start HTTP server
            httpServer.Start();
            
            // Set initial host permissions for single player
            SetInitialHostPermissions();
        }

        private void SetInitialHostPermissions()
        {
            try
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.currentNetworkUser != null)
                {
                    string userId = localUser.currentNetworkUser.id.value.ToString();
                    permissionService.SetHost(userId);
                    Logger.LogInfo($"Initial host permissions set for user: {userId}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Could not set initial host permissions: {ex.Message}");
            }
        }

        private void EnsureHostPermissions()
        {
            try
            {
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.currentNetworkUser != null)
                {
                    string userId = localUser.currentNetworkUser.id.value.ToString();
                    var currentPermission = permissionService.GetPermission(userId);
                    
                    // If user has no permissions or is not marked as host, set them as host
                    if (currentPermission == Models.PermissionLevel.None || !permissionService.IsHost(userId))
                    {
                        permissionService.SetHost(userId);
                        Logger.LogInfo($"Ensured host permissions for user: {userId}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently fail - this is just a fallback check
            }
        }

        private void CheckNetworkState()
        {
            // Check if server just started
            if (NetworkServer.active && !wasServerActive)
            {
                Logger.LogInfo("Server started - initializing networking");
                networkingService.Initialize();
                
                // Set host permissions
                var localUser = LocalUserManager.GetFirstLocalUser();
                if (localUser?.currentNetworkUser != null)
                {
                    string hostId = localUser.currentNetworkUser.id.value.ToString();
                    permissionService.SetHost(hostId);
                }
                
                wasServerActive = true;
            }
            // Check if server just stopped
            else if (!NetworkServer.active && wasServerActive)
            {
                Logger.LogInfo("Server stopped - shutting down networking");
                networkingService.Shutdown();
                permissionService.ClearAllPermissions();
                wasServerActive = false;
            }

            // Check if client just started
            if (NetworkClient.active && !wasClientActive)
            {
                Logger.LogInfo("Client started - initializing networking");
                networkingService.Initialize();
                wasClientActive = true;
            }
            // Check if client just stopped
            else if (!NetworkClient.active && wasClientActive)
            {
                Logger.LogInfo("Client stopped - shutting down networking");
                networkingService.Shutdown();
                wasClientActive = false;
            }
        }

        private float lastGodModeUpdate = 0f;
        private float lastESPUpdate = 0f;
        private const float GOD_MODE_UPDATE_INTERVAL = 0.1f; // Update god mode every 100ms instead of every frame
        private const float ESP_UPDATE_INTERVAL = 0.05f; // Update ESP overlays every 50ms (20 FPS)

        private float lastPermissionCheck = 0f;
        private const float PERMISSION_CHECK_INTERVAL = 2f; // Check every 2 seconds

        public void Update()
        {
            try
            {
                // Check for network state changes
                CheckNetworkState();
                
                // Periodically ensure host has permissions (fallback)
                if (UnityEngine.Time.time - lastPermissionCheck > PERMISSION_CHECK_INTERVAL)
                {
                    EnsureHostPermissions();
                    lastPermissionCheck = UnityEngine.Time.time;
                }
                
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
            networkingService?.Shutdown();
            espOverlayService?.Cleanup();
            httpServer?.Stop();
        }
    }
}