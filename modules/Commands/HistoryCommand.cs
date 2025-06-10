using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class HistoryCommand
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
            if (!IsValidAddress(address))
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **Invalid Address**: Must start with 'IN' and be 26 characters long.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            try
            {
                var data = await Fetch.GetAsync("address_transactions", new Dictionary<string, string> { { "address", address } });
                if (data.GetArrayLength() == 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"ℹ️ **No Transactions Found** in your address: `{address}`",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }

                var reply = new StringBuilder($"📊 **Transactions for** `{address}`\n\n");
                foreach (var tx in data.EnumerateArray())
                {
                    reply.AppendLine($"**ID**: {tx.GetProperty("id").GetString()}");
                    reply.AppendLine($"**From**: `{tx.GetProperty("fromAddress").GetString() ?? "System"}`");
                    reply.AppendLine($"**To**: `{tx.GetProperty("toAddress").GetString()}`");
                    reply.AppendLine($"**Amount**: {tx.GetProperty("amount").GetDouble()} INF\n");
                }
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: reply.ToString(),
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

        private static bool IsValidAddress(string address)
        {
            return address.StartsWith("IN") && address.Length == 26;
        }
    }
}
