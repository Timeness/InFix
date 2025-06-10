using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class BlockCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var args = message.Text.Split(' ');
            if (args.Length < 2)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "ℹ️ **Usage**: /block {blockIndex}",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var index = args[1];
            try
            {
                var data = await Fetch.GetAsync("block", new Dictionary<string, string> { { "index", index } });
                var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(data.GetProperty("timestamp").GetInt64()).ToString("yyyy-MM-dd HH:mm:ss");
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"🧱 **Block #{data.GetProperty("index").GetInt32()}**\n\n**Hash**: `{data.GetProperty("hash").GetString()}`\n**Previous Hash**: `{data.GetProperty("previousHash").GetString()}`\n**Timestamp**: `{timestamp}`\n**Transactions**: `{data.GetProperty("transactions").GetArrayLength()}`",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (FormatException)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **Invalid Index**: Please enter a valid block number.",
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
