using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.EF
{
    public class TelegramContext : BaseEfContext<TelegramContext>
    {
        public TelegramContext(DbContextOptions<TelegramContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}
