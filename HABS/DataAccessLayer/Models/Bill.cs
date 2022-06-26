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
            TT_EBANKING,
            TT_TIEN_MAT,
            HUY
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

        public virtual Cashier Cashier { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
