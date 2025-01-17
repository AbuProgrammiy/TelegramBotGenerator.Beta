using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using TgUser = Telegram.Bot.Types.User;
using User = CQRSTemplate.Domain.Entities.Models.PrimaryModels.User;
using CQRSTemplate.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CQRSTemplate.Domain.Entities.Enums;

namespace CQRSTemplate.Application.BackgroundServices
{
    public partial class MainBackgroundService
    {
        private async Task InitializeMainBot()
        {
            string mainBotToken = _configuration["MainTelegramBot"];
            _mainBotClient = new TelegramBotClient(mainBotToken);

            _mainBotClient.StartReceiving(HandleMainBotUpdateAsync, HandlePollingErrorAsync);
        }

        private async Task HandleMainBotUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is not null)
            {
                switch(update.Message.Text)
                {
                    case "/start":
                        await RegisterUser(botClient, update, cancellationToken);
                        break;
                    case "/generateterminator":
                        RequestToCreateBot(botClient, update, cancellationToken);
                        break;

                }
            }
        }

        private async Task RegisterUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == update.Message.Chat.Id);

            if (user == null)
            {
                await dbContext.Users.AddAsync(new User
                {
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName,
                    Username = update.Message.Chat.Username,
                    UserId = update.Message.Chat.Id,
                    Status = Domain.Entities.Enums.Status.NoStatus
                });
            }


            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendMessage(update.Message.Chat, $"Botga xush kelibsiz!\n" +
                                                             $"Xurmatli {update.Message.Chat.FirstName} {update.Message.Chat.LastName}");

            await botClient.SendMessage(update.Message.Chat, "Bot generatsiya qilish uchun /generateterminator ni yuboring.");
        }

        private async Task RequestToCreateBot(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == update.Message.Chat.Id);
            user.Status=Status.CreatingBot;

            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendMessage(update.Message.Chat, "Endi @BotFather dan bot generatsiya qilib uni tokenini menga tashlang!");
        }
    }
}
