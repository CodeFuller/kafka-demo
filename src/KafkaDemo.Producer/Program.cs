using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KafkaDemo.Common;
using log4net;
using log4net.Config;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaDemo.Producer
{
	public static class Program
	{
		private static readonly ILog Log = LogManager.GetLogger("KafkaDemo.Producer");

		public static async Task<int> Main()
		{
			XmlConfigurator.Configure(new FileInfo("log4net.config"));

			try
			{
				var configurationBuilder = new ConfigurationBuilder();
				configurationBuilder.AddJsonFile("AppSettings.json", optional: false);
				var configuration = configurationBuilder.Build();

				var services = new ServiceCollection();
				services.AddKafkaProducer(configuration.Bind);

				await using var serviceProvider = services.BuildServiceProvider();

				Log.Info("Starting the bus ...");
				var busControl = serviceProvider.GetRequiredService<IBusControl>();
				await busControl.StartAsync();
				Log.Info("The bus was started successfully");

				using var serviceScope = serviceProvider.CreateScope();
				var messageProducer = serviceScope.ServiceProvider.GetRequiredService<IMessageProducer<DemoMessage>>();

				await ProduceMessage(messageProducer, CancellationToken.None);

				Log.Info("Stopping the bus ...");
				await busControl.StopAsync();
				Log.Info("The bus was stopped successfully");

				return 0;
			}
#pragma warning disable CA1031
			catch (Exception e)
#pragma warning restore CA1031
			{
				Log.Fatal("Application has failed", e);
				return e.HResult;
			}
		}

		private static async Task ProduceMessage(IMessageProducer<DemoMessage> messageProducer, CancellationToken cancellationToken)
		{
			var correlationId = Guid.NewGuid();

			var message = new DemoMessage
			{
				Data = "Hello, World! :)",
				Timestamp = DateTimeOffset.Now,
			};

			Log.Info($"Producing message with correlation id {correlationId} ...");
			await messageProducer.ProduceMessage(message, correlationId, cancellationToken);
			Log.Info("Message was produced successfully");
		}
	}
}