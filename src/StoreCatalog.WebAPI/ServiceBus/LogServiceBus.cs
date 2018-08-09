using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.ServiceBus
{
    public class LogServiceBus : ILogServiceBus
    {
        private const string Topic = "Log";
        private const string MicroService = "StoreCatalog";

        private readonly IConfiguration _configuration;
        private readonly List<Message> _messages;
        private Task _lastTask;

        public LogServiceBus(IConfiguration configuration)
        {
            _configuration = configuration;
            _messages = new List<Message>();

            EnsureTopicIsCreated();
        }

        private void EnsureTopicIsCreated()
        {
            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _configuration["serviceBus:clientId"],
                _configuration["serviceBus:clientSecret"],
                _configuration["serviceBus:tenantId"],
                AzureEnvironment.AzureGlobalCloud);

            var serviceBusManager = ServiceBusManager.Authenticate(credentials, _configuration["serviceBus:subscriptionId"]);
            var serviceBusNamespace = serviceBusManager.Namespaces.GetByResourceGroup(_configuration["serviceBus:resourceGroup"], _configuration["serviceBus:namespaceName"]);

            if (!serviceBusNamespace.Topics.List()
                .Any(topic => topic.Name
                    .Equals(Topic, StringComparison.InvariantCultureIgnoreCase)))
            {
                serviceBusNamespace.Topics.Define(Topic)
                    .WithSizeInMB(1024)
                    .Create();
            };
        }
        
        private Message GetMessage(string message)
        {
            var productChangedByteArray = Encoding.UTF8.GetBytes(message);

            return new Message
            {
                Body = productChangedByteArray,
                MessageId = Guid.NewGuid().ToString(),
                Label = MicroService
            };
        }

        public async Task SendMessagesAsync(string message)
        {
            //return;

            _messages.Add(GetMessage($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} -> {message}"));

            if (_lastTask != null && !_lastTask.IsCompleted)
                return;

            var topicClient = new TopicClient(_configuration["serviceBus:connectionString"], Topic);

            _lastTask = SendAsync(topicClient);

            await _lastTask;

            var closeTask = topicClient.CloseAsync();
            await closeTask;
            HandleException(closeTask);
        }

        private async Task SendAsync(TopicClient topicClient)
        {
            int tries = 0;
            Message message;
            while (true)
            {
                if (_messages.Count <= 0)
                    break;

                lock (_messages)
                {
                    message = _messages.FirstOrDefault();
                }

                var sendTask = topicClient.SendAsync(message);
                await sendTask;
                var success = HandleException(sendTask);

                if (!success)
                    Thread.Sleep(10000 * (tries < 60 ? tries++ : tries));
                else
                    _messages.Remove(message);
            }
        }

        private bool HandleException(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList().ForEach(innerException =>
            {
                Console.WriteLine($"Error in SendAsync task: {innerException.Message}. Details:{innerException.StackTrace} ");

                if (innerException is ServiceBusCommunicationException)
                    Console.WriteLine("Connection Problem with Host. Internet Connection can be down");
            });

            return false;
        }
    }
}
