using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfixBot.modules.Commands
{
    public static class StartWalletCommand
    {
        public static async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
        {
            const string startWalletMessage = @"
**Welcome to Infix Bot!**
Your gateway to the Infix blockchain. Manage your INF tokens with ease.

**Available Commands**:
👛 /createWallet - Create a new Infix wallet
💼 /myWallet - View your wallet details
🔄 /backup {secretPhrase} - Recover a wallet using your secret phrase
🏧 /withdraw {amount} {address} - Withdraw INF to an address
🗑️ /deleteWallet - Delete your current wallet
📜 /transaction {transactionHash} - View transaction details of specefic transactionHash
⛏️ /mine - Mine 10 INF daily
🔗 /chain - View Infix blockchain info
🧱 /block {blockIndex} - View block details
📊 /history - View your all transactions";

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: startWalletMessage,
                parseMode: ParseMode.Markdown
            );
        }
    }
}
