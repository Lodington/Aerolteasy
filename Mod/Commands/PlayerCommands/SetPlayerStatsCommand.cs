using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class SetPlayerStatsCommand : BaseCommand
    {
        public override string CommandName => "setplayerstats";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var statType = command.Data?.GetValueOrDefault("statType")?.ToString();
            var value = command.Data?.GetValueOrDefault("value")?.ToString();

            if (string.IsNullOrEmpty(statType) || string.IsNullOrEmpty(value))
            {
                logger.LogError("SetPlayerStats command requires statType and value");
                return;
            }

            if (!float.TryParse(value, out float statValue))
            {
                logger.LogError($"Invalid stat value: {value}");
                return;
            }

            logger.LogInfo($"Setting {statType} to {statValue} for player {playerId}");

            try
            {
                NetworkUser networkUser = null;
                CharacterMaster master = null;
                CharacterBody body = null;

                if (playerId == -1)
                {
                    // Apply to all players
                    if (NetworkUser.readOnlyInstancesList != null)
                    {
                        foreach (var user in NetworkUser.readOnlyInstancesList)
                        {
                            if (user?.master?.GetBody() != null)
                            {
                                SetPlayerStat(user.master.GetBody(), statType, statValue, logger);
                            }
                        }
                    }
                }
                else
                {
                    networkUser = GetNetworkUserById(playerId);
                    master = networkUser?.master;
                    body = master?.GetBody();

                    if (body != null)
                    {
                        SetPlayerStat(body, statType, statValue, logger);
                    }
                    else
                    {
                        logger.LogWarning($"Could not find body for player {playerId}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error setting player stat: {ex.Message}");
            }
        }

        private void SetPlayerStat(CharacterBody body, string statType, float value, ManualLogSource logger)
        {
            try
            {
                switch (statType.ToLower())
                {
                    case "experience":
                        if (body.master?.teamIndex != null)
                        {
                            TeamManager.instance?.GiveTeamExperience(body.master.teamIndex, (ulong)value);
                        }
                        break;

                    case "damage":
                        if (body.baseDamage > 0)
                        {
                            body.baseDamage = value;
                            body.MarkAllStatsDirty();
                        }
                        break;

                    case "armor":
                        body.baseArmor = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "attackspeed":
                        body.baseAttackSpeed = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "critchance":
                        body.baseCrit = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "movespeed":
                        body.baseMoveSpeed = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "jumppower":
                        body.baseJumpPower = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "maxhealth":
                        body.baseMaxHealth = value;
                        body.MarkAllStatsDirty();
                        // Also update current health if it exceeds new max
                        if (body.healthComponent != null)
                        {
                            if (body.healthComponent.health > body.maxHealth)
                            {
                                body.healthComponent.health = body.maxHealth;
                            }
                        }
                        break;

                    case "maxshield":
                        body.baseMaxShield = value;
                        body.MarkAllStatsDirty();
                        break;

                    case "regen":
                        body.baseRegen = value;
                        body.MarkAllStatsDirty();
                        break;

                    default:
                        logger.LogWarning($"Unknown stat type: {statType}");
                        break;
                }

                logger.LogInfo($"Set {statType} to {value} for {body.GetDisplayName()}");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error setting {statType} for {body?.GetDisplayName()}: {ex.Message}");
            }
        }
    }
}