using Google.Apis.Sheets.v4;
using System.ComponentModel;
using System.Text;
using System.Threading;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VTFCAnonBot;

TelegramBotClient botClient;

string googleToken = "";
string sheetFileName = "";
GoogleHelper helper;
string cellName = "";
string value = "";
var range = sheetFileName + "!" + cellName + ":" + cellName;
var values = new List<List<object>> { new List<object> { value } };
botClient = new TelegramBotClient("5629886428:AAHW9qgRwgR-rmo9kOyxhr5Gdmyk1v7DyWg");
Dictionary<long, string> chatStates = new Dictionary<long, string>();

async Task StartReceiver()
{
    var token = new CancellationTokenSource();
    var cancelToken = token.Token;
    var reOpts = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.CallbackQuery, UpdateType.Message } };

    await botClient.ReceiveAsync(HandleUpdateAsync, ErrorMessage, reOpts, cancelToken);
}
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message)
    {
        await OnMessage(botClient, update, cancellationToken);
    }
    else if (update.Type == UpdateType.CallbackQuery)
    {
        await OnCallbackQuery(botClient, update, update.CallbackQuery, cancellationToken);
    }
}
async Task Logger(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var message = update.Message;
    long chatId = message.Chat.Id;

    Console.OutputEncoding = Encoding.Unicode;
    Console.InputEncoding = Encoding.Unicode;

    // Виведення нікнейму користувача та його повідомлення в консоль
    Console.WriteLine($"Нікнейм користувача: {message.From.Username}");
    Console.WriteLine($"Повідомлення: {message.Text}");
}

async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CallbackQuery callbackQuery, CancellationToken cancellationToken)
{
    if (callbackQuery.Data == "button1")
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Скарга", "button2"),
            InlineKeyboardButton.WithCallbackData("Пропозиція", "button3")
        });
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Ви хочете надіслати нам скаргу чи пропозицію?",
            replyMarkup: inlineKeyboard);
    }

    if (callbackQuery.Data == "button2")
    {
        long chatId = callbackQuery.Message.Chat.Id;

        if (chatStates.ContainsKey(chatId) || chatStates[chatId] == "Очікує повідомлення")
        {
            await botClient.SendTextMessageAsync(chatId, "Напишіть на що ви хочете поскаржитись: ");
            chatStates[chatId] = "Очікує закінчення скарги";
        }
        //else if (chatStates[chatId] == "Очікує закінчення")
        //{
        //    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        //    {
        //    InlineKeyboardButton.WithCallbackData("Повернутись на головну", "button4")
        //});
        //    await botClient.SendTextMessageAsync(chatId, "Дякуємо за вашу скаргу! При першій нагоді ми її переглянемо та зробимо висновки.",
        //        replyMarkup: inlineKeyboard);
        //}
    }

    if (callbackQuery.Data == "button3")
    {
        long chatId = callbackQuery.Message.Chat.Id;

        if (chatStates.ContainsKey(chatId) || chatStates[chatId] == "Очікує повідомлення")
        {
            await botClient.SendTextMessageAsync(chatId, "Напишіть вашу пропозицію: ");
            chatStates[chatId] = "Очікує закінчення пропозиції";
        }
        //else if (chatStates[chatId] == "Очікує закінчення")
        //{
        //    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        //    {
        //        InlineKeyboardButton.WithCallbackData("Повернутись на головну", "button4")
        //    });
        //    await botClient.SendTextMessageAsync(chatId, "Дякуємо за вашу пропозицію! При першій нагоді ми її переглянемо та зробимо висновки.",
        //        replyMarkup: inlineKeyboard);
        //}
    }

    if (callbackQuery.Data == "button4")
    {
        long chatId = callbackQuery.Message.Chat.Id;

        // Видаліть стан чату
        if (chatStates.ContainsKey(chatId))
        {
            chatStates.Remove(chatId);
        }

        // Відправте повідомлення з початковими опціями
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Анонімно", "button1")
        });

        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Привіт👋\r\n" +
            "Ти запустив анонімного чат-бота студентської ради ВТФК👨‍💻👩‍🎓\r\n" +
            "Він створений для вивчення потреб та проблем студентів. Можеш відправити пропозиції, свої думки та скарги на рахунок навчання або організації діяльності нашого коледжу.\r\n" +
            "Проте не забувай про ПРАВИЛА ПРИ ЗВЕРНЕННІ: \r\n" +
            "1. Висловлюй свою думку чітко, без помилок😎\r\n" +
            "2. Не забувай про культру мовлення🙃\r\n" +
            "3. І пам‘ятай, цей бот орієнтований на серйозну роботу з покращення ТВОГО незабутнього періоду навчання😇\r\n" +
            "4. Ім‘я вказуєте за бажанням («Без імені»)\r\n" +
            "Для початку вкажіть ваше ім'я, або ж натисність кнопку 'Анонімно' щоб продовжити без імені",
            replyMarkup: inlineKeyboard);

        chatStates[callbackQuery.Message.Chat.Id] = "Очікує скаргу або пропозицію";
    }
}

async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
{
    if (update.Message is Message message)
    {
        if (!chatStates.ContainsKey(message.Chat.Id) || chatStates[message.Chat.Id] == "Очікує ім'я")
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Анонімно", "button1")
            });

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

            chatStates[message.Chat.Id] = "Очікує скаргу або пропозицію";
        }
        else if (chatStates[message.Chat.Id] == "Очікує скаргу або пропозицію")
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Скарга", "button2"),
                InlineKeyboardButton.WithCallbackData("Пропозиція", "button3")
            });
            // Користувач ввів своє ім'я або натиснув кнопку "Анонімно", тепер ви можете запитати про скаргу або пропозицію
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ви хочете надіслати нам скаргу чи пропозицію?",
                replyMarkup: inlineKeyboard);
        }
    }
    if (chatStates.ContainsKey(update.Message.Chat.Id) && chatStates[update.Message.Chat.Id] == "Очікує закінчення скарги")
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Повернутись на головну", "button4")
        });
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Дякуємо за вашу скаргу! При першій нагоді ми її переглянемо та зробимо висновки.",
            replyMarkup: inlineKeyboard);
    }

    if (chatStates.ContainsKey(update.Message.Chat.Id) && chatStates[update.Message.Chat.Id] == "Очікує закінчення пропозиції")
    {
        InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Повернутись на головну", "button4")
        });
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Дякуємо за вашу пропозицію! При першій нагоді ми її переглянемо та зробимо висновки.",
            replyMarkup: inlineKeyboard);
    }
    await Logger(botClient, update, cancellation);
}

async Task ErrorMessage(ITelegramBotClient botClient, Exception exception, CancellationToken cancellation)
{
    if (exception is ApiRequestException requestException)
    {
        await botClient.SendTextMessageAsync("Помилка: ", exception.Message.ToString());
    }
}

async Task SetData(string cellName, string value)
{
    var range = sheetFileName + "!" + cellName + ":" + cellName;
    var values = new List<List<object>> { new List<object> { value } };

    var requesn = SheetsService
}


//async Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
//{
//    if (update.Message?.Text is "/start")
//    {
//        await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Привіт👋\r\nТи запустив анонімного чат-бота студентської ради ВТФК👨‍💻👩‍🎓\r\nВін створений для вивчення потреб та проблем студентів. Можеш відправити пропозиції, свої думки та скарги на рахунок навчання або організації діяльності нашого коледжу.\r\nПроте не забувай про ПРАВИЛА ПРИ ЗВЕРНЕННІ: \r\n1. Висловлюй свою думку чітко, без помилок😎\r\n2. Не забувай про культру мовлення🙃\r\n3. І пам‘ятай, цей бот орієнтований на серйозну роботу з покращення ТВОГО незабутнього періоду навчання😇\r\n4. Ім‘я вказуєте за бажанням («Без імені»)");
//    }
//}

async Task SaveData()
{
    helper = new GoogleHelper(googleToken, sheetFileName);

    helper.Start();
}


await StartReceiver();