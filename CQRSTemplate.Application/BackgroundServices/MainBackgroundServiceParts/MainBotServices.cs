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
                        await RequestToCreateBot(botClient, update, cancellationToken);
                        break;
                    case "/bekor":
                        await CancelOperation(botClient, update, cancellationToken);
                        break;
                    default:
                        await CheckOtherOptions(update.Message.Text);
                        break;

                }

                async Task CheckOtherOptions(string messageText)
                {
                    IServiceScope scope = _serviceScopeFactory.CreateScope();
                    IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    User user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == update.Message.Chat.Id);

                    if(user.Status==Status.CreatingBot)
                    {
                        await CreateBot(botClient, update, cancellationToken);
                    }
                    else
                    {
                        await botClient.SendMessage(update.Message.Chat, "🤖: Tushunarsiz buyruq!");
                    }
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
                                                             $"Xurmatli {update.Message.Chat.FirstName} {update.Message.Chat.LastName}" +
                                                             $"Ushbu bot orqali o'zingizni botingizga gaplashishni o'rgatishingiz mumkin! 🦾\n\n" +
                                                             $"Masalan, botingizga:\n" +
                                                             $"S:qalaysan\n" +
                                                             $"J:yaxshi\n" +
                                                             $"Deb yuborsangiz, u hargal 'qalaysan' deb yozganingizda 'yaxshi' deb javob qaytaradi.");

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

        private async Task CreateBot(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            Bot bot = await dbContext.Bots.FirstOrDefaultAsync(b => b.Token == update.Message.Text);

            if(bot != null)
            {
                await botClient.SendMessage(update.Message.Chat, $"🤖: Bu bot oldin ro'yxatdan o'tgan!");
                return;
            }

            TelegramBotClient newBotClient;

            try
            {
                newBotClient = new TelegramBotClient(update.Message.Text);
            }
            catch (Exception ex)
            {
                await botClient.SendMessage(update.Message.Chat, $"⚠️: Token haqiqiy emas!\n" +
                                                                 $"Xatolik xabari: '{ex.Message}'\n\n" +
                                                                 $"Qayta jo'nating.\n" +
                                                                 $"Yoki operatsiyani bekor qilish uchun /bekor ni yuboring");
                return;
            }

            newBotClient = new TelegramBotClient(update.Message.Text);
            newBotClient.StartReceiving(HandleBotUpdateAsync, HandlePollingErrorAsync);
            _botClients.Add(newBotClient);

            User user = await dbContext.Users.FirstOrDefaultAsync(u=>u.UserId==update.Message.Chat.Id);   
            TgUser tgUser = await newBotClient.GetMe();

            bot = new Bot
            {
                BotId = newBotClient.BotId,
                FirstName = tgUser.FirstName,
                Username = tgUser.Username,
                Token = update.Message.Text,
                User = user
            };

            user.Status=Status.NoStatus;

            await dbContext.Bots.AddAsync(bot);
            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendMessage(update.Message.Chat, $"✅: Muvaffaqiyatli qo'shildi!\n\n" +
                                                             $"Bot haqida ma'lumot:\n" +
                                                             $"Nomi: {bot.FirstName}\n" +
                                                             $"Username: @{bot.Username}\n\n" +
                                                             $"Botingizga kirib /yordam ni yuboring uyog'ini botingiz tushuntiradi");
        }

        private async Task CancelOperation(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            User user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == update.Message.Chat.Id);

            if(user.Status==Status.NoStatus)
            {
                await botClient.SendMessage(update.Message.Chat, "Bekor qilish uchun hech qanday operatsiya yo'q!");
                return;
            }

            user.Status=Status.NoStatus;

            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendMessage(update.Message.Chat, "Operatsiya bekor qilindi!");
        }
    }
}
