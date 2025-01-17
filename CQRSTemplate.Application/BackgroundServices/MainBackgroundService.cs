using CQRSTemplate.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TgUser = Telegram.Bot.Types.User;

namespace CQRSTemplate.Application.BackgroundServices
{
    public partial class MainBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IApplicationDbContext _applicationDbContext;
        private List<TelegramBotClient> _botClients = new();
        private TelegramBotClient _mainBotClient;

        public MainBackgroundService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;

            IServiceScope scope = _serviceScopeFactory.CreateScope();
            _applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            InitializeMainBot().GetAwaiter().GetResult();
            InitializeOtherBots().GetAwaiter().GetResult();
        }

        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var me = await _mainBotClient.GetMe();
                Console.WriteLine($"Main Bot {me.Username} is running.");

                foreach (var botClient in _botClients)
                {
                    var botInfo = await botClient.GetMe();
                    Console.WriteLine($"Bot {botInfo.Username} is running.");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Polling error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
