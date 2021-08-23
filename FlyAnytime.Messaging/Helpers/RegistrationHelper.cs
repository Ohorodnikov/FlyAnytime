using FlyAnytime.Messaging.Messages;
using FlyAnytime.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlyAnytime.Messaging.Helpers
{
    public static class RegistrationHelper
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitConnection, RabbitMqConnection>();
            services.AddSingleton<IMessageBus, RabbitMessageBus>();

            services.AddAllSelfImplementations<IMessageHandler>(services.AddTransient);

            CheckResponseMessageOnHavingPrivateCtor();

            return services;
        }

        private static void CheckResponseMessageOnHavingPrivateCtor()
        {
            var types = TypesHelper.GetAllTypes()
                .Where(t => typeof(BaseMessage).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToList();

            foreach (var type in types)
            {
                var baseType = type.BaseType;

                while (baseType != typeof(object))
                {
                    if (baseType.IsConstructedGenericType)
                    {
                        var genericDefinition = baseType.GetGenericTypeDefinition();

                        if (genericDefinition == typeof(BaseResponseMessage<>))
                        {
                            var privateParametrelessCtor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(x => x.GetParameters().Count() == 0);

                            if (privateParametrelessCtor == null)
                                throw new NoPrivateCtorException(type);

                            break;
                        }
                    }

                    baseType = baseType.BaseType;
                }
            }

        }
    }
}
