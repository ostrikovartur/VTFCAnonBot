using Telegram;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

TelegramBotClient botClient;

botClient = new TelegramBotClient("5629886428:AAGN3iJzNtPrRSVvB6A2TA0YiWRHIPvxH-o");

async Task StartReceiver()
{
    var token = new CancellationTokenSource();
    var cancelToken = token.Token;
    var reOpts = new ReceiverOptions { };
    await botClient.ReceiveAsync(Start, ErrorMessage, reOpts, cancelToken);
}

async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
{
    if (update.Message is Message message && update.Message.ToString() is "/start")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Привіт👋\r\nТи запустив анонімного чат-бота студентської ради ВТФК👨‍💻👩‍🎓\r\nВін створений для вивчення потреб та проблем студентів. Можеш відправити пропозиції, свої думки та скарги на рахунок навчання або організації діяльності нашого коледжу.\r\nПроте не забувай про ПРАВИЛА ПРИ ЗВЕРНЕННІ: \r\n1. Висловлюй свою думку чітко, без помилок😎\r\n2. Не забувай про культру мовлення🙃\r\n3. І пам‘ятай, цей бот орієнтований на серйозну роботу з покращення ТВОГО незабутнього періоду навчання😇\r\n4. Ім‘я вказуєте за бажанням («Без імені»)");
    }
}

async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
{
    if(update.Message is Message message)
    {
        var userName = message.From.Username;
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Hello! {userName}");
    }
}

async Task ErrorMessage(ITelegramBotClient botClient, Exception exception, CancellationToken cancellation)
{
    if(exception is ApiRequestException requestException)
    {
        await botClient.SendTextMessageAsync("", exception.Message.ToString());
    }
}

await StartReceiver();