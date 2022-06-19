using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Operation
    {
        public enum OperationType
        {
            KHAM_BENH,
            XET_NGHIEM
        }
        public Operation()
        {
            BillDetails = new HashSet<BillDetail>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int InsuranceStatus { get; set; }
        public int Status { get; set; }
        public OperationType Type { get; set; }
        public string Note { get; set; }
        public long? RoomTypeId { get; set; }
        public long? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
