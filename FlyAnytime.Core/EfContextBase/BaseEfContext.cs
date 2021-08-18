using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.EfContextBase
{
    public abstract class BaseEfContext<TContext> : DbContext
        where TContext : BaseEfContext<TContext>
    {
        IServiceProvider _serviceProvider;

        private static bool _firstRun = true;

        public BaseEfContext(DbContextOptions<TContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;

            if (_firstRun)
                Database.EnsureDeleted();

            _firstRun = false;
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
