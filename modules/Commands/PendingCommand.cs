using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class PendingCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.From.Id != Config.OWNER)
            {
                return;
            }

            try
            {
                var data = await Fetch.GetAsync("pending_transactions");
                if (data.GetArrayLength() == 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ℹ️ **No Pending Transactions Found!**",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }

                var reply = new StringBuilder("⏳ **Pending Transactions**:\n\n");
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
    }
}
