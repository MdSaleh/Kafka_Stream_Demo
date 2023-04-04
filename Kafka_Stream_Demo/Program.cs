using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using Streamiz.Kafka.Net;
using System;
using Kafka_Stream_Demo;

namespace Kafka_Stream_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton(new KStream("localhost:9092", "input-topic", "output-topic"));
            services.AddSingleton(new KafkaProducer("localhost:9092", "input-topic"));
            services.AddSingleton(new KafkaConsumer("localhost:9092", "output-topic"));
            services.AddHostedService<KafkaStreamService>();

            var serviceProvider = services.BuildServiceProvider();
            var kafkaProducer = serviceProvider.GetService<KafkaProducer>();

            kafkaProducer.Produce("sample message");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
