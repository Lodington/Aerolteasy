using System.Collections.Generic;
using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands
{
    public interface IDevCommand
    {
        string CommandName { get; }
        void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService);
    }
}