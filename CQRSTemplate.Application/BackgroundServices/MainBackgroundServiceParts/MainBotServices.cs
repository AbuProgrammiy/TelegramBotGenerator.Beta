using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CQRSTemplate.Application.BackgroundServices
{
    public partial class MainBackgroundService
    {
        private void InitializeMainBot()
        {
            string mainBotToken = _configuration["MainTelegramBot"];
            _mainBotClient = new TelegramBotClient(mainBotToken);

            _mainBotClient.StartReceiving(HandleMainBotUpdateAsync, HandlePollingErrorAsync);
        }

        private async Task HandleMainBotUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is not null)
            {
                Console.WriteLine($"Main Bot received: {update.Message.Text}");
                await botClient.SendMessage(update.Message.Chat.Id, $"Echo: {update.Message.Text}", cancellationToken: cancellationToken);
            }
        }
    }
}
