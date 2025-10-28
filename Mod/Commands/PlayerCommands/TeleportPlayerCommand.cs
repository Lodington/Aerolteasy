using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class TeleportPlayerCommand : BaseCommand
    {
        public override string CommandName => "teleportplayer";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var x = command.Data?.GetValueOrDefault("x")?.ToString();
            var y = command.Data?.GetValueOrDefault("y")?.ToString();
            var z = command.Data?.GetValueOrDefault("z")?.ToString();
            var offsetStr = command.Data?.GetValueOrDefault("yOffset")?.ToString();

            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y) || string.IsNullOrEmpty(z))
            {
                logger.LogError("Teleport command requires x, y, and z coordinates");
                return;
            }

            if (!float.TryParse(x, out float posX) || 
                !float.TryParse(y, out float posY) || 
                !float.TryParse(z, out float posZ))
            {
                logger.LogError($"Invalid coordinates: x={x}, y={y}, z={z}");
                return;
            }

            // Parse optional Y offset (default to 2.5 units above target)
            float yOffset = 2.5f;
            if (!string.IsNullOrEmpty(offsetStr) && float.TryParse(offsetStr, out float parsedOffset))
            {
                yOffset = parsedOffset;
            }

            var targetPosition = new Vector3(posX, posY, posZ);
            logger.LogInfo($"Teleporting player {playerId} to position ({posX}, {posY}, {posZ})");

            try
            {
                if (playerId == -1)
                {
                    // Teleport all players
                    if (NetworkUser.readOnlyInstancesList != null)
                    {
                        foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                        {
                            if (networkUser?.master?.GetBody() != null)
                            {
                                TeleportPlayer(networkUser.master.GetBody(), targetPosition, yOffset, logger);
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
                        TeleportPlayer(body, targetPosition, yOffset, logger);
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

        private void TeleportPlayer(CharacterBody body, Vector3 position, float yOffset, ManualLogSource logger)
        {
            try
            {
                if (body?.transform != null)
                {
                    // Store the original position for logging
                    var originalPos = body.transform.position;
                    
                    // Add vertical offset to prevent getting stuck inside objects
                    // Use configurable offset (default 2.5 units above target)
                    var safePosition = position + Vector3.up * yOffset;
                    
                    // Teleport the player
                    body.transform.position = safePosition;
                    
                    // Also teleport the character motor if it exists (for proper physics)
                    var motor = body.characterMotor;
                    if (motor != null)
                    {
                        motor.Motor.SetPosition(safePosition);
                    }
                    
                    // Teleport the rigidbody if it exists
                    var rigidbody = body.rigidbody;
                    if (rigidbody != null)
                    {
                        rigidbody.position = safePosition;
                        rigidbody.velocity = Vector3.zero; // Stop any momentum
                    }

                    // Create a teleport effect at the destination (at player position, not target)
                    CreateTeleportEffect(safePosition);

                    logger.LogInfo($"Teleported {body.GetDisplayName()} from ({originalPos.x:F1}, {originalPos.y:F1}, {originalPos.z:F1}) to ({safePosition.x:F1}, {safePosition.y:F1}, {safePosition.z:F1}) [+{yOffset}m above target]");
                }
                else
                {
                    logger.LogWarning($"Could not teleport {body?.GetDisplayName()}: missing transform");
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
                // Try to create a teleport effect at the destination
                // This uses the same effect as the Quantum Translocator
                var effectData = new EffectData
                {
                    origin = position,
                    rotation = Quaternion.identity
                };

                // Try to find a suitable teleport effect
                var teleportEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
                if (teleportEffectPrefab != null)
                {
                    EffectManager.SpawnEffect(teleportEffectPrefab, effectData, true);
                }
            }
            catch (System.Exception ex)
            {
                // Effect creation is optional, don't fail the teleport if it doesn't work
                // Just log it for debugging
                UnityEngine.Debug.LogWarning($"Could not create teleport effect: {ex.Message}");
            }
        }
    }
}