namespace KafkaDemo.Consumer.Settings
{
	public class KafkaEndpointSettings
	{
		public KafkaTopicSettings Topic { get; set; }

		public KafkaCheckpointSettings Checkpoint { get; set; }

		public KafkaScalabilitySettings Scalability { get; set; }
	}
}
