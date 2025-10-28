using BepInEx.Logging;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.GameCommands
{
    public class SetMoneyCommand : BaseCommand
    {
        public override string CommandName => "setmoney";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            var playerId = GetPlayerIdFromCommand(command);
            var amount = GetUIntFromCommand(command, "amount");
            
            logger.LogInfo($"Setting money for player {playerId} to {amount}");
            SetMoney(amount, playerId, logger);
        }

        private void SetMoney(uint amount, int playerId, ManualLogSource logger)
        {
            var networkUser = GetNetworkUserById(playerId);
            if (networkUser?.master != null)
            {
                networkUser.master.money += amount;
                logger.LogInfo($"Added {amount} money to player {playerId}");
            }
            else
            {
                logger.LogWarning($"Player {playerId} not found");
            }
        }
    }
}