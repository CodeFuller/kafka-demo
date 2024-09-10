using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace KafkaDemo.Producer
{
	internal class MessageProducer<TMessage> : IMessageProducer<TMessage>
		where TMessage : class
	{
		private readonly ITopicProducer<TMessage> topicProducer;

		public MessageProducer(ITopicProducer<TMessage> topicProducer)
		{
			this.topicProducer = topicProducer ?? throw new ArgumentNullException(nameof(topicProducer));
		}

		public async Task ProduceMessage(TMessage message, Guid correlationId, CancellationToken cancellationToken)
		{
			var pipe = Pipe.Execute<KafkaSendContext>(sendContext =>
			{
				sendContext.CorrelationId = correlationId;
			});

			await topicProducer.Produce(message, pipe, cancellationToken);
		}
	}
}
