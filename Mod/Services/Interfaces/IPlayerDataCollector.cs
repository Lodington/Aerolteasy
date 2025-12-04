using System.Collections.Generic;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services.Interfaces
{
    public interface IPlayerDataCollector
    {
        List<PlayerData> GetAllPlayers();
        void SetPlayerGodMode(int playerId, bool enabled);
        void ApplyGodMode();
    }
}
