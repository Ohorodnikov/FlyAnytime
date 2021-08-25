using FlyAnytime.Login.EF;
using FlyAnytime.Login.Models;
using FlyAnytime.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.MessageHandlers
{
    public class RegisterNewUserHandler : IMessageHandler<RegisterNewUserMessage>
    {
        private readonly LoginContext _dbContext;

        public RegisterNewUserHandler(LoginContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(RegisterNewUserMessage message)
        {
            var user = new User
            {
                Id = message.UserId
            };

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
