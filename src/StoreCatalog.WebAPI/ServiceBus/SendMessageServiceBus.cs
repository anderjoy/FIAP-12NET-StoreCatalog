using GeekBurger.StoreCatalog.Contract.Model;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.ServiceBus
{
    public class SendMessageServiceBus : ISendMessageServiceBus
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public SendMessageServiceBus(IConfiguration configuration)
        {
            _configuration = configuration;

            connectionString = _configuration["serviceBus:connectionString"];
        }

        public async Task SendStoreCatalogReadyAsync()
        {
            try
            {
                var topicClient = new TopicClient(connectionString, "storecatalog");

                var ready = new StoreCatalogReady() { IsReady = true };

                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ready)));

                await topicClient.SendAsync(message);
                await topicClient.CloseAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
