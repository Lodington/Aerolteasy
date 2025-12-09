using System.Collections.Generic;
using BepInEx.Logging;
using Newtonsoft.Json;
using RoR2DevTool.Core;
using RoR2DevTool.Services;
using UnityEngine.Networking;

namespace RoR2DevTool.Networking.Messages
{
    /// <summary>
    /// Message for executing commands across the network
    /// </summary>
    public class ExecuteCommandMessage : RoR2DevToolMessageBase
    {
        private string commandType;
        private string commandDataJson;
        
        // Static reference to services (set during initialization)
        public static CommandProcessor CommandProcessor;
        public static ManualLogSource Logger;

        public ExecuteCommandMessage()
        {
        }

        public ExecuteCommandMessage(DevCommand command)
        {
            commandType = command.Type;
            commandDataJson = JsonConvert.SerializeObject(command.Data);
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(commandType);
            writer.Write(commandDataJson);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            commandType = reader.ReadString();
            commandDataJson = reader.ReadString();
        }

        public override void Handle()
        {
            base.Handle();

            if (CommandProcessor == null || Logger == null)
            {
                return;
            }

            try
            {
                var command = new DevCommand
                {
                    Type = commandType,
                    Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(commandDataJson)
                };

                CommandProcessor.EnqueueCommand(command);
                Logger?.LogInfo($"Networked command executed: {commandType}");
            }
            catch (System.Exception ex)
            {
                Logger?.LogError($"Error executing networked command: {ex.Message}");
            }
        }
    }
}
