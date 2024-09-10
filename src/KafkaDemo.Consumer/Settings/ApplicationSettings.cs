using KafkaDemo.Common.Settings;

namespace KafkaDemo.Consumer.Settings
{
	public class ApplicationSettings
	{
		public KafkaSettings Kafka { get; set; }

		public ConsumerSettings DemoConsumer { get; set; }
	}
}
