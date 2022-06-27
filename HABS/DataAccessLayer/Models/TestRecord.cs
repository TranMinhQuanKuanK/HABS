using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class TestRecord
    {
        public enum TestRecordStatus
        {
            CHUA_DAT_LICH,
            DA_DAT_LICH,
            DA_THANH_TOAN,
            DANG_TIEN_HANH,
            CHO_KET_QUA,
            HOAN_THANH,
            DA_HUY,
            DA_XOA
        }
        public TestRecord()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public long Id { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public DateTime? Date { get; set; }
        public int? NumericalOrder { get; set; }
        public TestRecordStatus Status { get; set; }
        public string ResultFileLink { get; set; }
        public string PatientName { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public long? RoomId { get; set; }
        public long? PatientId { get; set; }
        public long? CheckupRecordId { get; set; }
        public long? OperationId { get; set; }
        public string OperationName { get; set; }
        public long? DoctorId { get; set; }
        public string DoctorName { get; set; }

        public virtual CheckupRecord CheckupRecord { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
