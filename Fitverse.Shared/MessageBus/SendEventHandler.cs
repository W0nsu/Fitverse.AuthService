using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Fitverse.Shared.MessageBus
{
	public static class SendEventHandler
	{
		private static string _hostName;
		private static string _userName;
		private static string _password;
		private static IConnection _connection;

		public static void SendEvent<T>(T eventContent, IOptions<RabbitMqConfiguration> rabbitMqOptions,
			Tuple<string, string> exchangeConfig)
		{
			if (ConnectionExists(rabbitMqOptions))
			{
				using (var channel = _connection.CreateModel())
				{
					var json = JsonConvert.SerializeObject(eventContent);
					var body = Encoding.UTF8.GetBytes(json);

					var (exchange, routingKey) = exchangeConfig;
					channel.ExchangeDeclare(exchange, "direct");
					channel.BasicPublish(exchange, routingKey, null, body);
				}
			}
		}

		private static void CreateConnection()
		{
			try
			{
				var factory = new ConnectionFactory {HostName = _hostName, UserName = _userName, Password = _password};
				_connection = factory.CreateConnection();
			}
			catch
			{
				// ignored
			}
		}

		private static bool ConnectionExists(IOptions<RabbitMqConfiguration> rabbitMqOptions)
		{
			if (_connection != null)
				return true;

			_hostName = rabbitMqOptions.Value.Hostname;
			_userName = rabbitMqOptions.Value.UserName;
			_password = rabbitMqOptions.Value.Password;

			CreateConnection();

			return _connection != null;
		}
	}
}