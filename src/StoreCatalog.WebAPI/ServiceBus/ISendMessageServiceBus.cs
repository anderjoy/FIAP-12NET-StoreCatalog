using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.StoreCatalog.WebAPI.ServiceBus
{
    public interface ISendMessageServiceBus
    {
        Task SendStoreCatalogReadyAsync();
        Task SendUserWithLessOffer(dynamic userInfo);
    }
}
