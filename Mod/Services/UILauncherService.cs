using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BepInEx.Logging;

namespace RoR2DevTool.Services
{
    public class UILauncherService
    {
        private readonly ManualLogSource logger;
        private const string UI_PROCESS_NAME = "ror2-devtool";
        private const string UI_EXE_NAME = "ror2-devtool.exe";
        private const string GITHUB_REPO = "Lodington/Aerolteasy"; // Update with your actual repo
        private const string MOD_VERSION = RoR2DevToolPlugin.PluginVersion;
        
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
                string versionFilePath = Path.Combine(modDirectory, "ui-version.txt");

                bool needsDownload = false;
                string reason = "";

                if (!File.Exists(exePath))
                {
                    needsDownload = true;
                    reason = "UI executable not found";
                }
                else if (!File.Exists(versionFilePath))
                {
                    needsDownload = true;
                    reason = "Version file not found";
                }
                else
                {
                    // Check if version matches
                    string installedVersion = File.ReadAllText(versionFilePath).Trim();
                    if (installedVersion != MOD_VERSION)
                    {
                        needsDownload = true;
                        reason = $"Version mismatch (installed: {installedVersion}, required: {MOD_VERSION})";
                    }
                }

                if (needsDownload)
                {
                    logger.LogInfo($"{reason}. Attempting to download UI version {MOD_VERSION} from GitHub...");
                    
                    // Try to download from GitHub releases
                    if (DownloadUIFromGitHub(exePath))
                    {
                        // Write version file
                        File.WriteAllText(versionFilePath, MOD_VERSION);
                        logger.LogInfo("Successfully downloaded UI from GitHub");
                    }
                    else
                    {
                        logger.LogError("Failed to download UI from GitHub");
                        logger.LogInfo($"Please manually download the UI from: https://github.com/{GITHUB_REPO}/releases/tag/v{MOD_VERSION}");
                        return;
                    }
                }

                LaunchUI(exePath);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error launching UI: {ex.Message}");
                logger.LogInfo("You can manually launch the UI from the mod folder");
            }
        }

        private bool DownloadUIFromGitHub(string destinationPath)
        {
            try
            {
                // Construct the GitHub release URL
                string downloadUrl = $"https://github.com/{GITHUB_REPO}/releases/download/v{MOD_VERSION}/{UI_EXE_NAME}";
                
                logger.LogInfo($"Downloading from: {downloadUrl}");

                using (var client = new WebClient())
                {
                    // Set user agent to avoid GitHub blocking
                    client.Headers.Add("User-Agent", $"RoR2DevTool/{MOD_VERSION}");
                    
                    // Download the file
                    client.DownloadFile(downloadUrl, destinationPath);
                }

                // Verify the file was downloaded
                if (File.Exists(destinationPath) && new FileInfo(destinationPath).Length > 0)
                {
                    logger.LogInfo($"Downloaded UI executable ({new FileInfo(destinationPath).Length / 1024 / 1024} MB)");
                    return true;
                }

                return false;
            }
            catch (WebException webEx)
            {
                logger.LogError($"Failed to download UI from GitHub: {webEx.Message}");
                if (webEx.Response is HttpWebResponse response)
                {
                    logger.LogError($"HTTP Status: {response.StatusCode}");
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        logger.LogError($"Release v{MOD_VERSION} not found on GitHub");
                        logger.LogInfo("Make sure the GitHub release exists and contains the UI executable");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error downloading UI: {ex.Message}");
                return false;
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
