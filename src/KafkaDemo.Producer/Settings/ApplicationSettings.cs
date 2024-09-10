using KafkaDemo.Common.Settings;

namespace KafkaDemo.Producer.Settings
{
	public class ApplicationSettings
	{
		public KafkaSettings Kafka { get; set; }

		public ProducerSettings DemoProducer { get; set; }
	}
}
