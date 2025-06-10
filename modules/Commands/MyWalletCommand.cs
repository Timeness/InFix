using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class MyWalletCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var user = message.From;
            var wallet = Database.GetUserWallet(user.Id);

            if (wallet.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **No Wallet Found**\nYou don't have a wallet yet. Create one using `/createWallet`.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var address = wallet[0].Address;
            try
            {
                var data = await Fetch.GetAsync("wallet", new Dictionary<string, string> { { "address", address } });
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"💼 **{user.FirstName}'s Wallet Details**:\n\n**Address**: `{data.GetProperty("address").GetString()}`\n**Balance**: `{data.GetProperty("balance").GetString()}` **INF**\n**User ID**: `{data.GetProperty("userId").GetString()}`\n**Username**: {data.GetProperty("username").GetString()}\n**Created**: `{data.GetProperty("createdAt").GetString()}`\n**Mining Streak**: `{data.GetProperty("miningStreak").GetString()}` days",
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
