using Confluent.Kafka;
using Streamiz.Kafka.Net.SerDes;

namespace Kafka_Stream_Demo
{
    public class KafkaConsumer
    {
        private readonly string bootstrapServers;
        private readonly string topic;

        public KafkaConsumer(string bootstrapServers, string topic)
        {
            this.bootstrapServers = bootstrapServers;
            this.topic = topic;
        }

        public void Consume()
        {
            using (var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig { BootstrapServers = bootstrapServers, GroupId = "kafka-consumer-group" }).Build())
            {
                consumer.Subscribe(topic);
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                }
            }
        }
    }
}
