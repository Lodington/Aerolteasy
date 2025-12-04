using System.Collections.Generic;

namespace RoR2DevTool.Services.Helpers
{
    public class CharacterIconProvider
    {
        private readonly Dictionary<string, string> characterIconCache = new Dictionary<string, string>();

        public string GetCharacterIcon(string characterName)
        {
            if (characterIconCache.ContainsKey(characterName))
            {
                return characterIconCache[characterName];
            }

            var icon = GetIconForCharacter(characterName);
            characterIconCache[characterName] = icon;
            return icon;
        }

        private string GetIconForCharacter(string characterName)
        {
            return characterName switch
            {
                "CommandoBody" => "🔫",
                "HuntressBody" => "🏹",
                "Bandit2Body" => "🔪",
                "ToolbotBody" => "🤖",
                "EngiBody" => "🔧",
                "MageBody" => "🔮",
                "MercBody" => "⚔️",
                "TreebotBody" => "🌱",
                "LoaderBody" => "👊",
                "CrocoBody" => "🦎",
                "CaptainBody" => "⚓",
                "RailgunnerBody" => "🎯",
                "VoidSurvivorBody" => "🕳️",
                "SeekerBody" => "👁️",
                "FalseSonBody" => "👤",
                "ChefBody" => "👨‍🍳",
                _ => "👤"
            };
        }
    }
}
