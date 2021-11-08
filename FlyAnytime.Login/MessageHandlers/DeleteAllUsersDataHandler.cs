using FlyAnytime.Login.EF;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.MessageHandlers
{
    public class DeleteAllUsersDataHandler : IMessageHandler<DeleteAllUsersDataMessage>
    {
        private readonly LoginContext _context;

        public DeleteAllUsersDataHandler(LoginContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAllUsersDataMessage message)
        {
            await _context.ReCreateDb();
        }
    }
}
