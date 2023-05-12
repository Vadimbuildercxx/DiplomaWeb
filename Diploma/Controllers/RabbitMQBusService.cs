using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;
using Diploma.Hubs;
using Microsoft.AspNetCore.SignalR;
using Diploma.Models;
using System.Reflection.Metadata;

namespace Diploma.Controllers
{
    public enum Colors
    {
        red,
        blue,
        yellow
    }
    public class RabbitMQBusService: BackgroundService
    {

        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private readonly IHubContext<MessageHub> _hubContext;
        //private readonly IServiceProvider _provider;
        //private readonly DBContext _context;
        private readonly MessageHandler _messageHandler;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        //public IModel Model => throw new NotImplementedException();


        public RabbitMQBusService(IHubContext<MessageHub> hubContext, IServiceProvider provider)
        {
            _hubContext = hubContext;

            //_provider = provider;

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "data_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            _messageHandler = new MessageHandler(provider);
        }

       

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            stoppingToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Получение.....");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                // Обрабатываем полученное сообщение
                
                //Debug.WriteLine($"Получено сообщение: {content}");
                Tuple<string, Message> message = await _messageHandler.CreateMessage(content);
                if (message.Item2 == Message.Warning) 
                {
                    Console.WriteLine("---------------");
                    SendMessage(message.Item1, Colors.yellow);
                }
                else if (message.Item2 == Message.Alert)
                {
                    Console.WriteLine("+++++++++++++++");
                    SendMessage(message.Item1, Colors.red);
                }

                //Console.WriteLine($"Получено сообщение: {content}");

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("data_queue", false, consumer);

            return Task.CompletedTask;
		}

        private async void SendMessage(string message, Colors color)
        {
            await _hubContext.Clients.All.SendAsync("Notify", $": {message} - {DateTime.Now.ToShortTimeString()}", color.ToString());
        }

		public override void Dispose()
		{
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
