using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class WithdrawCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var userId = message.From.Id;
            var senderWallet = Database.GetUserWallet(userId);

            if (senderWallet.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "⚠️ **No Wallet Found**\nYou don't have a wallet. Create one with /createWallet.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var senderAddress = senderWallet[0].Address;
            string amount = null;
            string toAddress = null;

            if (message.ReplyToMessage != null && message.ReplyToMessage.From != null)
            {
                if (message.Text.Split(' ').Length != 2)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ℹ️ **Usage (Reply Mode)**: Reply to a user with /withdraw {amount}",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                amount = message.Text.Split(' ')[1];
                var repliedUserId = message.ReplyToMessage.From.Id;
                var targetWallet = Database.GetUserWallet(repliedUserId);
                if (targetWallet.Count == 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"⚠️ **No Wallet Found**: The user ({message.ReplyToMessage.From.Username ?? "ID: " + repliedUserId}) has no wallet.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                toAddress = targetWallet[0].Address;
            }
            else
            {
                if (message.Text.Split(' ').Length != 3)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ℹ️ **Usage (Direct Mode)**: /withdraw {amount} {address}",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
                amount = message.Text.Split(' ')[1];
                toAddress = message.Text.Split(' ')[2];
                if (!IsValidAddress(toAddress))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "⚠️ **Invalid Address**: Must start with 'IN' and be 26 characters long.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }
            }

            var processingMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "⏳ **Processing Withdrawal...** Please wait.",
                parseMode: ParseMode.Markdown
            );

            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(4, 21)));

            try
            {
                var amountValue = double.Parse(amount);
                if (amountValue <= 0)
                {
                    await botClient.EditMessageTextAsync(
                        chatId: message.Chat.Id,
                        messageId: processingMessage.MessageId,
                        text: "⚠️ **Invalid Amount**: Amount must be greater than 0.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }

                var balanceData = await Fetch.GetAsync("wallet", new Dictionary<string, string> { { "address", senderAddress } });
                var balance = balanceData.GetProperty("balance").GetDouble();
                if (balance < amountValue)
                {
                    await botClient.EditMessageTextAsync(
                        chatId: message.Chat.Id,
                        messageId: processingMessage.MessageId,
                        text: $"⚠️ **Insufficient Balance**: Your wallet has {balance} INF, but you tried to withdraw {amountValue} INF.",
                        parseMode: ParseMode.Markdown
                    );
                    return;
                }

                var data = await Fetch.GetAsync("wallet", new Dictionary<string, string>
                {
                    { "withdraw", "True" },
                    { "address", senderAddress },
                    { "toAddress", toAddress },
                    { "amount", amount }
                });
                await botClient.EditMessageTextAsync(
                    chatId: message.Chat.Id,
                    messageId: processingMessage.MessageId,
                    text: $"✅ **Withdrawal Successful!**\n\n**Amount**: {amount} INF\n**From**: `{senderAddress}`\n**To**: `{toAddress}`\n**Transaction ID**: `{data.GetProperty("transactionId").GetString()}`",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (FormatException)
            {
                await botClient.EditMessageTextAsync(
                    chatId: message.Chat.Id,
                    messageId: processingMessage.MessageId,
                    text: "⚠️ **Invalid Amount**: Please enter a valid number.",
                    parseMode: ParseMode.Markdown
                );
            }
            catch (Exception ex)
            {
                await botClient.EditMessageTextAsync(
                    chatId: message.Chat.Id,
                    messageId: processingMessage.MessageId,
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
