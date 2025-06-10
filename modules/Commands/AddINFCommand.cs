using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class AddINFCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            if (message.From.Id != Config.OWNER)
            {
                return;
            }

            string amount = null;
            string address = null;

            if (message.ReplyToMessage != null && message.ReplyToMessage.From != null)
            {
                if (message.Text.Split(' ').Length != 2)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ℹ️ **Usage (Reply Mode)**: Reply to a user with /addINF {amount}",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                amount = message.Text.Split(' ')[1];
                var repliedUserId = message.ReplyToMessage.From.Id;
                var wallet = Database.GetUserWallet(repliedUserId);
                if (wallet.Count == 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"⚠️ **No Wallet Found**: The user ({message.ReplyToMessage.From.Username ?? "ID: " + repliedUserId}) has no wallet.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                address = wallet[0].Address;
            }
            else
            {
                if (message.Text.Split(' ').Length != 3)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ℹ️ **Usage (Direct Mode)**: /addINF {amount} {address}",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                amount = message.Text.Split(' ')[1];
                address = message.Text.Split(' ')[2];
                if (!IsValidAddress(address))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "⚠️ **Invalid Address**: Must start with 'IN' and be 26 characters long.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
            }

            try
            {
                var amountValue = double.Parse(amount);
                var data = await Fetch.GetAsync("wallet", new Dictionary<string, string> { { "addINF", "True" }, { "address", address }, { "amount", amount } });
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"✅ **INF Added Successfully!**\n\n**Amount**: `{amount}` INF\n**Address**: `{address}`\n**Transaction ID**: `{data.GetProperty("transactionId").GetString()}`",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (FormatException)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **Invalid Amount**: Please enter a valid number.",
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
