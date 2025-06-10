using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class DeleteWalletCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var user = message.From;
            var wallet = Database.GetUserWallet(user.Id);

            if (wallet.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **No Wallet Found**\nYou don't have a wallet to delete. Create one with /createWallet.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var address = wallet[0].Address;
            try
            {
                var data = await Fetch.GetAsync("delete_wallet", new Dictionary<string, string> { { "address", address } });
                Database.Remove(user.Id);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "🗑️ **Wallet Deleted Successfully**",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"❌ **Error**: {ex.Message}",
                    parseMode: ParseMode.Markdown
                );
            }
        }
    }
}
