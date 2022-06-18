using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class TestRecord
    {
        public TestRecord()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public long Id { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public DateTime? Date { get; set; }
        public int? NumericalOrder { get; set; }
        public int Status { get; set; }
        public string ResultFileLink { get; set; }
        public string PatientName { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public long? RoomId { get; set; }
        public long? BillId { get; set; }
        public long? PatientId { get; set; }
        public long? CheckupRecordId { get; set; }
        public long? OperationId { get; set; }
        public string OperationName { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual CheckupRecord CheckupRecord { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
