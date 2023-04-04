using Confluent.Kafka;
using Streamiz.Kafka.Net.SerDes;

namespace Kafka_Stream_Demo
{
    public class KafkaProducer
    {
        private readonly string bootstrapServers;
        private readonly string topic;

        public KafkaProducer(string bootstrapServers, string topic)
        {
            this.bootstrapServers = bootstrapServers;
            this.topic = topic;
        }

        public void Produce(string message)
        {
            using (var producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = bootstrapServers }).Build())
            {
                producer.ProduceAsync(topic, new Message<string, string> { Key = null, Value = message });
                producer.Flush();
            }
        }
    }
}
