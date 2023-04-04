using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using System;

namespace Kafka_Stream_Demo
{
    public class KStream
    {
        private readonly string bootstrapServers;
        private readonly string inputTopic;
        private readonly string outputTopic;

        public KStream(string bootstrapServers, string inputTopic, string outputTopic)
        {
            this.bootstrapServers = bootstrapServers;
            this.inputTopic = inputTopic;
            this.outputTopic = outputTopic;
        }

        public async Task Start()
        {
            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "kafka-stream-example";
            config.BootstrapServers = bootstrapServers;

            var builder = new StreamBuilder();
            builder.Stream<string, string>(inputTopic)
                .MapValues(value => value.ToUpper())
                .To(outputTopic);

            var topology = builder.Build();
            var stream = new KafkaStream(topology, config);
            await stream.StartAsync();
        }
    }
}
