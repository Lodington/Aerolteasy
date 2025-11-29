using System;
using System.Diagnostics;
using System.IO;
using BepInEx.Logging;

namespace RoR2DevTool.Services
{
    public class UILauncherService
    {
        private readonly ManualLogSource logger;
        private const string UI_PROCESS_NAME = "ror2-devtool";
        private const string UI_EXE_NAME = "ror2-devtool.exe";
        
        public UILauncherService(ManualLogSource logger)
        {
            this.logger = logger;
        }

        public void LaunchUIIfNotRunning()
        {
            try
            {
                // Check if the UI is already running
                if (IsUIRunning())
                {
                    logger.LogInfo("RoR2 DevTool UI is already running");
                    return;
                }

                // Get the UI executable path (same directory as the mod DLL)
                string modDirectory = Path.GetDirectoryName(typeof(UILauncherService).Assembly.Location);
                string exePath = Path.Combine(modDirectory, UI_EXE_NAME);

                if (!File.Exists(exePath))
                {
                    logger.LogWarning($"RoR2 DevTool UI executable not found at: {exePath}");
                    logger.LogInfo("Please place ror2-devtool-ui.exe in the same folder as RoR2DevTool.dll");
                    return;
                }

                LaunchUI(exePath);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error launching UI: {ex.Message}");
                logger.LogInfo("You can manually launch the UI from the mod folder");
            }
        }

        private bool IsUIRunning()
        {
            try
            {
                // Check if any process with the UI name is running
                var processes = Process.GetProcessesByName(UI_PROCESS_NAME);
                if (processes.Length > 0)
                {
                    logger.LogInfo($"Found {processes.Length} instance(s) of UI already running");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error checking if UI is running: {ex.Message}");
                return false; // Assume not running if we can't check
            }
        }

        private void LaunchUI(string exePath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true, // Launch as independent process
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                };

                Process.Start(startInfo);
                logger.LogInfo($"Successfully launched RoR2 DevTool UI");
                logger.LogInfo("The UI will open in a separate window");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to launch UI: {ex.Message}");
                logger.LogInfo($"You can manually launch the UI by running: {exePath}");
            }
        }

        public void CheckUIConnection()
        {
            try
            {
                // Simple check to see if the HTTP server can be reached
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadString("http://localhost:5173/api/status");
                    logger.LogInfo("UI is connected and responding");
                }
            }
            catch
            {
                // Silently fail - UI might not be ready yet
            }
        }
    }
}
