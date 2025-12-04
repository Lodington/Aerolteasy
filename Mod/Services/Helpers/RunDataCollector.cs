using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;
using RoR2DevTool.Services.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2DevTool.Services.Helpers
{
    public class RunDataCollector : IRunDataCollector
    {
        private readonly ManualLogSource logger;
        private readonly bool verboseLogging;

        public RunDataCollector(ManualLogSource logger, bool verboseLogging = false)
        {
            this.logger = logger;
            this.verboseLogging = verboseLogging;
        }

        public bool IsInRun()
        {
            try
            {
                if (Run.instance == null) return false;

                bool hasActivePlayers = HasActivePlayers();
                bool isInGameScene = IsInGameScene();
                bool inRun = Run.instance != null && hasActivePlayers && isInGameScene;

                if (verboseLogging)
                {
                    var currentScene = SceneManager.GetActiveScene();
                    logger.LogInfo($"Run detection - Run.instance: {Run.instance != null}, HasActivePlayers: {hasActivePlayers}, InGameScene: {isInGameScene} ({currentScene.name}), Result: {inRun}");
                }

                return inRun;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error detecting run state: {ex.Message}");
                return Run.instance != null;
            }
        }

        public RunData GetRunData()
        {
            var runData = new RunData();

            try
            {
                if (Run.instance != null)
                {
                    runData.CurrentStage = GetCurrentStage();
                    runData.StageNumber = Run.instance.stageClearCount + 1;
                    runData.DifficultyCoefficient = Run.instance.difficultyCoefficient;
                    runData.GameTime = Run.instance.GetRunStopwatch();
                    runData.IsPaused = Time.timeScale == 0f;
                    runData.TimeScale = Time.timeScale;
                    runData.Seed = Run.instance.seed.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error getting run data: {ex.Message}");
            }

            return runData;
        }

        private string GetCurrentStage()
        {
            try
            {
                return SceneCatalog.GetSceneDefForCurrentScene()?.baseSceneName ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private bool HasActivePlayers()
        {
            if (NetworkUser.readOnlyInstancesList == null) return false;

            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
            {
                if (networkUser?.master?.GetBody() != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsInGameScene()
        {
            try
            {
                var currentScene = SceneManager.GetActiveScene();
                if (currentScene == null) return false;

                var sceneName = currentScene.name.ToLower();
                return !sceneName.Contains("menu") &&
                       !sceneName.Contains("lobby") &&
                       !sceneName.Contains("title");
            }
            catch
            {
                return false;
            }
        }
    }
}
