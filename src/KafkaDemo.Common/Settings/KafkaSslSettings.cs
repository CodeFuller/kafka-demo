namespace KafkaDemo.Common.Settings
{
	public class KafkaSslSettings
	{
		public bool IsEnabled { get; set; }

		public string TruststoreCertLocation { get; set; }

		public bool? IsTruststoreCertVerification { get; set; }

		public string KeystoreCertLocation { get; set; }

		public string KeystoreKeyLocation { get; set; }
	}
}
