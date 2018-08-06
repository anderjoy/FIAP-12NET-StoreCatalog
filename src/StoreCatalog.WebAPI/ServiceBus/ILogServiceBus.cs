using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.ServiceBus
{
    public interface ILogServiceBus
    {
        Task SendMessagesAsync(string message);
    }
}
