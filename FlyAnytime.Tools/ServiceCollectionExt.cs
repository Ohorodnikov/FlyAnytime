using FlyAnytime.Core.EfContextBase;
using FlyAnytime.Core.Entity;
using FlyAnytime.Core.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;

namespace FlyAnytime.Tools
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddLazy<TService, TImplementation>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            registration(typeof(TService), typeof(TImplementation));
            services.AddTransient(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));

            return services;
        }
        public static IServiceCollection AddIEntityAsBase(this IServiceCollection services)
        {
            services.AddAllImplementations<IEntity>(services.AddTransient);
            services.AddAllGenericImplementations(typeof(IEntityMap<,>), services.AddScoped);

            return services;
        }

        public static IServiceCollection SetCommonJwtSettings(this IServiceCollection services)
        {
            services
               .AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = true;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = JwtOptions.GetSymmetricSecurityKey(),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        RequireExpirationTime = false,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true
                    };
                });

            return services;
        }

        

        public static IServiceCollection AddAllImplementations<TInterface>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            var intType = typeof(TInterface);
            services.AddAllImplementationsImpl(intType, type => registration(intType, type));

            return services;
        }

        public static IServiceCollection AddAllImplementationsWithAllInterfaces<TInterface>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            services.AddAllImplementationsImpl(typeof(TInterface), type =>
            {
                var allInt = type.GetInterfaces();
                foreach (var interf in allInt)
                {
                    if (interf.IsGenericType)
                    {
                        services.AddAllGenericImplementations(interf.GetGenericTypeDefinition(), registration);
                    }
                    else
                    {
                        services.AddAllImplementationsImpl(interf, t => registration(interf, t));
                    }
                }
            });

            return services;
        }

        public static IServiceCollection AddAllSelfImplementations<TInterface>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            services.AddAllImplementationsImpl(typeof(TInterface), type => registration(type, type));

            return services;
        }

        private static IServiceCollection AddAllImplementationsImpl(this IServiceCollection services, Type interfaceType, Action<Type> registration)
        {
            var allTypes = TypesHelper.GetAllTypes();
            var implementations = allTypes
                .Where(type => type.IsImplementInterface(interfaceType))
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.IsGenericType)
                .ToList()
                ;

            foreach (var impl in implementations)
            {
                if (!services.HasRegistration(interfaceType, impl))
                    registration(impl);
            }

            return services;
        }

        public static IServiceCollection AddAllGenericImplementations(this IServiceCollection services, Type openInterf, Func<Type, Type, IServiceCollection> registration)
        {
            //https://stackoverflow.com/a/56144492
            TypesHelper.GetAllTypes()
            .Where(item => item.GetInterfaces()
                                .Where(i => i.IsGenericType)
                                .Any(i => i.GetGenericTypeDefinition() == openInterf)
                            && !item.IsAbstract && !item.IsInterface
                    )
            .ToList()
            .ForEach(assignedTypes =>
            {
                var serviceType = assignedTypes.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == openInterf);
                if (!services.HasRegistration(serviceType, assignedTypes))
                    registration(serviceType, assignedTypes);
            });

            return services;
        }

        public static bool HasRegistration(this IServiceCollection services, Type serviceType, Type implType)
        {
            return services.Any(x => x.ImplementationType == implType && x.ServiceType == serviceType);
        }

        public static IServiceCollection AddChannel<TEntity>(this IServiceCollection services)
        {
            services.AddSingleton<Channel<TEntity>>(Channel.CreateUnbounded<TEntity>(new UnboundedChannelOptions() { SingleReader = true }));
            services.AddSingleton<ChannelReader<TEntity>>(svc => svc.GetRequiredService<Channel<TEntity>>().Reader);
            services.AddSingleton<ChannelWriter<TEntity>>(svc => svc.GetRequiredService<Channel<TEntity>>().Writer);
            return services;
        }
    }
}
