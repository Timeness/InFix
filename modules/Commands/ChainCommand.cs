using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class ChainCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            try
            {
                var data = await Fetch.GetAsync("chain");
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"🔗 **Blockchain Overview**\n\n**Total Blocks**: `{data.GetProperty("length").GetInt32()}`\nUse /block {{blockIndex}} to view details of a specific block.",
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
