using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class BackupCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            var args = message.Text.Split(' ');
            if (args.Length < 2)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "ℹ️ **Usage**: /backup {secretPhrase}",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            var secret = args[1];
            var user = message.From;
            var existingWallet = Database.GetUserWallet(user.Id);

            if (existingWallet.Count > 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"⚠️ **Wallet Exists**\n\nYou already have a wallet with address: `{existingWallet[0].Address}`.\nIf you have any secret phrases and want to backup, please delete your current wallet using `/deleteWallet` before recovering a new one.",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            try
            {
                var data = await Fetch.GetAsync("wallet", new Dictionary<string, string> { { "backup", secret } });
                Database.Insert(new Database.Wallet
                {
                    UserId = user.Id,
                    Username = user.Username ?? "Anonymous",
                    Address = data.GetProperty("address").GetString(),
                    Secret = secret,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"✅ **Wallet Recovered Successfully**\n\n**Address**: `{data.GetProperty("address").GetString()}`\n**Balance**: `{data.GetProperty("balance").GetString()}` **INF**\n**WalletUser's ID**: `{data.GetProperty("userId").GetString()}`\n**WalletUser's Username**: {data.GetProperty("username").GetString()}\n**Created**: `{data.GetProperty("createdAt").GetString()}`\n**Mining Streak**: `{data.GetProperty("miningStreak").GetString()}` days",
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
