using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Telegram.Bot.Conversations;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using FlyAnytime.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace FlyAnytime.Telegram.Bot
{
    public interface IBotHelper
    {
        ITelegramBotClient Bot { get; }
        TelegramContext DbContext { get; }
        IMessageBus MessageBus { get; }
        IServiceProvider ServiceProvider { get; }

        Task<global::Telegram.Bot.Types.Message> OnStartPrivateChat(long chatId);
        Task OnReStartPrivateChat(long chatId);
        Task OnKickFromPrivateChat(long chatId);
        Task OnAddToGroup(long chatId);
        Task OnRemoveFromGroup(long chatId);
        Task OnSetGroupAdmin(long chatId);
        Task OnRemoveFromGroupAdmin(long chatId);
    }

    public class BotHelper : IBotHelper
    {
        public ITelegramBotClient Bot { get; }
        public TelegramContext DbContext { get; }
        public IMessageBus MessageBus { get; }
        public IServiceProvider ServiceProvider { get; }

        public BotHelper(ITelegramBotClient bot, TelegramContext context, IMessageBus messageBus, IServiceProvider serviceProvider)
        {
            Bot = bot;
            DbContext = context;
            MessageBus = messageBus;
            ServiceProvider = serviceProvider;
        }

        public async Task<global::Telegram.Bot.Types.Message> OnStartPrivateChat(long chatId)
        {
            var savedChat = await DbContext.Set<Chat>().FindAsync(chatId);
            if (savedChat != null)
            {
                var restartDate = savedChat.RestartDateTime;
                if(restartDate.HasValue)
                {
                    var now = DateTimeHelper.UnixNow;

                    if (now - restartDate <= 10) //if restart chat has 2 actions: onRestart and /start
                        return null;
                }
                return await Bot.SendTextMessageAsync(chatId, "This chat is already registered");
            }
            
            var chat = await Bot.GetChatAsync(chatId);

            var dbChat = new Chat()
            {
                Id = chat.Id,
                CreationDateTime = DateTimeHelper.UnixNow,
                IsGroup = false,
                HasAdminRights = false,
                Username = chat.Username,
                Title = null,
                IsPaused = true,
                IsRemovedFromChat = false
            };

            var user = await DbContext.Set<User>().FindAsync(chat.Id);

            RegisterNewChatMessage regNewUser = null;
            if (user == null)
            {
                user = new User
                {
                    Id = chat.Id,
                    CreationDateTime = DateTimeHelper.UnixNow,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName,
                    UserName = chat.Username
                };
            }

            regNewUser = new RegisterNewChatMessage(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.UserName,

                    dbChat.Id,
                    dbChat.Title,
                    dbChat.IsGroup
                    );

            dbChat.ChatOwner = user;

            dbChat.UserLanguage = DbContext.Set<Language>().First();

            dbChat.SearchSettings = new ChatSearchSettingsBase();
            DbContext.Set<Chat>().Add(dbChat);

            await DbContext.SaveChangesAsync();

            if (regNewUser != null)
            {
                MessageBus.Publish(regNewUser);
            }


            var welcomeMsg = @"Hello!
This bot was created to find cheapest avia tickets from your location to any countries you want to.
Please, select your **language * *and * *country * *with * *city * *.
Then you need to write** country you want to fly to** and set** max ticket price**and we will send cheapes tickets ones a day.";

            await Bot.SendTextMessageAsync(chatId, welcomeMsg, ParseMode.Markdown);

            return await new UpdateSettingsFullConversation(this).Start(chatId);
        }

        public async Task OnReStartPrivateChat(long chatId)
        {
            var savedChat = await DbContext.Set<Chat>().FindAsync(chatId);

            if (savedChat == null)
            {
                await OnStartPrivateChat(chatId);
                return;
            }

            savedChat.IsPaused = true;
            savedChat.IsRemovedFromChat = false;
            savedChat.RestartDateTime = DateTimeHelper.UnixNow;


            DbContext.Set<Chat>().Update(savedChat);
            await DbContext.SaveChangesAsync();

            await Bot.SendTextMessageAsync(chatId, $"Welcome back, {savedChat.ChatOwner.FirstName} {savedChat.ChatOwner.LastName}! Press /help to get help text ");
        }
        public async Task OnKickFromPrivateChat(long chatId)
        {
            var savedChat = await DbContext.Set<Chat>().FindAsync(chatId);

            if (savedChat == null)
                return;

            savedChat.IsPaused = true;
            savedChat.IsRemovedFromChat = true;

            DbContext.Set<Chat>().Update(savedChat);
            await DbContext.SaveChangesAsync();
        }

        public async Task OnAddToGroup(long chatId)
        {

        }

        public async Task OnRemoveFromGroup(long chatId)
        {

        }

        public async Task OnSetGroupAdmin(long chatId)
        {

        }
        public async Task OnRemoveFromGroupAdmin(long chatId)
        {

        }
    }
}
