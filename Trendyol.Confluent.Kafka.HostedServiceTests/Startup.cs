using System;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trendyol.Confluent.Kafka.HostedServiceTests.Services;

namespace Trendyol.Confluent.Kafka.HostedServiceTests
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private KafkaConfiguration _kafkaConfiguration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IService, Service>();
            services.AddScoped<IKafkaHelper, KafkaHelper>();

            services.AddHostedService<MyHostedService>();
            
            services.AddKafkaConsumer<MyConsumer>(configuration =>
            {
                configuration.Topic = Guid.NewGuid().ToString();
                configuration.GroupId = Guid.NewGuid().ToString();
                configuration.BootstrapServers = _configuration.GetValue<string>("BootstrapServers");
                configuration.AutoOffsetReset = AutoOffsetReset.Earliest;

                _kafkaConfiguration = configuration;
            });

            services.AddSingleton(_configuration);
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var kafkaHelper = app.ApplicationServices.GetRequiredService<IKafkaHelper>();
            
            kafkaHelper.CreateTopic(_kafkaConfiguration);
            kafkaHelper.BeginProducingMessages(_kafkaConfiguration);
        }
    }
}