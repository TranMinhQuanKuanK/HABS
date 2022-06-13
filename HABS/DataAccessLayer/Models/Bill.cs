using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Bill
    {
        public Bill()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public int Total { get; set; }
        public string TotalInWord { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public string PatientName { get; set; }
        public string CashierName { get; set; }
        public long? CashierId { get; set; }

        public virtual Cashier Cashier { get; set; }
        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
