using System;

namespace KafkaDemo.Common
{
	public class DemoMessage
	{
		public Guid ClientId { get; set; }

		public string Data { get; set; }

		public DateTimeOffset Timestamp { get; set; }
	}
}
