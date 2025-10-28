using System.Collections.Generic;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.DebugCommands
{
    public class DebugCharacterDefaultsCommand : BaseCommand
    {
        public override string CommandName => "debugcharacterdefaults";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            logger.LogInfo("Collecting character default stats...");

            try
            {
                var characterDefaults = new List<object>();

                // Get all available character bodies from the catalog
                foreach (var bodyPrefab in BodyCatalog.allBodyPrefabs)
                {
                    if (bodyPrefab != null)
                    {
                        var characterBody = bodyPrefab.GetComponent<CharacterBody>();
                        if (characterBody != null && !string.IsNullOrEmpty(characterBody.name))
                        {
                            // Skip non-playable characters (monsters, etc.)
                            if (IsPlayableCharacter(characterBody.name))
                            {
                                var defaultStats = new
                                {
                                    CharacterName = characterBody.name,
                                    DisplayName = GetCharacterDisplayName(characterBody.name),
                                    BaseDamage = characterBody.baseDamage,
                                    BaseArmor = characterBody.baseArmor,
                                    BaseAttackSpeed = characterBody.baseAttackSpeed,
                                    BaseCrit = characterBody.baseCrit,
                                    BaseMoveSpeed = characterBody.baseMoveSpeed,
                                    BaseJumpPower = characterBody.baseJumpPower,
                                    BaseMaxHealth = characterBody.baseMaxHealth,
                                    BaseMaxShield = characterBody.baseMaxShield,
                                    BaseRegen = characterBody.baseRegen,
                                    LevelDamage = characterBody.levelDamage,
                                    LevelArmor = characterBody.levelArmor,
                                    LevelAttackSpeed = characterBody.levelAttackSpeed,
                                    LevelCrit = characterBody.levelCrit,
                                    LevelMoveSpeed = characterBody.levelMoveSpeed,
                                    LevelJumpPower = characterBody.levelJumpPower,
                                    LevelMaxHealth = characterBody.levelMaxHealth,
                                    LevelMaxShield = characterBody.levelMaxShield,
                                    LevelRegen = characterBody.levelRegen
                                };

                                characterDefaults.Add(defaultStats);
                                logger.LogInfo($"Added defaults for {characterBody.name}");
                            }
                        }
                    }
                }

                // Store the character defaults in the game state service
                gameStateService.SetCharacterDefaults(characterDefaults);
                logger.LogInfo($"Character defaults collected: {characterDefaults.Count} characters");
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error collecting character defaults: {ex.Message}");
            }
        }

        private bool IsPlayableCharacter(string characterName)
        {
            // List of known playable characters
            var playableCharacters = new HashSet<string>
            {
                "CommandoBody",
                "HuntressBody", 
                "Bandit2Body",
                "ToolbotBody",
                "EngiBody",
                "MageBody",
                "MercBody",
                "TreebotBody",
                "LoaderBody",
                "CrocoBody",
                "CaptainBody",
                "RailgunnerBody",
                "VoidSurvivorBody",
                "SeekerBody",
                "FalseSonBody",
                "ChefBody"
            };

            return playableCharacters.Contains(characterName);
        }

        private string GetCharacterDisplayName(string characterName)
        {
            return characterName switch
            {
                "CommandoBody" => "Commando",
                "HuntressBody" => "Huntress",
                "Bandit2Body" => "Bandit",
                "ToolbotBody" => "MUL-T",
                "EngiBody" => "Engineer",
                "MageBody" => "Artificer",
                "MercBody" => "Mercenary",
                "TreebotBody" => "REX",
                "LoaderBody" => "Loader",
                "CrocoBody" => "Acrid",
                "CaptainBody" => "Captain",
                "RailgunnerBody" => "Railgunner",
                "VoidSurvivorBody" => "Void Fiend",
                "SeekerBody" => "Seeker",
                "FalseSonBody" => "False Son",
                "ChefBody" => "Chef",
                _ => characterName.Replace("Body", "")
            };
        }
    }
}