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

namespace FlyAnytime.Tools
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddIEntityAsBase(this IServiceCollection services)
        {
            services.AddAllImplementations<IEntity>(services.AddTransient);
            services.AddAllGenericImplementations(typeof(IEntityMap<>), services.AddScoped);

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
            services.AddAllImplementationsImpl<TInterface>(type => registration(intType, type));

            return services;
        }

        public static IServiceCollection AddAllSelfImplementations<TInterface>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            services.AddAllImplementationsImpl<TInterface>(type => registration(type, type));

            return services;
        }

        private static IServiceCollection AddAllImplementationsImpl<TInterface>(this IServiceCollection services, Action<Type> registration)
        {
            var allTypes = TypesHelper.GetAllTypes();
            var intType = typeof(TInterface);
            var implementations = allTypes
                .Where(type => type.IsImplementInterface(intType))
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.IsGenericType)
                ;

            foreach (var impl in implementations)
            {
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
                var serviceType = assignedTypes.GetInterfaces().First(i => i.GetGenericTypeDefinition() == openInterf);
                registration(serviceType, assignedTypes);
            });

            return services;
        }
    }
}
