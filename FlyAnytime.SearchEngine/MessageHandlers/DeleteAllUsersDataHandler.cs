using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Messaging.Messages;
using FlyAnytime.SearchEngine.EF;
using FlyAnytime.SearchEngine.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.MessageHandlers
{
    public class DeleteAllUsersDataHandler : IMessageHandler<DeleteAllUsersDataMessage>
    {
        private readonly SearchEngineContext _context;

        public DeleteAllUsersDataHandler(SearchEngineContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteAllUsersDataMessage message)
        {
            //await _context.Set<SearchResultItem>().RemoveAll();
        }
    }
}
