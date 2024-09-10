using System;
using Confluent.Kafka;
using KafkaDemo.Common;
using KafkaDemo.Consumer.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaDemo.Consumer
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, Action<ApplicationSettings> setupSettings)
		{
			var settings = new ApplicationSettings();
			setupSettings(settings);

			var kafkaSettings = settings.Kafka;

			services.AddMassTransit(massTransitConfigurator =>
			{
				massTransitConfigurator.UsingInMemory((context, config) => config.ConfigureEndpoints(context));

				massTransitConfigurator.AddRider(riderConfigurator =>
				{
					riderConfigurator.AddConsumer<DemoConsumer>();

					riderConfigurator.UsingKafka(
						(kafkaContext, kafkaFactoryConfigurator) =>
						{
							kafkaFactoryConfigurator.Host(kafkaSettings.Server, kafkaHostConfigurator =>
							{
								var sslSettings = kafkaSettings.Ssl;

								if (sslSettings.IsEnabled)
								{
									kafkaHostConfigurator.UseSsl(kafkaSslConfigurator =>
									{
										kafkaSslConfigurator.SslCaLocation = sslSettings.TruststoreCertLocation;
										kafkaSslConfigurator.EnableSslCertificateVerification = sslSettings.IsTruststoreCertVerification;

										kafkaSslConfigurator.SslCertificateLocation = sslSettings.KeystoreCertLocation;
										kafkaSslConfigurator.KeyLocation = sslSettings.KeystoreKeyLocation;
									});

									kafkaHostConfigurator.UseSasl(kafkaSslConfigurator =>
									{
										kafkaSslConfigurator.SecurityProtocol = SecurityProtocol.Ssl;
									});
								}
							});

							kafkaFactoryConfigurator.ConfigureTopicEndpoint(kafkaContext, settings.DemoConsumer.KafkaEndpoint);
						});
				});
			});

			return services;
		}

		private static void ConfigureTopicEndpoint(this IKafkaFactoryConfigurator configurator, IRegistrationContext context, KafkaEndpointSettings settings)
		{
			configurator.TopicEndpoint<DemoMessage>(settings.Topic.Name, settings.Topic.Consumer, endpointConfigurator =>
			{
				// https://masstransit.io/documentation/configuration/transports/kafka#checkpoint
				endpointConfigurator.CheckpointInterval = settings.Checkpoint.Interval;
				endpointConfigurator.CheckpointMessageCount = settings.Checkpoint.MessageCount;

				// https://masstransit.io/documentation/configuration/transports/kafka#scalability
				endpointConfigurator.ConcurrentConsumerLimit = settings.Scalability.ConcurrentConsumers;

				endpointConfigurator.AutoOffsetReset = AutoOffsetReset.Latest;

				endpointConfigurator.ConfigureConsumer<DemoConsumer>(context);
			});
		}
	}
}
