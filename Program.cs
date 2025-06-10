using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using InfixBot.modules.Commands;

namespace InfixBot
{
    class Program
    {
        private static TelegramBotClient botClient;

        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("YOUR_BOT_TOKEN_HERE"); // Replace with your bot token
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Bot started: @{me.Username}");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                CancellationToken.None
            );

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message?.Text != null)
                {
                    var message = update.Message;
                    var command = message.Text.Split(' ')[0].ToLower();

                    switch (command)
                    {
                        case "/startwallet":
                            await StartWalletCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/createwallet":
                            await CreateWalletCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/mywallet":
                            await MyWalletCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/backup":
                            await BackupCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/addinf":
                            await AddINFCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/chain":
                            await ChainCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/pending":
                            await PendingCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/block":
                            await BlockCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/transaction":
                            await TransactionCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/history":
                            await HistoryCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/mine":
                            await MineCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/deletewallet":
                            await DeleteWalletCommand.ExecuteAsync(botClient, message);
                            break;
                        case "/withdraw":
                            await WithdrawCommand.ExecuteAsync(botClient, message);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling update: {ex.Message}");
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception is Telegram.Bot.Exceptions.ApiRequestException apiException
                ? $"Telegram API Error: [{apiException.ErrorCode}] {apiException.Message}"
                : exception.ToString();
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
