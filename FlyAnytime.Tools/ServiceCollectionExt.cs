using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlyAnytime.Tools
{
    public static class ServiceCollectionExt
    {
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
                //.Where(type => typeof(IProfile).IsAssignableFrom(type));

            return allTypes;
        }
        public static IServiceCollection AddAllImplementations<TInterface>(this IServiceCollection services)
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
                services.AddSingleton(intType, impl);
            }

            return services;
        }
    }
}
