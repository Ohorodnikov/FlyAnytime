using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Telegram.Bot.Conversations;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace FlyAnytime.Telegram.Bot
{
    public interface IBotHelper
    {
        ITelegramBotClient Bot { get; }
        TelegramContext DbContext { get; }
        IMessageBus MessageBus { get; }

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

        public BotHelper(ITelegramBotClient bot, TelegramContext context, IMessageBus messageBus)
        {
            Bot = bot;
            DbContext = context;
            MessageBus = messageBus;
        }

        public async Task<global::Telegram.Bot.Types.Message> OnStartPrivateChat(long chatId)
        {
            var savedChat = await DbContext.Set<Chat>().FindAsync(chatId);
            if (savedChat != null)
            {
                var restartDate = savedChat.RestartDateTime;
                if(restartDate.HasValue)
                {
                    var now = DateTime.UtcNow;

                    if (now - restartDate <= new TimeSpan(0, 0, 10)) //if restart chat has 2 actions: onRestart and /start
                        return null;
                }
                return await Bot.SendTextMessageAsync(chatId, "This chat is already registered");
            }
            
            var chat = await Bot.GetChatAsync(chatId);

            var dbChat = new Chat()
            {
                Id = chat.Id,
                CreationDateTime = DateTime.UtcNow,
                IsGroup = false,
                HasAdminRights = false,
                Username = chat.Username,
                Title = null,
                IsPaused = true,
                IsRemovedFromChat = false
            };

            var user = await DbContext.Set<User>().FindAsync(chat.Id);

            RegisterNewUserMessage regNewUser = null;
            if (user == null)
            {
                user = new User
                {
                    Id = chat.Id,
                    CreationDateTime = DateTime.UtcNow,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName,
                    UserName = chat.Username
                };

                regNewUser = new RegisterNewUserMessage(user.Id, user.FirstName, user.LastName, user.UserName);
            }

            dbChat.ChatOwner = user;

            DbContext.Set<Chat>().Add(dbChat);


            var chatSettings = new ChatSettings
            {
                Chat = dbChat,
                UserLanguage = DbContext.Set<Language>().First(),
                SearchSettings = new ChatSearchSettingsBase
                {
                    Chat = dbChat
                }
            };

            DbContext.Add(chatSettings);

            await DbContext.SaveChangesAsync();

            if (regNewUser != null)
            {
                MessageBus.Publish(regNewUser);
            }

            await Bot.SendTextMessageAsync(chatId, "Welcome to bot!");

            //TODO: send /help text after welcome

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
            savedChat.RestartDateTime = DateTime.UtcNow;


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
