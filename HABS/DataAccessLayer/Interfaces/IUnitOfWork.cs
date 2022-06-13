using DataAcessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Bill> BillRepository { get; }
        IGenericRepository<BillDetail> BillDetailRepository { get; }
        IGenericRepository<Brand> BrandRepository { get; }
        IGenericRepository<Cashier> CashierRepository { get; }
        IGenericRepository<UserBrand> UserBrandRepository { get; }
        IGenericRepository<DailyRevenue> DailyRevenueRepository { get; }
        IGenericRepository<Event> EventRepository { get; }
        IGenericRepository<EventDetail> EventDetailRepository { get; }
        IGenericRepository<Receipt> ReceiptRepository { get; }
        IGenericRepository<ReceiptDetail> ReceiptDetailRepository { get; }
        IGenericRepository<Stock> StockRepository { get; }
        IGenericRepository<Store> StoreRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<FcmtokenMobile> FcmTokenMobileRepository { get; }
        GroceryCloud18th2Context Context();

        Task SaveChangesAsync();
    }
}
