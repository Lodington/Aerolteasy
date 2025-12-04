using System.Collections.Generic;
using RoR2DevTool.Models;

namespace RoR2DevTool.Services.Interfaces
{
    public interface IWorldDataCollector
    {
        List<MonsterData> GetMonsters();
        List<InteractableData> GetInteractables();
        List<ItemData> GetWorldItems();
        TeleporterData GetTeleporterData();
    }
}
