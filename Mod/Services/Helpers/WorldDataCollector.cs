using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;
using RoR2DevTool.Services.Interfaces;
using UnityEngine;

namespace RoR2DevTool.Services.Helpers
{
    public class WorldDataCollector : IWorldDataCollector
    {
        private readonly ManualLogSource logger;
        private readonly bool verboseLogging;

        public WorldDataCollector(ManualLogSource logger, bool verboseLogging = false)
        {
            this.logger = logger;
            this.verboseLogging = verboseLogging;
        }

        public List<MonsterData> GetMonsters()
        {
            var monsters = new List<MonsterData>();

            try
            {
                if (CharacterMaster.readOnlyInstancesList == null) return monsters;

                var playerPosition = GetPlayerPosition();

                foreach (var master in CharacterMaster.readOnlyInstancesList)
                {
                    if (master?.teamIndex == TeamIndex.Monster && master.hasBody)
                    {
                        var body = master.GetBody();
                        if (body?.gameObject != null)
                        {
                            var monsterData = CreateMonsterData(body, playerPosition);
                            monsters.Add(monsterData);
                        }
                    }
                }

                if (verboseLogging)
                {
                    logger.LogInfo($"Collected {monsters.Count} monsters");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error collecting monster data: {ex.Message}");
            }

            return monsters;
        }

        public List<InteractableData> GetInteractables()
        {
            var interactables = new List<InteractableData>();

            try
            {
                var playerPosition = GetPlayerPosition();
                var interactions = InstanceTracker.GetInstancesList<PurchaseInteraction>();

                if (interactions != null)
                {
                    foreach (var interaction in interactions)
                    {
                        if (interaction?.gameObject != null)
                        {
                            var interactableData = CreateInteractableData(interaction, playerPosition);
                            interactables.Add(interactableData);
                        }
                    }
                }

                if (verboseLogging)
                {
                    logger.LogInfo($"Collected {interactables.Count} interactables");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error collecting interactable data: {ex.Message}");
            }

            return interactables;
        }

        public List<ItemData> GetWorldItems()
        {
            var items = new List<ItemData>();

            try
            {
                var playerPosition = GetPlayerPosition();
                var pickups = InstanceTracker.GetInstancesList<GenericPickupController>();

                if (pickups != null)
                {
                    foreach (var pickup in pickups)
                    {
                        if (pickup?.gameObject != null && pickup.pickupIndex != PickupIndex.none)
                        {
                            var itemData = CreateItemData(pickup, playerPosition);
                            if (itemData != null)
                            {
                                items.Add(itemData);
                            }
                        }
                    }
                }

                if (verboseLogging)
                {
                    logger.LogInfo($"Collected {items.Count} items");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error collecting item data: {ex.Message}");
            }

            return items;
        }

        public TeleporterData GetTeleporterData()
        {
            var teleporterData = new TeleporterData();

            try
            {
                var teleporter = TeleporterInteraction.instance;
                if (teleporter?.gameObject != null)
                {
                    teleporterData.IsActive = teleporter.isActiveAndEnabled;
                    teleporterData.IsCharged = teleporter.isCharged;
                    teleporterData.ChargeProgress = teleporter.chargeFraction;
                    teleporterData.BossEventActive = teleporter.bossDirector?.enabled ?? false;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error getting teleporter info: {ex.Message}");
            }

            return teleporterData;
        }

        private Vector3 GetPlayerPosition()
        {
            var playerBody = LocalUserManager.GetFirstLocalUser()?.cachedBody;
            return playerBody?.transform?.position ?? Vector3.zero;
        }

        private MonsterData CreateMonsterData(CharacterBody body, Vector3 playerPosition)
        {
            var position = body.transform.position;
            var distance = Vector3.Distance(playerPosition, position);

            return new MonsterData
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
                Distance = distance
            };
        }

        private InteractableData CreateInteractableData(PurchaseInteraction interaction, Vector3 playerPosition)
        {
            var position = interaction.transform.position;
            var distance = Vector3.Distance(playerPosition, position);

            return new InteractableData
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
        }

        private ItemData CreateItemData(GenericPickupController pickup, Vector3 playerPosition)
        {
            var position = pickup.transform.position;
            var distance = Vector3.Distance(playerPosition, position);

            var pickupDef = PickupCatalog.GetPickupDef(pickup.pickupIndex);
            if (pickupDef?.itemIndex == ItemIndex.None) return null;

            var itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
            if (itemDef == null) return null;

            return new ItemData
            {
                Name = itemDef.name ?? "Unknown",
                DisplayName = itemDef.nameToken != null ? Language.GetString(itemDef.nameToken) : itemDef.name ?? "Unknown Item",
                ItemTier = GetItemTierName(itemDef.tier),
                PositionX = position.x,
                PositionY = position.y,
                PositionZ = position.z,
                Distance = distance,
                NetworkId = pickup.gameObject.GetInstanceID(),
                IsPickupable = pickup.gameObject.activeInHierarchy
            };
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

                if (name.Contains("chest") || name.Contains("crate")) return "Chest";
                if (name.Contains("shrine")) return "Shrine";
                if (name.Contains("barrel") || name.Contains("pot")) return "Barrel";
                if (name.Contains("drone") || name.Contains("turret")) return "Drone";
                if (name.Contains("shop") || name.Contains("vendor")) return "Shop";
                if (name.Contains("printer") || name.Contains("duplicator")) return "Printer";
                if (name.Contains("portal") || name.Contains("teleporter")) return "Portal";
                if (name.Contains("scrap")) return "Scrap";

                return "Other";
            }
            catch
            {
                return "Other";
            }
        }

        private string GetItemTierName(ItemTier tier)
        {
            return tier switch
            {
                ItemTier.Tier1 => "White",
                ItemTier.Tier2 => "Green",
                ItemTier.Tier3 => "Red",
                ItemTier.Lunar => "Lunar",
                ItemTier.Boss => "Boss",
                ItemTier.VoidTier1 => "Void White",
                ItemTier.VoidTier2 => "Void Green",
                ItemTier.VoidTier3 => "Void Red",
                ItemTier.VoidBoss => "Void Boss",
                _ => "Unknown"
            };
        }
    }
}
