using System;

namespace KafkaDemo.Common
{
	public class DemoMessage
	{
		public string Data { get; set; }

		public DateTimeOffset Timestamp { get; set; }
	}
}
