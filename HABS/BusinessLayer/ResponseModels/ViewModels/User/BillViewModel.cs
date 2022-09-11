using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.User
{
    public class BillViewModel
    {
        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public int Total { get; set; }
        public string TotalInWord { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public string PatientName { get; set; }
        public string PhoneNo { get; set; }
        public string AccountPhoneNo { get; set; }
        public long PatientId { get; set; }
        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        //VnPay
        public int PaymentMethod { get; set; } //0 là tiền mặt, 1 là vnpay
        public string BankCode { get; set; } // "NCB", "VIETCOMBANK"
        public string BankName { get; set; } // "Ngân hàng nhà nước Việt Nam"
        public string BankLogoLink { get; set; } // má từ từ xài cái này, lỗi mẹ r  :'(
        public string BankTranNo { get; set; } // Mã giao dịch từ ngân hàng
        public string CardType { get; set; } // Loại thẻ thanh toán
        public string  VnPayTranNo { get; set; } //Mã giao dịch VnPay
        public string TransactionStatus { get; set; } //Tình trạng của giao dịch (tra discord)

        public string CashierName { get; set; }
        public long? CashierId { get; set; }
        public List<BillDetailViewModel> Details { get; set; }
    }
}
