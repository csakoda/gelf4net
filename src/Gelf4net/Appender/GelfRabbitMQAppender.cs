using log4net.Appender;
using log4net.Util;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace gelf4net.Appender
{
    public class GelfRabbitMQAppender : GelfAmqpAppender
    {
        public GelfRabbitMQAppender(): base() { }

        public string RemoteExchange { get; set; }
        
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent).GzipMessage(Encoding);

            using (IConnection conn = ConnectionFactory.CreateConnection())
            {
                var model = conn.CreateModel();
                model.ExchangeDeclare(this.RemoteExchange, ExchangeType.Topic);
                byte[] messageBodyBytes = message;
                model.BasicPublish(RemoteExchange, "key", null, messageBodyBytes);
            }
        }
    }
}
