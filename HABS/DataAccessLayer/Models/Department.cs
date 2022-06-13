using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Department
    {
        public Department()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            Doctors = new HashSet<Doctor>();
            Operations = new HashSet<Operation>();
            Rooms = new HashSet<Room>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
