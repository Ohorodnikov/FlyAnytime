using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging
{
    public interface IRabbitConnection
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }

    public class RabbitMqConnection : IRabbitConnection
    {
        IConnection _connection;
        ConnectionFactory _connectionFactory;
        public RabbitMqConnection(IConfiguration configuration)
        {
            var cfg = configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();
            var host = cfg.HostName;
            _connectionFactory = new ConnectionFactory
            {
                HostName = host,
                DispatchConsumersAsync = true,
            };



            //_connection = fact.CreateConnection();

        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public bool TryConnect()
        {
            _connection = _connectionFactory.CreateConnection();

            return true;
        }
    }
}
