using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgUser = Telegram.Bot.Types.User;

namespace CQRSTemplate.Application.BackgroundServices
{
    public partial class MainBackgroundService
    {
        private async Task InitializeOtherBots()
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            List<Bot> bots = await dbContext.Bots.ToListAsync();

            foreach (var bot in bots)
            {
                TelegramBotClient botClient = new TelegramBotClient(bot.Token);

                botClient.StartReceiving(HandleBotUpdateAsync, HandlePollingErrorAsync);

                _botClients.Add(botClient);
            }
        }

        private async Task InitializeAnotherBot(string token)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            TelegramBotClient botClient = new TelegramBotClient(token);
            TgUser bot = await botClient.GetMe();
            botClient.StartReceiving(HandleBotUpdateAsync, HandlePollingErrorAsync);
            _botClients.Add(botClient);

            await dbContext.Bots.AddAsync(new Bot
            {
                Token = token,
                Username = bot.Username
            });

            await dbContext.SaveChangesAsync(new CancellationToken());
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
