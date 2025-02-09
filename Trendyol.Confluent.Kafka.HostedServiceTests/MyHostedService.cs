using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Trendyol.Confluent.Kafka.HostedServiceTests.Services;

namespace Trendyol.Confluent.Kafka.HostedServiceTests
{
    public class MyHostedService : BackgroundService
    {
        private readonly MyConsumer _myConsumer;
        private readonly IKafkaHelper _kafkaHelper;
        
        public MyHostedService(MyConsumer myConsumer, 
            IKafkaHelper kafkaHelper)
        {
            _myConsumer = myConsumer;
            _kafkaHelper = kafkaHelper;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _myConsumer.RunAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _kafkaHelper.DeleteTopics();
            
            base.Dispose();
        }
    }
}