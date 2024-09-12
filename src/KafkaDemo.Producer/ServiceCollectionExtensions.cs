using System;
using Confluent.Kafka;
using KafkaDemo.Common;
using KafkaDemo.Common.Settings;
using KafkaDemo.Producer.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaDemo.Producer
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddKafkaProducer(this IServiceCollection services, Action<ApplicationSettings> setupSettings)
		{
			var settings = new ApplicationSettings();
			setupSettings(settings);

			var kafkaSettings = settings.Kafka;

			services.AddMassTransit(busRegistrationConfigurator =>
			{
				busRegistrationConfigurator.UsingInMemory((context, config) => config.ConfigureEndpoints(context));

				busRegistrationConfigurator.AddRider(rider =>
				{
					rider.AddProducer<Guid, DemoMessage>(settings.DemoProducer.TopicName);

					rider.UsingKafka((_, kafkaFactoryConfigurator) =>
					{
						kafkaFactoryConfigurator.Host(kafkaSettings.Server, kafkaHostConfigurator =>
						{
							var sslSettings = kafkaSettings.Ssl;

							if (sslSettings.IsEnabled)
							{
								kafkaHostConfigurator.UseSsl(s =>
								{
									s.SslCaLocation = sslSettings.TruststoreCertLocation;
									s.EnableSslCertificateVerification = sslSettings.IsTruststoreCertVerification;

									s.SslCertificateLocation = sslSettings.KeystoreCertLocation;
									s.KeyLocation = sslSettings.KeystoreKeyLocation;
								});

								kafkaHostConfigurator.UseSasl(s =>
								{
									s.SecurityProtocol = SecurityProtocol.Ssl;
								});
							}
						});
					});
				});
			});

			services.AddScoped<IMessageProducer<DemoMessage>, DemoProducer>();

			return services;
		}
	}
}
