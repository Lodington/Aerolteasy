using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.GameCommands
{
    public class ChangeStageCommand : BaseCommand
    {
        public override string CommandName => "changestage";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var stageName = GetStringFromCommand(command, "stageName");
            
            if (string.IsNullOrEmpty(stageName))
            {
                logger.LogWarning("Stage name is required for changestage command");
                return;
            }

            logger.LogInfo($"Attempting to change stage to: {stageName}");
            ChangeStage(stageName, logger);
        }

        private void ChangeStage(string stageName, ManualLogSource logger)
        {
            var sceneIndex = SceneCatalog.FindSceneIndex(stageName);
            if (sceneIndex == SceneIndex.Invalid) 
            {
                logger.LogWarning($"Invalid scene name: {stageName}");
                return;
            }
            
            var sceneDef = SceneCatalog.GetSceneDef(sceneIndex);
            if (sceneDef == null) 
            {
                logger.LogWarning($"Scene definition not found for: {stageName}");
                return;
            }
            
            logger.LogInfo($"Found scene: {sceneDef.baseSceneName}");
            
            // Try to immediately change to the stage
            if (Run.instance != null)
            {
                // Set the next stage
                var weightedSelection = new WeightedSelection<SceneDef>();
                weightedSelection.AddChoice(sceneDef, 1f);
                Run.instance.PickNextStageScene(weightedSelection);
                
                // Force advance to next stage immediately
                try
                {
                    // This will trigger the stage transition
                    Run.instance.AdvanceStage(sceneDef);
                    logger.LogInfo($"Successfully initiated stage change to: {sceneDef.baseSceneName}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to advance stage directly: {ex.Message}");
                    logger.LogInfo("Stage will change on next teleporter activation");
                }
            }
            else
            {
                logger.LogWarning("No active run found - cannot change stage");
            }
        }
    }
}