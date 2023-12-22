namespace OnlineShoppingAPI
{
    using Confluent.Kafka;
    using MongoDB.Bson.IO;
    using Newtonsoft.Json;
    using System.Text.Json;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    [Target("KafkaAsync")]
    public class KafkaAsync : AsyncTaskTarget
    {
        // Pooling  
        private readonly ConcurrentQueue<IProducer<Null, string>> _producerPool;
        private int _pCount;
        private int _maxSize;

        public KafkaAsync()
        {
            _producerPool = new ConcurrentQueue<IProducer<Null, string>>();
            _maxSize = 10;
           
        }

        [RequiredParameter]
        public Layout Topic { get; set; }

        [RequiredParameter]
        public string BootstrapServers { get; set; }

        protected override void CloseTarget()
        {
            base.CloseTarget();
            _maxSize = 0;
            while (_producerPool.TryDequeue(out var context))
            {
                context.Dispose();
            }
        }

        private IProducer<Null, string> RentProducer()
        {
            if (_producerPool.TryDequeue(out var producer))
            {
                Interlocked.Decrement(ref _pCount);

                return producer;
            }

            var config = new ProducerConfig
            {
                BootstrapServers = BootstrapServers,
            };

            producer = new ProducerBuilder<Null, string>(config).Build();

            return producer;
        }

        private bool Return(IProducer<Null, string> producer)
        {
            if (Interlocked.Increment(ref _pCount) <= _maxSize)
            {
                _producerPool.Enqueue(producer);

                return true;
            }

            Interlocked.Decrement(ref _pCount);

            return false;
        }     
        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {


            // Using RenderLogEvent will allow NLog-Target to make optimal reuse of StringBuilder-buffers.  
            string topic = base.RenderLogEvent(this.Topic, logEvent);
            string msg = base.RenderLogEvent(this.Layout, logEvent);

            //string topic = this.Topic.Render(logEvent);  
            //string traceId = this.TraceId.Render(logEvent);  
            //string requestIp = this.RequestIp.Render(logEvent);  
            //string msg = this.Layout.Render(logEvent);  

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                level = logEvent.Level.Name.ToUpper(),
                @class = logEvent.LoggerName,
                message = msg
            });

            var producer = RentProducer();

            try
            {
                await producer.ProduceAsync(topic, new Message<Null, string>()
                {
                    Value = json
                });
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, $"kafka published error.");
            }
            finally
            {
                var returned = Return(producer);
                if (!returned)
                {
                    producer.Dispose();
                }
            }
        }
    }
}