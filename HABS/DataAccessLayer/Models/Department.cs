using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Department
    {
        public enum DepartmentStatus
        {
            CO_MO_KHAM,
            KHONG_HO_TRO
        }
        public Department()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            Operations = new HashSet<Operation>();
            Rooms = new HashSet<Room>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public DepartmentStatus? Status { get; set; }

        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
