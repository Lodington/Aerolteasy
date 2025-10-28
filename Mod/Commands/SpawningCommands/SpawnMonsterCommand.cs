using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine;

namespace RoR2DevTool.Commands.SpawningCommands
{
    public class SpawnMonsterCommand : BaseCommand
    {
        public override string CommandName => "spawnmonster";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var monsterName = GetStringFromCommand(command, "monsterName");
            var count = GetIntFromCommand(command, "count", 1);
            var distance = GetFloatFromCommand(command, "distance", 10f);
            var teamName = GetStringFromCommand(command, "team") ?? "Monster";
            
            if (string.IsNullOrEmpty(monsterName))
            {
                logger.LogWarning("Monster name is required for spawnmonster command");
                return;
            }

            // Parse team index
            var teamIndex = ParseTeamIndex(teamName);
            
            logger.LogInfo($"Spawning {count}x {monsterName} on team {teamName} at distance {distance}");
            SpawnMonster(monsterName, count, distance, teamIndex, logger);
        }

        private void SpawnMonster(string monsterName, int count, float distance, TeamIndex teamIndex, ManualLogSource logger)
        {
            // Map common names to actual master prefab names
            string masterName = GetMasterPrefabName(monsterName);
            
            // Find the master prefab
            var masterPrefab = MasterCatalog.FindMasterPrefab(masterName);
            if (masterPrefab == null)
            {
                // Try with "Master" suffix
                masterPrefab = MasterCatalog.FindMasterPrefab(masterName + "Master");
                if (masterPrefab == null)
                {
                    // Try with "Body" replaced with "Master"
                    var bodyToMaster = masterName.Replace("Body", "Master");
                    masterPrefab = MasterCatalog.FindMasterPrefab(bodyToMaster);
                    if (masterPrefab == null)
                    {
                        logger.LogWarning($"Monster master prefab not found for: {monsterName} (tried: {masterName}, {masterName}Master, {bodyToMaster})");
                        return;
                    }
                }
            }

            // Get spawn position
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser?.cachedBody == null)
            {
                logger.LogWarning("Local player not found for spawn position");
                return;
            }

            var playerPosition = localUser.cachedBody.transform.position;
            
            for (int i = 0; i < count; i++)
            {
                // Calculate spawn position with some randomization
                var angle = (360f / count) * i * Mathf.Deg2Rad;
                var randomOffset = Random.Range(-2f, 2f);
                var spawnPosition = playerPosition + new Vector3(
                    Mathf.Cos(angle) * (distance + randomOffset),
                    2f, // Spawn slightly above ground
                    Mathf.Sin(angle) * (distance + randomOffset)
                );

                // Spawn the monster
                var spawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
                spawnCard.prefab = masterPrefab;
                spawnCard.sendOverNetwork = true;
                spawnCard.hullSize = HullClassification.Human; // Default hull size
                spawnCard.directorCreditCost = 0;
                spawnCard.occupyPosition = false;

                var spawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    position = spawnPosition,
                    minDistance = 0f,
                    maxDistance = 0f
                }, RoR2Application.rng);

                spawnRequest.teamIndexOverride = teamIndex;
                
                DirectorCore.instance?.TrySpawnObject(spawnRequest);
                
                logger.LogInfo($"Spawned {monsterName} at position {spawnPosition}");
            }
        }

        private string GetMasterPrefabName(string monsterName)
        {
            // Map UI names to actual RoR2 master prefab names
            return monsterName.ToLower() switch
            {
                "beetle" => "BeetleMaster",
                "beetleguard" => "BeetleGuardMaster", 
                "wisp" => "WispMaster",
                "greaterwisp" => "GreaterWispMaster",
                "lemurian" => "LemurianMaster",
                "lemurianbruiser" => "LemurianBruiserMaster",
                "golem" => "GolemMaster",
                "hermitcrab" => "HermitCrabMaster",
                "jellyfish" => "JellyfishMaster",
                "bison" => "BisonMaster",
                "parent" => "ParentMaster",
                "claytemplar" => "ClayTemplarMaster",
                "vermin" => "VerminMaster",
                "vulture" => "VultureMaster",
                "roboballboss" => "RoboBallBossMaster",
                "superroboballboss" => "SuperRoboBallBossMaster",
                "clayboss" => "ClayBossMaster",
                "titan" => "TitanMaster",
                "vagrant" => "VagrantMaster",
                "magmaworm" => "MagmaWormMaster",
                "impboss" => "ImpBossMaster",
                "gravekeeperboss" => "GravekeeperBossMaster",
                "scav" => "ScavMaster",
                "bell" => "BellMaster",
                "minimushroom" => "MiniMushroomMaster",
                "acidlarva" => "AcidLarvaMaster",
                "assassin" => "AssassinMaster",
                "claybruiser" => "ClayBruiserMaster",
                "nullifier" => "NullifierMaster",
                "lunargolem" => "LunarGolemMaster",
                "lunarwisp" => "LunarWispMaster",
                "lunarexploder" => "LunarExploderMaster",
                _ => monsterName + "Master" // Default fallback
            };
        }

        private TeamIndex ParseTeamIndex(string teamName)
        {
            return teamName.ToLower() switch
            {
                "player" => TeamIndex.Player,
                "monster" => TeamIndex.Monster,
                "neutral" => TeamIndex.Neutral,
                "void" => TeamIndex.Void,
                "lunar" => TeamIndex.Lunar,
                _ => TeamIndex.Monster // Default to monster team
            };
        }
    }
}