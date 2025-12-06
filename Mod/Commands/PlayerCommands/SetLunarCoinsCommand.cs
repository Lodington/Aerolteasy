using System;
using BepInEx.Logging;
using RoR2;
using RoR2DevTool.Core;
using RoR2DevTool.Services;

namespace RoR2DevTool.Commands.PlayerCommands
{
    public class SetLunarCoinsCommand : BaseCommand
    {
        public override string CommandName => "setlunarcoins";

        public override void Execute(DevCommand command, ManualLogSource logger, GameStateService gameStateService)
        {
            try
            {
                int amount = GetIntFromCommand(command, "amount");
                int playerId = GetPlayerIdFromCommand(command);

                logger.LogInfo($"SetLunarCoins command received - Amount: {amount}, PlayerId: {playerId}");

                if (amount < 0)
                {
                    logger.LogWarning("SetLunarCoins: Amount cannot be negative");
                    return;
                }

                if (playerId == -1)
                {
                    // Set lunar coins for all players
                    foreach (var networkUser in NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser != null)
                        {
                            SetLunarCoinsForUser(networkUser, (uint)amount, logger);
                        }
                    }
                    logger.LogInfo($"Set lunar coins to {amount} for all players");
                }
                else
                {
                    // Set lunar coins for specific player
                    var networkUser = GetNetworkUserById(playerId);
                    if (networkUser != null)
                    {
                        SetLunarCoinsForUser(networkUser, (uint)amount, logger);
                        logger.LogInfo($"Set lunar coins to {amount} for player {playerId}");
                    }
                    else
                    {
                        logger.LogWarning($"Player {playerId} not found");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error setting lunar coins: {ex.Message}");
            }
        }

        private void SetLunarCoinsForUser(NetworkUser user, uint targetAmount, ManualLogSource logger)
        {
            try
            {
                uint currentAmount = user.lunarCoins;
                int difference = (int)targetAmount - (int)currentAmount;

                logger.LogInfo($"Setting lunar coins - Current: {currentAmount}, Target: {targetAmount}, Difference: {difference}");

                if (difference < 0)
                {
                    // Need to deduct coins
                    uint deductAmount = (uint)Math.Abs(difference);
                    logger.LogInfo($"Deducting {deductAmount} lunar coins");
                    user.DeductLunarCoins(deductAmount);
                }
                else if (difference > 0)
                {
                    // Need to award coins
                    logger.LogInfo($"Awarding {difference} lunar coins");
                    user.AwardLunarCoins((uint)difference);
                }
                else
                {
                    logger.LogInfo("Lunar coins already at target amount, no change needed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error setting lunar coins for user: {ex.Message}");
                logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
