using System.Text;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
namespace RabbitAmazon
{
    public class RabbitMqService
    {
        private readonly string _hostname = "b-3278bb84-2b68-4123-bfa1-3d231dfd9bb2.mq.us-east-1.amazonaws.com";
        private readonly string _username = "guestAmazon";  // замените на ваше имя пользователя
        private readonly string _password = "kuki1212kuki";  // замените на ваш пароль
        private readonly int _port = 5671; // порт для AMQPS

        private IConnection _connection;


        public RabbitMqService()
        {
            CreateConnection();
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password,
                    Port = _port,
                    Ssl = new SslOption
                    {
                        Enabled = true,
                        ServerName = _hostname
                    }
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

     

        public string ReceiveMessage()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "my_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var result = channel.BasicGet("my_queue", autoAck: true);

                if (result != null)
                {
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine($"[x] Received {message}");
                    return message;
                }
                else
                {
                    Console.WriteLine("[x] No message available in the queue.");
                    return "No messages";
                }
            }
        }


        public (string message, ulong deliveryTag) ReceiveMessageWithoutDeleting()
        {
            if (_connection == null) return ("No connection available", 0);

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "my_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var result = channel.BasicGet("my_queue", autoAck: false); // Set autoAck to false
                if (result != null)
                {
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine($"[x] Received {message}");
                    return (message, result.DeliveryTag); // Return message and delivery tag as a tuple
                }
                else
                {
                    Console.WriteLine("[x] No message available in the queue.");
                    return ("No messages", 0); // Return a tuple with a message and a default delivery tag
                }
            }
        }

        public (List<Vehicle> vehicles, ulong deliveryTag) ReceiveMessageVehicleWithoutDeleting()
        {
            if (_connection == null) return (new List<Vehicle>(), 0);

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: "my_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var result = channel.BasicGet("my_queue", autoAck: false); // Set autoAck to false
                if (result != null)
                {
                    // Convert the message body to a UTF-8 string
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());

                    // Deserialize the message from JSON into a list of Vehicle objects
                    var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(message);

                    Console.WriteLine($"[x] Received {message}");
                    return (vehicles, result.DeliveryTag); // Return the deserialized list and delivery tag as a tuple
                }
                else
                {
                    Console.WriteLine("[x] No message available in the queue.");
                    return (new List<Vehicle>(), 0); // Return an empty list and a default delivery tag
                }
            }
        }



    }
}

