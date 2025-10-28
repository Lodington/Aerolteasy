using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using System.Linq;

namespace RoR2DevTool.Commands.MonsterCommands
{
    public class GiveMonsterBuffCommand : BaseCommand
    {
        public override string CommandName => "givemonsterbuff";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                string buffName = GetStringFromCommand(command, "buffName");
                int stacks = GetIntFromCommand(command, "stacks", 1);
                float duration = GetFloatFromCommand(command, "duration", -1f); // -1 means permanent

                if (string.IsNullOrEmpty(buffName))
                {
                    logger.LogWarning("Buff name is required for givemonsterbuff command");
                    return;
                }

                // Get the buff definition
                var buffDef = BuffCatalog.FindBuffIndex(buffName);
                if (buffDef == BuffIndex.None)
                {
                    logger.LogWarning($"Buff '{buffName}' not found");
                    return;
                }

                // Find all monster bodies (enemies)
                var monsterBodies = CharacterBody.readOnlyInstancesList
                    .Where(body => body != null && 
                           body.teamComponent != null &&
                           body.teamComponent.teamIndex == TeamIndex.Monster && 
                           body.healthComponent != null &&
                           body.healthComponent.alive)
                    .ToList();

                if (monsterBodies.Count == 0)
                {
                    logger.LogInfo("No alive monsters found to give buffs to");
                    return;
                }

                // Give the buff to all monsters
                int monstersAffected = 0;
                foreach (var body in monsterBodies)
                {
                    try
                    {
                        if (body.healthComponent != null && body.healthComponent.alive)
                        {
                            for (int i = 0; i < stacks; i++)
                            {
                                if (duration > 0)
                                {
                                    body.AddTimedBuff(buffDef, duration);
                                }
                                else
                                {
                                    body.AddBuff(buffDef);
                                }
                            }
                            monstersAffected++;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogWarning($"Failed to give buff to monster: {ex.Message}");
                    }
                }

                string durationText = duration > 0 ? $" for {duration}s" : " (permanent)";
                logger.LogInfo($"Gave {stacks}x {buffName}{durationText} to {monstersAffected} monster(s)");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error in GiveMonsterBuffCommand: {ex.Message}");
            }
        }
    }
}