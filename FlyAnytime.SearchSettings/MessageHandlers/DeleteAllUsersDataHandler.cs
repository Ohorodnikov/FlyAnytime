using FlyAnytime.Messaging.Messages;
using FlyAnytime.SearchSettings.Models;
using FlyAnytime.SearchSettings.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.MessageHandlers
{
    public class DeleteAllUsersDataHandler : IMessageHandler<DeleteAllUsersDataMessage>
    {
        private readonly IMongoDbContext _context;

        public DeleteAllUsersDataHandler(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAllUsersDataMessage message)
        {
            await _context.Set<Chat>().RemoveAll();
            await _context.Set<User>().RemoveAll();
        }
    }
}
