using Microsoft.Extensions.Hosting;
using Streamiz.Kafka.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka_Stream_Demo
{
    public class KafkaStreamService : BackgroundService
    {
        private readonly KStream kafkaStream;
        public KafkaStreamService(KStream kafkaStream)
        {
            this.kafkaStream = kafkaStream;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await kafkaStream.Start();
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }
    }
}
