using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class TeleportAllToCommand : BaseCommand
    {
        public override string CommandName => "teleportallto";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var targetPlayerIdObj = command.Data?.GetValueOrDefault("targetPlayerId");
            
            if (targetPlayerIdObj == null)
            {
                logger.LogError("TeleportAllTo command requires targetPlayerId");
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

            logger.LogInfo($"Teleporting all players to player {targetPlayerId}");

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
                int teleportedCount = 0;

                // Teleport all other players to target
                if (NetworkUser.readOnlyInstancesList != null)
                {
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser?.master?.GetBody() != null)
                        {
                            var body = networkUser.master.GetBody();
                            var playerId = GetPlayerIdFromNetworkUser(networkUser);
                            
                            // Don't teleport target to themselves
                            if (playerId != targetPlayerId)
                            {
                                TeleportPlayerToPosition(body, targetPosition, teleportedCount, logger);
                                teleportedCount++;
                            }
                        }
                    }
                }

                logger.LogInfo($"Teleported {teleportedCount} player(s) to {targetBody.GetDisplayName()}");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error teleporting players: {ex.Message}");
            }
        }

        private void TeleportPlayerToPosition(CharacterBody body, Vector3 targetPosition, int index, ManualLogSource logger)
        {
            try
            {
                if (body?.transform != null)
                {
                    // Arrange players in a circle around the target
                    float radius = 3f;
                    float angle = (360f / Mathf.Max(1, index + 1)) * index * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    
                    var safePosition = targetPosition + offset + Vector3.up * 0.5f;
                    
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
