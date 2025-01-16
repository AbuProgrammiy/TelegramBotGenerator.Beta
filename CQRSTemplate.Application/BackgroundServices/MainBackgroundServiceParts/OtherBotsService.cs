using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using CQRSTemplate.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;

namespace CQRSTemplate.Application.BackgroundServices
{
    public partial class MainBackgroundService
    {
        private void InitializeOtherBots()
        {
            List<Bot> bots = _applicationDbContext.Bots.ToList();
            _botClients = new List<TelegramBotClient>(); 

            foreach (var bot in bots)
            {
                TelegramBotClient botClient = new TelegramBotClient(bot.Token);

                ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
                botClient.StartReceiving(HandleBotUpdateAsync, HandlePollingErrorAsync, receiverOptions);

                _botClients.Add(botClient);
            }
        }

        private async Task HandleBotUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is not null)
            {
                Console.WriteLine($"Bot received: {update.Message.Text}");
                await botClient.SendMessage(update.Message.Chat.Id, $"Echo: {update.Message.Text}", cancellationToken: cancellationToken);
            }
        }
    }
}
