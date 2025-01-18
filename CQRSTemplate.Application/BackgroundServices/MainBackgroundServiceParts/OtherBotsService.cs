using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Domain.Entities.Enums;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgUser = Telegram.Bot.Types.User;
using User = CQRSTemplate.Domain.Entities.Models.PrimaryModels.User;

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

        private async Task HandleBotUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text is not null)
            {
                if (update.Message.Text == "/yordam")
                {
                    await Help(botClient, update, cancellationToken);
                }
                else if(update.Message.Text.Contains("S:") && update.Message.Text.Contains("J:"))
                {
                    await AddVocabulary(botClient, update, cancellationToken);
                }
                else
                {
                    await AnswearTheQuestion(botClient, update, cancellationToken);
                }
            }
        }

        private async Task Help(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            Bot bot = await dbContext.Bots.FirstOrDefaultAsync(b => b.BotId == botClient.BotId && b.User.UserId == update.Message.Chat.Id);

            if(bot != null)
            {
                await botClient.SendMessage(update.Message.Chat, "🤖: Botga xush kelibsiz xo'jayin!\n" +
                                                                 "Men savollarga javob beruvchi botman!\n" +
                                                                 "Savol va javob qo'shish uchun shu formatda menga matn yuboring:\n\n" +
                                                                 "S:Qalaysan?\n" +
                                                                 "J:Yaxshi o'zinchi)");
                return;
            }

            await AnswearTheQuestion(botClient, update, cancellationToken);
        }

        private async Task AddVocabulary(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            Bot bot = await dbContext.Bots.FirstOrDefaultAsync(b => b.BotId == botClient.BotId && b.User.UserId == update.Message.Chat.Id);

            if (bot != null)
            {
                Regex regex = new Regex(@"S:(?<SValue>.+)\nJ:(?<JValue>.+)");

                Match match = regex.Match(update.Message.Text);

                string question = match.Groups["SValue"].Value.Trim();
                string answer = match.Groups["JValue"].Value.Trim();

                Vocabulary vocabulary=await dbContext.Vocabularies.FirstOrDefaultAsync(v=>v.Question.ToLower() == question.Trim().ToLower());

                if (vocabulary != null)
                {
                    vocabulary.Answer = answer;
                }
                else
                {
                    vocabulary = new Vocabulary
                    {
                        Question = question,
                        Answer = answer,
                        Bot=bot
                    };

                    await dbContext.Vocabularies.AddAsync(vocabulary);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                await botClient.SendMessage(update.Message.Chat, "✅: Muvaffaqiyatli qo'shildi!");

                return;
            }

            await AnswearTheQuestion(botClient, update, cancellationToken);
        }

        private async Task AnswearTheQuestion(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            Vocabulary vocabulary = await dbContext.Vocabularies.FirstOrDefaultAsync(v => v.Bot.BotId == botClient.BotId && v.Question.ToLower() == update.Message.Text.ToLower());

            if (vocabulary != null)
            {
                await botClient.SendMessage(update.Message.Chat, vocabulary.Answer);
                return;
            }

            await botClient.SendMessage(update.Message.Chat, "Tushunarsiz savol!");
        }
    }
}
