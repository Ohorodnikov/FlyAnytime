using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.MessageHandlers
{
    public class DeleteAllUsersDataHandler : IMessageHandler<DeleteAllUsersDataMessage>
    {
        private readonly TelegramContext _context;

        public DeleteAllUsersDataHandler(TelegramContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAllUsersDataMessage message)
        {
            await _context.Set<Poll>().RemoveAll();
            await _context.Set<ChatConversation>().RemoveAll();
            await _context.Set<Chat>().RemoveAll();
            await _context.Set<User>().RemoveAll();
        }
    }
}
