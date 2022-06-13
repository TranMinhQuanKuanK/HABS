using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class BillDetail
    {
        public long Id { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
        public int InsuranceStatus { get; set; }
        public long? CheckupRecordId { get; set; }
        public long? TestRecordId { get; set; }
        public long? OperationId { get; set; }
        public string OperationName { get; set; }

        public virtual CheckupRecord CheckupRecord { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual TestRecord TestRecord { get; set; }
    }
}
