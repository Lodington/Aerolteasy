using System.Linq;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands
{
    public abstract class BaseCommand : IDevCommand
    {
        public abstract string CommandName { get; }
        
        public abstract void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService);

        protected NetworkUser GetNetworkUserById(int playerId)
        {
            foreach (var networkUser in NetworkUser.readOnlyInstancesList)
            {
                if ((int)networkUser.id.value == playerId)
                {
                    return networkUser;
                }
            }
            return null;
        }

        protected int GetPlayerIdFromCommand(DevCommand command, int defaultValue = -1)
        {
            return command.Data.ContainsKey("playerId") ? int.Parse(command.Data["playerId"].ToString()) : defaultValue;
        }

        protected string GetStringFromCommand(DevCommand command, string key)
        {
            return command.Data.ContainsKey(key) ? command.Data[key].ToString() : null;
        }

        protected bool GetBoolFromCommand(DevCommand command, string key, bool defaultValue = false)
        {
            return command.Data.ContainsKey(key) ? bool.Parse(command.Data[key].ToString()) : defaultValue;
        }

        protected int GetIntFromCommand(DevCommand command, string key, int defaultValue = 0)
        {
            return command.Data.ContainsKey(key) ? int.Parse(command.Data[key].ToString()) : defaultValue;
        }

        protected float GetFloatFromCommand(DevCommand command, string key, float defaultValue = 0f)
        {
            return command.Data.ContainsKey(key) ? float.Parse(command.Data[key].ToString()) : defaultValue;
        }

        protected uint GetUIntFromCommand(DevCommand command, string key, uint defaultValue = 0)
        {
            return command.Data.ContainsKey(key) ? uint.Parse(command.Data[key].ToString()) : defaultValue;
        }
    }
}