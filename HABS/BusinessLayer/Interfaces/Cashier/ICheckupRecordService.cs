using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Cashier
{
    public interface IBillService
    {
        List<BillViewModel> GetBills(BillSearchModel search);
        BillViewModel GetBillById(long id);
        Task PayABill(long billId, long cashierId);
        Task CancelABill(long billId, long cashierId);
        BillViewModel GetBillByQr(string qrCode);
    }
}
