using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class TeleportToPlayerCommand : BaseCommand
    {
        public override string CommandName => "teleporttoplayer";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var targetPlayerIdObj = command.Data?.GetValueOrDefault("targetPlayerId");
            
            if (targetPlayerIdObj == null)
            {
                logger.LogError("TeleportToPlayer command requires targetPlayerId");
                return;
            }

            int targetPlayerId;
            if (targetPlayerIdObj is long longValue)
            {
                targetPlayerId = (int)longValue;
            }
            else if (!int.TryParse(targetPlayerIdObj.ToString(), out targetPlayerId))
            {
                logger.LogError($"Invalid targetPlayerId: {targetPlayerIdObj}");
                return;
            }

            logger.LogInfo($"Teleporting player {playerId} to player {targetPlayerId}");

            try
            {
                // Get target player's position
                var targetUser = GetNetworkUserById(targetPlayerId);
                var targetBody = targetUser?.master?.GetBody();

                if (targetBody == null)
                {
                    logger.LogWarning($"Could not find target player {targetPlayerId}");
                    return;
                }

                var targetPosition = targetBody.transform.position;

                // Teleport player(s) to target
                if (playerId == -1)
                {
                    // Teleport all players except target
                    if (NetworkUser.readOnlyInstancesList != null)
                    {
                        foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                        {
                            if (networkUser?.master?.GetBody() != null)
                            {
                                var body = networkUser.master.GetBody();
                                // Don't teleport target to themselves
                                if (GetPlayerIdFromNetworkUser(networkUser) != targetPlayerId)
                                {
                                    TeleportPlayerToPosition(body, targetPosition, logger);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Teleport specific player
                    var networkUser = GetNetworkUserById(playerId);
                    var body = networkUser?.master?.GetBody();

                    if (body != null)
                    {
                        TeleportPlayerToPosition(body, targetPosition, logger);
                    }
                    else
                    {
                        logger.LogWarning($"Could not find body for player {playerId}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error teleporting player: {ex.Message}");
            }
        }

        private void TeleportPlayerToPosition(CharacterBody body, Vector3 targetPosition, ManualLogSource logger)
        {
            try
            {
                if (body?.transform != null)
                {
                    // Add small offset to prevent players from spawning inside each other
                    var offset = Random.insideUnitSphere * 3f; // Random position within 3 units
                    offset.y = 0; // Keep on same vertical level
                    var safePosition = targetPosition + offset + Vector3.up * 0.5f; // Slight vertical offset
                    
                    // Teleport the player
                    body.transform.position = safePosition;
                    
                    // Update character motor
                    var motor = body.characterMotor;
                    if (motor != null)
                    {
                        motor.Motor.SetPosition(safePosition);
                    }
                    
                    // Update rigidbody
                    var rigidbody = body.rigidbody;
                    if (rigidbody != null)
                    {
                        rigidbody.position = safePosition;
                        rigidbody.velocity = Vector3.zero;
                    }

                    // Create teleport effect
                    CreateTeleportEffect(safePosition);

                    logger.LogInfo($"Teleported {body.GetDisplayName()} to target location");
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error teleporting {body?.GetDisplayName()}: {ex.Message}");
            }
        }

        private void CreateTeleportEffect(Vector3 position)
        {
            try
            {
                var effectData = new EffectData
                {
                    origin = position,
                    rotation = Quaternion.identity
                };

                var teleportEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
                if (teleportEffectPrefab != null)
                {
                    EffectManager.SpawnEffect(teleportEffectPrefab, effectData, true);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Could not create teleport effect: {ex.Message}");
            }
        }

        private int GetPlayerIdFromNetworkUser(NetworkUser networkUser)
        {
            if (networkUser?.master?.playerCharacterMasterController != null)
            {
                return (int)networkUser.master.playerCharacterMasterController.netId.Value;
            }
            return -1;
        }
    }
}
