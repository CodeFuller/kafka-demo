using System;

namespace KafkaDemo.Consumer.Settings
{
	public class KafkaCheckpointSettings
	{
		public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);

		public ushort MessageCount { get; set; } = 5000;
	}
}
