using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaDemo.Producer
{
	internal interface IMessageProducer<in TMessage>
		where TMessage : class
	{
		Task ProduceMessage(TMessage message, Guid correlationId, CancellationToken cancellationToken);
	}
}
