using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugESPDataCommand : BaseCommand
    {
        public override string CommandName => "debugespdata";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Collecting ESP data for debugging...");

            try
            {
                var monsters = new List<object>();
                var interactables = new List<object>();
                
                // Get player position for distance calculations
                var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                var playerPosition = playerBody?.transform?.position ?? Vector3.zero;

                // Collect monster data
                if (CharacterMaster.readOnlyInstancesList != null)
                {
                    foreach (var master in CharacterMaster.readOnlyInstancesList)
                    {
                        if (master != null && master.teamIndex == TeamIndex.Monster && master.hasBody)
                        {
                            var body = master.GetBody();
                            if (body != null && body.gameObject != null)
                            {
                                var position = body.transform.position;
                                var distance = Vector3.Distance(playerPosition, position);
                                
                                var monsterData = new
                                {
                                    Name = body.name?.Replace("(Clone)", "") ?? "Unknown",
                                    DisplayName = body.GetDisplayName() ?? "Unknown Monster",
                                    Health = body.healthComponent?.health ?? 0f,
                                    MaxHealth = body.healthComponent?.fullHealth ?? 0f,
                                    IsAlive = body.healthComponent?.alive ?? false,
                                    IsElite = body.isElite,
                                    EliteType = GetEliteTypeName(body),
                                    PositionX = position.x,
                                    PositionY = position.y,
                                    PositionZ = position.z,
                                    Distance = distance,
                                };
                                
                                monsters.Add(monsterData);
                                logger.LogInfo($"Found monster: {monsterData.DisplayName} at distance {distance:F1}m");
                            }
                        }
                    }
                }

                // Collect interactable data
                if (InstanceTracker.GetInstancesList<PurchaseInteraction>() != null)
                {
                    foreach (var interaction in InstanceTracker.GetInstancesList<PurchaseInteraction>())
                    {
                        if (interaction != null && interaction.gameObject != null)
                        {
                            var position = interaction.transform.position;
                            var distance = Vector3.Distance(playerPosition, position);
                            
                            var interactableData = new
                            {
                                Name = interaction.name?.Replace("(Clone)", "") ?? "Unknown",
                                DisplayName = interaction.GetDisplayName() ?? "Unknown Interactable",
                                IsAvailable = interaction.available,
                                IsActivated = !interaction.available,
                                PositionX = position.x,
                                PositionY = position.y,
                                PositionZ = position.z,
                                Distance = distance,
                                Category = GetInteractableCategory(interaction)
                            };
                            
                            interactables.Add(interactableData);
                            logger.LogInfo($"Found interactable: {interactableData.DisplayName} ({interactableData.Category}) at distance {distance:F1}m");
                        }
                    }
                }

                logger.LogInfo($"ESP Data Collection Complete - Monsters: {monsters.Count}, Interactables: {interactables.Count}");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error collecting ESP data: {ex.Message}");
                logger.LogError(ex.StackTrace);
            }
        }

        private string GetEliteTypeName(CharacterBody body)
        {
            try
            {
                if (body?.inventory?.currentEquipmentIndex != null)
                {
                    var equipmentDef = EquipmentCatalog.GetEquipmentDef(body.inventory.currentEquipmentIndex);
                    if (equipmentDef != null)
                    {
                        return equipmentDef.name switch
                        {
                            "AffixRed" => "Fire",
                            "AffixBlue" => "Ice", 
                            "AffixYellow" => "Lightning",
                            "AffixPoison" => "Poison",
                            "AffixHaunted" => "Celestine",
                            "AffixWhite" => "Celestine",
                            "AffixSealed" => "Malachite",
                            "AffixLunar" => "Perfected",
                            _ => "Elite"
                        };
                    }
                }
                return "Elite";
            }
            catch
            {
                return "Elite";
            }
        }

        private string GetInteractableCategory(PurchaseInteraction interaction)
        {
            try
            {
                var name = interaction.name?.ToLower() ?? "";
                
                if (name.Contains("chest") || name.Contains("crate"))
                    return "Chest";
                if (name.Contains("shrine"))
                    return "Shrine";
                if (name.Contains("barrel") || name.Contains("pot"))
                    return "Barrel";
                if (name.Contains("drone") || name.Contains("turret"))
                    return "Drone";
                if (name.Contains("shop") || name.Contains("vendor"))
                    return "Shop";
                if (name.Contains("printer") || name.Contains("duplicator"))
                    return "Printer";
                if (name.Contains("portal") || name.Contains("teleporter"))
                    return "Portal";
                if (name.Contains("scrap"))
                    return "Scrap";
                
                return "Other";
            }
            catch
            {
                return "Other";
            }
        }
    }
}