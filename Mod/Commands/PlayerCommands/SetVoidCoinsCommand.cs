using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class SetVoidCoinsCommand : BaseCommand
    {
        public override string CommandName => "setvoidcoins";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                int amount = GetIntFromCommand(command, "amount");
                int playerId = GetPlayerIdFromCommand(command);

                if (amount < 0)
                {
                    logger.LogWarning("SetVoidCoins: Amount cannot be negative");
                    return;
                }

                if (playerId == -1)
                {
                    // Set void coins for all players
                    foreach (var master in CharacterMaster.readOnlyInstancesList)
                    {
                        if (master != null && master.playerCharacterMasterController != null)
                        {
                            uint currentVoidCoins = master.voidCoins;
                            int difference = amount - (int)currentVoidCoins;
                            
                            logger.LogInfo($"Player {master.playerCharacterMasterController.networkUser?.userName ?? "Unknown"}: Current={currentVoidCoins}, Target={amount}, Difference={difference}");
                            
                            master.GiveVoidCoins((uint)difference);
                        }
                    }
                    logger.LogInfo($"Set void coins to {amount} for all players");
                }
                else
                {
                    // Set void coins for specific player
                    var networkUser = GetNetworkUserById(playerId);
                    if (networkUser != null)
                    {
                        var master = networkUser.master;
                        if (master != null)
                        {
                            uint currentVoidCoins = master.voidCoins;
                            int difference = amount - (int)currentVoidCoins;
                            
                            logger.LogInfo($"Current void coins: {currentVoidCoins}, Target: {amount}, Difference: {difference}");
                            
                            master.GiveVoidCoins((uint)difference);
                            
                            logger.LogInfo($"Set void coins to {amount} for player {playerId}");
                        }
                        else
                        {
                            logger.LogWarning($"Player {playerId} has no CharacterMaster");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Player {playerId} not found");
                    }
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Error setting void coins: {ex.Message}");
                logger.LogError(ex.StackTrace);
            }
        }
    }
}
