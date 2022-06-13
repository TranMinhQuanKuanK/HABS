using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Notification
{
    public interface INotificationService
    {
        Task SendNotificationOutOfStockProduct(int productId, int brandId, string productName);
        Task SendNotificationStoreApproved(int storeId, int brandId, string storeName);
        Task SendNotificationStoreRejected(int storeId, int brandId, string storeName);
    }
}
