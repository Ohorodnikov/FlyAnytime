using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class TypeExt
    {
        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsGenericType)
                return interfaceType.IsAssignableFrom(type);

            var allInterfaces = type.GetInterfaces().ToList();
            if (type.IsInterface)
            {
                allInterfaces.Add(type);
            }

            return allInterfaces
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }

        public static IEnumerable<PropertyInfo> GetAllPropsOfType(this Type objectType, Type propType)
        {
            var allProps = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in allProps)
            {
                var type2check = prop.PropertyType;

                if (type2check.IsGenericType && type2check.IsImplementInterface(typeof(IEnumerable<>)))
                    type2check = type2check.GetGenericArguments()[0];

                if (propType.IsInterface)
                {
                    if (type2check.IsImplementInterface(propType))
                    {
                        yield return prop;
                    }
                }
                else
                {
                    if (propType.IsAssignableFrom(type2check))
                    {
                        yield return prop;
                    }
                }
                
            }
        }
    }
}
