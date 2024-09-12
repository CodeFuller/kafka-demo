using System;
using System.Threading;
using System.Threading.Tasks;
using KafkaDemo.Common;
using MassTransit;

namespace KafkaDemo.Producer
{
	internal class DemoProducer : IMessageProducer<DemoMessage>
	{
		private readonly ITopicProducer<Guid, DemoMessage> topicProducer;

		public DemoProducer(ITopicProducer<Guid, DemoMessage> topicProducer)
		{
			this.topicProducer = topicProducer ?? throw new ArgumentNullException(nameof(topicProducer));
		}

		public async Task ProduceMessage(DemoMessage message, Guid correlationId, CancellationToken cancellationToken)
		{
			var pipe = Pipe.Execute<KafkaSendContext>(sendContext =>
			{
				sendContext.CorrelationId = correlationId;
			});

			await topicProducer.Produce(message.ClientId, message, pipe, cancellationToken);
		}
	}
}
