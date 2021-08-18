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

        private static List<Type> allTypes;
        public static IEnumerable<Type> GetAllTypes()
        {
            if (allTypes != null)
                return allTypes;

            allTypes = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(Assembly.GetEntryAssembly())
                .SelectMany(x => x.DefinedTypes)
                .Select(x => x.AsType())
                .ToList();

            return allTypes;
        }

        public static IServiceCollection AddAllImplementations<TInterface>(this IServiceCollection services, Func<Type, Type, IServiceCollection> registration)
        {
            var allTypes = GetAllTypes();
            var intType = typeof(TInterface);
            var implementations = allTypes
                .Where(type => type.IsImplementInterface(intType))
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.IsGenericType)
                ;

            foreach (var impl in implementations)
            {
                registration(intType, impl);
            }

            return services;
        }

        public static IServiceCollection AddAllGenericImplementations(this IServiceCollection services, Type openInterf, Func<Type, Type, IServiceCollection> registration)
        {
            //https://stackoverflow.com/a/56144492
            GetAllTypes()
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
