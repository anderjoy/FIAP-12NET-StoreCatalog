using GeekBurger.Productions.Contract;
using GeekBurger.Products.Contract;
using GeekBurger.StoreCatalog.WebAPI.Helpers;
using GeekBurger.StoreCatalog.WebAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.ServiceBus
{
    public class ReceiveMessageServiceBus : IReceiveMessageServiceBus
    {
        private readonly IConfiguration _configuration;
        private readonly IProductRepository _productRepository;
        private readonly IProductionAreaRepository _productionAreaRepository;

        static ISubscriptionClient _subscriptionClientProductChanged;
        static ISubscriptionClient _subscriptionClientProductionAreaChanged;

        public ReceiveMessageServiceBus(IConfiguration configuration,
            IProductRepository productRepository,
            IProductionAreaRepository productionAreaRepository)
        {
            _configuration = configuration;
            _productRepository = productRepository;
            _productionAreaRepository = productionAreaRepository;

            _subscriptionClientProductChanged = new SubscriptionClient(
                _configuration["serviceBus:connectionString"],
                "ProductChanged",
                "StoreCatalog-ProductChanged",
                ReceiveMode.PeekLock);

            _subscriptionClientProductionAreaChanged = new SubscriptionClient(
                _configuration["serviceBus:connectionString"],
                "ProductionAreaChanged",
                "StoreCatalog-ProductionAreaChanged",
                ReceiveMode.PeekLock);

            RegisterOnMessageHandlerAndReceiveMessages();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 3,
                AutoComplete = false
            };

            _subscriptionClientProductChanged.RegisterMessageHandler(ProcessMessagesProductChangedAsync, messageHandlerOptions: messageHandlerOptions);
            _subscriptionClientProductionAreaChanged.RegisterMessageHandler(ProcessMessagesProductionAreaChangedAsync, messageHandlerOptions: messageHandlerOptions);
        }

        private async Task ProcessMessagesProductChangedAsync(Message message, CancellationToken token)
        {
            try
            {
                ProductChangedMessage productContract = JsonConvert.DeserializeObject<ProductChangedMessage>(Encoding.UTF8.GetString(message.Body));

                switch (productContract.State)
                {
                    case ProductState.Deleted:
                        await _productRepository.DeleteAsync(productContract.Product.ToProduct());
                        break;
                    case ProductState.Modified:
                        await _productRepository.UpsertProductAsync(productContract.Product.ToProduct());
                        break;
                    case ProductState.Added:
                        await _productRepository.UpsertProductAsync(productContract.Product.ToProduct());
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task ProcessMessagesProductionAreaChangedAsync(Message message, CancellationToken token)
        {
            try
            {
                ProductionAreaChangedMessage productionAreaChanged = JsonConvert.DeserializeObject<ProductionAreaChangedMessage>(Encoding.UTF8.GetString(message.Body));

                await _productionAreaRepository.UpsertAsync(productionAreaChanged.ProductionArea.ToProductionAreas());
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
