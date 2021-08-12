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
        Task OnStartPrivateChat(long chatId);
        Task OnReStartPrivateChat(long chatId);
        Task OnKickFromPrivateChat(long chatId);
        Task OnAddToGroup(long chatId);
        Task OnRemoveFromGroup(long chatId);
        Task OnSetGroupAdmin(long chatId);
        Task OnRemoveFromGroupAdmin(long chatId);
    }

    public class BotHelper : IBotHelper
    {
        ITelegramBotClient _bot;
        TelegramContext _context;
        public BotHelper(ITelegramBotClient bot, TelegramContext context)
        {
            _bot = bot;
            _context = context;
        }

        public async Task OnStartPrivateChat(long chatId)
        {
            if (_context.Set<Chat>().Any(x => x.Id == chatId))
                return;
            
            var chat = await _bot.GetChatAsync(chatId);

            var dbChat = new Chat()
            {
                Id = chat.Id,
                IsGroup = false,
                HasAdminRights = false,
                Username = chat.Username,
                Title = null,
                IsPaused = true,
                IsRemovedFromChat = false
            };

            var user = await _context.Set<User>().FindAsync(chat.Id);

            if (user == null)
            {
                user = new User
                {
                    Id = chat.Id,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName,
                    UserName = chat.Username
                };
            }

            dbChat.ChatOwner = user;

            _context.Set<Chat>().Add(dbChat);

            await _context.SaveChangesAsync();

        }
        public async Task OnReStartPrivateChat(long chatId)
        {

        }
        public async Task OnKickFromPrivateChat(long chatId)
        {

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
