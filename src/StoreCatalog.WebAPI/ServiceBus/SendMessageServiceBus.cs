﻿using GeekBurger.StoreCatalog.Contract.Model;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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

        private async Task EnsureTopicIsCreated()
        {
            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _configuration["serviceBus:clientId"],
                _configuration["serviceBus:clientSecret"],
                _configuration["serviceBus:tenantId"], 
                AzureEnvironment.AzureGlobalCloud);

            var serviceBusManager = ServiceBusManager.Authenticate(credentials, _configuration["serviceBus:subscriptionId"]);
            var serviceBusNamespace = serviceBusManager.Namespaces.GetByResourceGroup(_configuration["serviceBus:resourceGroup"], _configuration["serviceBus:namespaceName"]);

            var topics = await serviceBusNamespace.Topics.ListAsync();

            var topicStoreCatalogReady = topics.FirstOrDefault(t => t.Name.Equals("StoreCatalogReady", StringComparison.InvariantCultureIgnoreCase));
            var topicUserWithLessOffer = topics.FirstOrDefault(t => t.Name.Equals("UserWithLessOffer", StringComparison.InvariantCultureIgnoreCase));

            var topicProductChanged = topics.FirstOrDefault(t => t.Name.Equals("ProductChanged", StringComparison.InvariantCultureIgnoreCase));
            var topicProductionAreaChanged = topics.FirstOrDefault(t => t.Name.Equals("ProductionAreaChanged", StringComparison.InvariantCultureIgnoreCase));

            SubscriptionInner subscriptionInner = new SubscriptionInner()
            {

            };

            #region Send Messages
            if (topicStoreCatalogReady == null)
            {
                topicStoreCatalogReady = await serviceBusNamespace.Topics.Define("StoreCatalogReady")
                    .WithSizeInMB(1024)
                    .CreateAsync();
            }

            if (topicUserWithLessOffer == null)
            {
                topicUserWithLessOffer = await serviceBusNamespace.Topics.Define("UserWithLessOffer")
                    .WithSizeInMB(1024)
                    .CreateAsync();
            }
            #endregion

            #region Receive Messages
            if (topicProductChanged == null)
            {
                topicProductChanged = await serviceBusNamespace.Topics.Define("ProductChanged")
                    .WithSizeInMB(1024)
                    .CreateAsync();
            }

            if (topicProductionAreaChanged == null)
            {
                topicProductionAreaChanged = await serviceBusNamespace.Topics.Define("ProductionAreaChanged")
                    .WithSizeInMB(1024)
                    .CreateAsync();
            } 
            #endregion

            /*Criação da subscrição no tópico de produção*/
            await topicProductChanged.Subscriptions.Define("StoreCatalog-ProductChanged").CreateAsync();

            /*Criação da subscrição no tópico de área de produção*/
            await topicProductionAreaChanged.Subscriptions.Define("StoreCatalog-ProductionAreaChanged").CreateAsync();
        }

        public async Task SendStoreCatalogReadyAsync()
        {
            try
            {
                await EnsureTopicIsCreated();

                var topicClient = new TopicClient(connectionString, "StoreCatalogReady");

                var ready = new StoreCatalogReady() { IsReady = true };

                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ready)));

                await topicClient.SendAsync(message);
                await topicClient.CloseAsync();
            }
            catch (Exception)
            {
            }
        }

        public async Task SendUserWithLessOffer(dynamic userInfo)
        {
            try
            {
                await EnsureTopicIsCreated();

                var topicClient = new TopicClient(connectionString, "UserWithLessOffer");
                

                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userInfo)));

                await topicClient.SendAsync(message);
                await topicClient.CloseAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
