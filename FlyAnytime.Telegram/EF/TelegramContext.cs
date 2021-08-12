using FlyAnytime.Telegram.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.EF
{
    public class TelegramContext : DbContext
    {
        IServiceProvider _serviceProvider;

        public TelegramContext(DbContextOptions<TelegramContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;

            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entities = _serviceProvider.GetServices<IEntity>();

            foreach (var ent in entities)
            {
                var mapType = typeof(IEntityMap<>).MakeGenericType(ent.GetType());
                var map = (IEntityMap)_serviceProvider.GetService(mapType);

                if (!map.IsMapped())
                    continue;

                map.DoMap(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
