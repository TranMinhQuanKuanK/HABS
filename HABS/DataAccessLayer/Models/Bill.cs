using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Bill
    {
        public enum BillStatus
        {
            CHUA_TT,
            DA_TT_EBANKING,
            DA_TT_TIEN_MAT,
            HUY,
            DANG_XU_LI_EBANKING
        }
        public enum PaymentMethodEnum
        {
           TIEN_MAT,
           VNPAY
        }
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public int Total { get; set; }
        public string TotalInWord { get; set; }
        public BillStatus Status { get; set; }
        public string Content { get; set; }
        public string PatientName { get; set; }
        public string CashierName { get; set; }
        public long? CashierId { get; set; }
        public long? PatientId { get; set; }
        public string PhoneNo { get; set; }
        public string AccountPhoneNo { get; set; }
        public PaymentMethodEnum? PaymentMethod { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankLogoLink { get; set; }
        public string BankTranNo { get; set; }
        public string CardType { get; set; }
        public DateTime? PayDate { get; set; }
        public string VnPayTransactionNo { get; set; }
        public string TransactionStatus { get; set; }

        public virtual Cashier Cashier { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
