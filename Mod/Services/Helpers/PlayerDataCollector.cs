using System;
using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Models;
using RoR2DevTool.Services.Interfaces;

namespace RoR2DevTool.Services.Helpers
{
    public class PlayerDataCollector : IPlayerDataCollector
    {
        private readonly ManualLogSource logger;
        private readonly Dictionary<int, bool> playerGodModes = new Dictionary<int, bool>();
        private readonly CharacterIconProvider iconProvider;

        public PlayerDataCollector(ManualLogSource logger)
        {
            this.logger = logger;
            this.iconProvider = new CharacterIconProvider();
        }

        public List<PlayerData> GetAllPlayers()
        {
            var players = new List<PlayerData>();

            try
            {
                if (NetworkUser.readOnlyInstancesList != null)
                {
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser?.gameObject != null)
                        {
                            try
                            {
                                var playerData = GetPlayerData(networkUser);
                                if (playerData != null)
                                {
                                    players.Add(playerData);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning($"Error processing player: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting all players: {ex.Message}");
            }

            return players;
        }

        private PlayerData GetPlayerData(NetworkUser networkUser)
        {
            var playerId = (int)networkUser.id.value;
            var playerData = new PlayerData
            {
                PlayerId = playerId,
                PlayerName = networkUser.userName ?? $"Player {playerId}",
                IsAlive = networkUser.master?.GetBody() != null,
                GodModeEnabled = playerGodModes.ContainsKey(playerId) && playerGodModes[playerId]
            };

            var body = networkUser.master?.GetBody();
            if (body?.gameObject != null)
            {
                PopulateBodyData(playerData, body, networkUser);
            }
            else
            {
                playerData.Items = new Dictionary<string, int>();
            }

            return playerData;
        }

        private void PopulateBodyData(PlayerData playerData, CharacterBody body, NetworkUser networkUser)
        {
            var characterName = body.name?.Replace("(Clone)", "") ?? "Unknown";
            playerData.CurrentCharacter = characterName;
            playerData.CharacterIcon = iconProvider.GetCharacterIcon(characterName);
            playerData.Health = body.healthComponent?.health ?? 0;
            playerData.MaxHealth = body.healthComponent?.fullHealth ?? 0;

            if (TeamManager.instance != null && networkUser.master != null)
            {
                playerData.Level = TeamManager.instance.GetTeamLevel(networkUser.master.teamIndex);
                playerData.Experience = TeamManager.instance.GetTeamCurrentLevelExperience(networkUser.master.teamIndex);
            }

            var inventory = body.inventory ?? networkUser.master?.inventory;
            playerData.Items = inventory != null ? GetPlayerItems(inventory) : new Dictionary<string, int>();

            PopulatePlayerStats(playerData, body);
        }

        private void PopulatePlayerStats(PlayerData playerData, CharacterBody body)
        {
            playerData.BaseDamage = body.baseDamage;
            playerData.Armor = body.armor;
            playerData.AttackSpeed = body.attackSpeed;
            playerData.CritChance = body.crit;
            playerData.MoveSpeed = body.moveSpeed;
            playerData.JumpPower = body.jumpPower;
            playerData.MaxShield = body.maxShield;
            playerData.HealthRegen = body.regen;
        }

        private Dictionary<string, int> GetPlayerItems(Inventory inventory)
        {
            var items = new Dictionary<string, int>();

            try
            {
                for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
                {
                    int count = inventory.GetItemCount(i);
                    if (count > 0)
                    {
                        var itemDef = ItemCatalog.GetItemDef(i);
                        if (itemDef != null)
                        {
                            items[itemDef.name] = count;
                        }
                    }
                }

                if (items.Count == 0 && inventory.itemAcquisitionOrder?.Count > 0)
                {
                    foreach (var itemIndex in inventory.itemAcquisitionOrder)
                    {
                        int count = inventory.GetItemCount(itemIndex);
                        var itemDef = ItemCatalog.GetItemDef(itemIndex);
                        if (itemDef != null)
                        {
                            items[itemDef.name] = count > 0 ? count : 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error getting player items: {ex.Message}");
            }

            return items;
        }

        public void SetPlayerGodMode(int playerId, bool enabled)
        {
            if (playerId == -1)
            {
                if (NetworkUser.readOnlyInstancesList != null)
                {
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        var id = (int)networkUser.id.value;
                        playerGodModes[id] = enabled;
                    }
                }
            }
            else
            {
                playerGodModes[playerId] = enabled;
            }
        }

        public void ApplyGodMode()
        {
            try
            {
                if (NetworkUser.readOnlyInstancesList == null) return;

                foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                {
                    if (networkUser?.master?.GetBody() != null)
                    {
                        var playerId = (int)networkUser.id.value;
                        if (playerGodModes.ContainsKey(playerId) && playerGodModes[playerId])
                        {
                            ApplyGodModeToPlayer(networkUser.master.GetBody());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Error applying god mode: {ex.Message}");
            }
        }

        private void ApplyGodModeToPlayer(CharacterBody body)
        {
            if (body?.healthComponent != null && body.gameObject != null)
            {
                var healthComp = body.healthComponent;
                if (healthComp.fullHealth > 0)
                {
                    healthComp.health = healthComp.fullHealth;
                }
                if (healthComp.fullShield > 0)
                {
                    healthComp.shield = healthComp.fullShield;
                }
            }
        }
    }
}
