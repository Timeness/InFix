using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class TransactionCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var args = message.Text.Split(' ');
            if (args.Length < 2)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "ℹ️ **Usage**: /transaction {transaction_hash}",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var transactionHash = args[1];
            try
            {
                var data = await Fetch.GetAsync("transaction", new Dictionary<string, string> { { "id", transactionHash } });
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"📜 **Transaction Details**:\n\n**ID**: `{data.GetProperty("id").GetString()}`\n**Time**: `{data.GetProperty("time").GetString()}`\n**Date**: `{data.GetProperty("date").GetString()}` | {data.GetProperty("day").GetString()}\n**From User ID**: `{data.GetProperty("fromUserId").GetString()}`\n**From Username**: {data.GetProperty("fromUsername").GetString()}\n**From Address**: `{data.GetProperty("fromAddress").GetString()}`\n**To User ID**: `{data.GetProperty("toUserId").GetString()}`\n**To Username**: {data.GetProperty("toUsername").GetString()}\n**To Address**: `{data.GetProperty("toAddress").GetString()}`",
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
