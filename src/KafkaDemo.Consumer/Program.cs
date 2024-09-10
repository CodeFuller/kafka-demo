using System;
using System.IO;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaDemo.Consumer
{
	public static class Program
	{
		private static readonly ILog Log = LogManager.GetLogger("KafkaDemo.Consumer");

		public static async Task<int> Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("log4net.config"));

			try
			{
				var builder = Host.CreateDefaultBuilder(args);

				builder.ConfigureAppConfiguration(configBuilder => configBuilder.AddJsonFile("AppSettings.json", optional: false));

				builder.ConfigureLogging(loggingBuilder =>
				{
					var loggingOptions = new Log4NetProviderOptions("log4net.config");

					loggingBuilder
						.ClearProviders()
						.AddLog4Net(loggingOptions);
				});

				builder.ConfigureServices((hostContext, services) =>
				{
					services.AddKafkaConsumer(hostContext.Configuration.Bind);
				});

				var host = builder.Build();
				await host.RunAsync();

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
	}
}
