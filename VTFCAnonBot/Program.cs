using Telegram;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

TelegramBotClient botClient;

botClient = new TelegramBotClient("5629886428:AAGN3iJzNtPrRSVvB6A2TA0YiWRHIPvxH-o");

async Task StartReceiver()
{
    var token = new CancellationTokenSource();
    var cancelToken = token.Token;
    var reOpts = new ReceiverOptions { };
    await botClient.ReceiveAsync(OnMessage, ErrorMessage, reOpts, cancelToken);
}

//async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
//{
//    if (update.Message?.Text is "/start")
//    {
//        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Привіт👋\r\nТи запустив анонімного чат-бота студентської ради ВТФК👨‍💻👩‍🎓\r\nВін створений для вивчення потреб та проблем студентів. Можеш відправити пропозиції, свої думки та скарги на рахунок навчання або організації діяльності нашого коледжу.\r\nПроте не забувай про ПРАВИЛА ПРИ ЗВЕРНЕННІ: \r\n1. Висловлюй свою думку чітко, без помилок😎\r\n2. Не забувай про культру мовлення🙃\r\n3. І пам‘ятай, цей бот орієнтований на серйозну роботу з покращення ТВОГО незабутнього періоду навчання😇\r\n4. Ім‘я вказуєте за бажанням («Без імені»)");
//    }
//}

async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
{
    if(update.Message is Message message)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton("Анонімно"));

        var userName = message.From.Username;
        await botClient.SendTextMessageAsync(message.Chat.Id, "Привіт👋\r\n" +
            "Ти запустив анонімного чат-бота студентської ради ВТФК👨‍💻👩‍🎓\r\n" +
            "Він створений для вивчення потреб та проблем студентів. Можеш відправити пропозиції, свої думки та скарги на рахунок навчання або організації діяльності нашого коледжу.\r\n" +
            "Проте не забувай про ПРАВИЛА ПРИ ЗВЕРНЕННІ: \r\n" +
            "1. Висловлюй свою думку чітко, без помилок😎\r\n" +
            "2. Не забувай про культру мовлення🙃\r\n" +
            "3. І пам‘ятай, цей бот орієнтований на серйозну роботу з покращення ТВОГО незабутнього періоду навчання😇\r\n" +
            "4. Ім‘я вказуєте за бажанням («Без імені»)\r\n" +
            "Для початку вкажіть ваше ім'я, або ж натисність кнопку 'Анонімно' щоб продовжити без імені",
            replyMarkup: inlineKeyboard);
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