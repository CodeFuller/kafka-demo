using System.Threading.Tasks;
using KafkaDemo.Common;
using log4net;
using MassTransit;

namespace KafkaDemo.Consumer
{
	internal class DemoConsumer : IConsumer<DemoMessage>
	{
		private static readonly ILog Log = LogManager.GetLogger("DemoConsumer");

		public Task Consume(ConsumeContext<DemoMessage> context)
		{
			var message = context.Message;

			Log.Info($"Processing message with correlation id {context.CorrelationId}: '{message.Timestamp:s}: {message.Data}'");

			return Task.CompletedTask;
		}
	}
}
