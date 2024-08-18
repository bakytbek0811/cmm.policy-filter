using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CMM.PolicyFilter.Services
{
    public interface IMessageQueueService
    {
        void StartListening();
    }

    public class MessageQueueService : IMessageQueueService
    {
        private readonly IModel _channel;
        private readonly string _queueName = "chat-message-policy-filter-queue";
        private readonly IMessageService _messageService;

        public MessageQueueService(IModel channel, IMessageService messageService)
        {
            _channel = channel;
            _messageService = messageService;

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageString = Encoding.UTF8.GetString(body);
                    Console.WriteLine(messageString);

                    var message = _messageService.DeserializeMessage(messageString);

                    Console.WriteLine($"New message | {message.Id} | {DateTime.Now}");

                    await _messageService.CheckMessageForPolicy(message);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in StartListening(): {ex.Message} | {DateTime.Now}");

                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }
    }
}