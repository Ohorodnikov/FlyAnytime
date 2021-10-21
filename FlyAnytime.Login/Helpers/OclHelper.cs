using FlyAnytime.Login.EF;
using FlyAnytime.Login.Models;
using FlyAnytime.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Helpers
{
    public interface IOclHelper
    {
        Task<OneClickLogin> FindOneClickLoginByUrl(string userUrl);
        Task<OneClickLogin> Create(long userId, int lifetimeInSec = 300);
    }
    public class OclHelper : IOclHelper
    {
        LoginContext _dbContext;
        public OclHelper(LoginContext context)
        {
            _dbContext = context;
        }

        public async Task<OneClickLogin> Create(long userId, int lifetimeInSec = 300)
        {
            var user = await _dbContext.Set<User>().FindAsync(userId);

            if (user == null)
                return null;

            var now = DateTimeHelper.UnixNow;
            var ocl = new OneClickLogin
            {
                CreationDateTime = now,
                ExpireDateTime = now + lifetimeInSec,
                User = user,
                LoginUrl = Guid.NewGuid().ToString().Replace("-", "")
            };

            _dbContext.Set<OneClickLogin>().Add(ocl);

            await _dbContext.SaveChangesAsync();

            return ocl;
        }

        public async Task<OneClickLogin> FindOneClickLoginByUrl(string userUrl)
        {
            return await _dbContext.Set<OneClickLogin>()
                                    .Include(x => x.User)
                                    .FirstOrDefaultAsync(x => x.LoginUrl == userUrl);
        }
    }
}
