using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class CreateWalletCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var user = message.From;
            var username = user.Username ?? "Anonymous";

            if (message.Chat.Type != ChatType.Private)
            {
                return;
            }

            var existingWallet = Database.GetUserWallet(user.Id);
            if (existingWallet.Count > 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"⚠️ **Wallet Already Exists**\n\nYou already have a wallet with address: `{existingWallet[0].Address}`.\nIf you want to create a new wallet, please delete the current wallet using `/deleteWallet`.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            try
            {
                var data = await Fetch.GetAsync("create_wallet", new Dictionary<string, string> { { "userId", user.Id.ToString() }, { "username", username } });
                Database.Insert(new Database.Wallet
                {
                    UserId = user.Id,
                    Username = username,
                    Address = data.GetProperty("address").GetString(),
                    Secret = data.GetProperty("secret").GetString(),
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"🎉 **Congratulations, Your Wallet Created Successfully!**\n\n**Address**: `{data.GetProperty("address").GetString()}`\n**Secret Phrase**: `{data.GetProperty("secret").GetString()}`\n\n⚠️ **Important**: Save your secret phrase securely. Do not share it with anyone! Use it to recover your wallet if needed.",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"❌ **Error**: An unexpected error occurred: {ex.Message}",
                    parseMode: ParseMode.Markdown
                );
            }
        }
    }
}
