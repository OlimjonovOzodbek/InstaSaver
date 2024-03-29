﻿using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1
{
    public class MyBot
    {
        //MyToken
        public string MyToken = "6351879801:AAEPXig2gHJfbMp9nJNb7EF82UGs1M1Qilo";



        public bool Follow = false;
        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient(MyToken);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for@{me.Username}");
            Console.ReadLine();

            cts.Cancel();

        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            string replaceMessage = message.Text!.Replace("www.", "dd");

            if (message.Text == "/start")
            {
                ChatMember membership = await botClient.GetChatMemberAsync("@ksebfcks", userId: message.Chat.Id);

                if (membership != null && membership.Status != ChatMemberStatus.Member && membership.Status != ChatMemberStatus.Administrator && membership.Status != ChatMemberStatus.Creator)
                {
                    await ForceUserToSubscribe();
                }

                else
                {
                    Follow = true;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Send link - > ",
                        cancellationToken: cancellationToken);
                }
            }

            else if (message.Text.StartsWith("https://www.instagram.com"))
            {
                if (Follow == true)
                {
                    try
                    {
                        await botClient.SendVideoAsync(
                           chatId: message.Chat.Id,
                           video: $"{replaceMessage}",
                           supportsStreaming: true,
                           cancellationToken: cancellationToken);
                    }
                    catch (Exception) { }

                    try
                    {
                        await botClient.SendPhotoAsync(
                               chatId: message.Chat.Id,
                               photo: $"{replaceMessage}",
                               cancellationToken: cancellationToken);
                    }
                    catch (Exception) { }
                }
                return;
            }


            async Task ForceUserToSubscribe()
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                      {
                        new []
                        {
                            InlineKeyboardButton.WithUrl(text: "Subscribe ", url: "https://t.me/zokirovb"),
                        },
                      }
                );

                await botClient.SendTextMessageAsync(
                chatId: message!.Chat.Id,
                text: "Hello",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
