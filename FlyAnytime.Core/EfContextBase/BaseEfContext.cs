using FlyAnytime.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Core.EfContextBase
{
    public abstract class BaseEfContext<TContext> : DbContext, IDbContextBase
        where TContext : BaseEfContext<TContext>
    {
        IServiceProvider _serviceProvider;

        public BaseEfContext(DbContextOptions<TContext> options, IServiceProvider serviceProvider) : base(options)
        {
            _serviceProvider = serviceProvider;

            Database.EnsureCreated();
        }

        public virtual async Task ReCreateDb()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entities = _serviceProvider.GetServices<IEntity>();

            foreach (var ent in entities)
            {
                var mapType = typeof(IEntityMap<,>).MakeGenericType(ent.GetType(), ent.GetType().GetProperty(nameof(IEntity.Id), BindingFlags.Instance | BindingFlags.Public).PropertyType);
                var map = (IEntityMap)_serviceProvider.GetService(mapType);

                if (!map.IsMapped())
                    continue;

                map.DoMap(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
