using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugCharacterIconsCommand : BaseCommand
    {
        public override string CommandName => "debugcharactericons";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Debug character icons requested");
            LogAllCharacterIcons(logger);
        }

        private void LogAllCharacterIcons(ManualLogSource logger)
        {
            logger.LogInfo("=== Available Character Icons ===");
            
            int totalBodies = 0;
            int bodiesWithPortraits = 0;
            int survivorBodies = 0;
            
            for (BodyIndex i = 0; i < (BodyIndex)BodyCatalog.bodyCount; i++)
            {
                var bodyPrefab = BodyCatalog.GetBodyPrefab(i);
                if (bodyPrefab != null)
                {
                    totalBodies++;
                    var characterBody = bodyPrefab.GetComponent<CharacterBody>();
                    if (characterBody != null)
                    {
                        var hasPortrait = characterBody.portraitIcon != null;
                        var survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);
                        var isSurvivor = survivorDef != null;
                        
                        if (hasPortrait) bodiesWithPortraits++;
                        if (isSurvivor) survivorBodies++;
                        
                        // Only log survivor bodies or bodies with portraits
                        if (isSurvivor || hasPortrait)
                        {
                            logger.LogInfo($"Body: {bodyPrefab.name} | Portrait: {hasPortrait} | Survivor: {isSurvivor}");
                            
                            if (hasPortrait)
                            {
                                var texture = characterBody.portraitIcon;
                                var textureType = texture.GetType().Name;
                                logger.LogInfo($"  Portrait: {texture.width}x{texture.height} ({textureType})");
                                
                                // Test if it's a Texture2D
                                var texture2D = texture as Texture2D;
                                if (texture2D != null)
                                {
                                    logger.LogInfo($"  Can convert to Texture2D: Yes");
                                }
                                else
                                {
                                    logger.LogInfo($"  Can convert to Texture2D: No");
                                }
                            }
                            
                            if (isSurvivor && survivorDef != null)
                            {
                                logger.LogInfo($"  Survivor: {survivorDef.displayNameToken}");
                            }
                        }
                    }
                }
            }
            
            logger.LogInfo($"Summary: {totalBodies} total bodies, {bodiesWithPortraits} with portraits, {survivorBodies} survivors");
            logger.LogInfo("=== End Character Icons ===");
        }
    }
}