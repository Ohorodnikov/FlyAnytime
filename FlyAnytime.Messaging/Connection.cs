using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging
{
    public class Connection
    {
        private Connection() { }

        private static IConnection connection;
        private static IModel channel;

        private static object _lock = new object();
        private static object _lock2 = new object();

        public static IModel CreateChannel()
        {
            if (channel == null)
            {
                lock (_lock)
                {
                    if (channel == null)
                    {
                        channel = CreateConnection().CreateModel();
                    }
                }
            }

            return channel;
        }

        private static IConnection CreateConnection()
        {
            if (connection == null)
            {
                lock (_lock2)
                {
                    if (connection == null)
                    {
                        var fact = new ConnectionFactory
                        {
                            HostName = "localhost",
                        };

                        connection = fact.CreateConnection();
                    }
                }
            }
            

            return connection;
        }
    }
}
