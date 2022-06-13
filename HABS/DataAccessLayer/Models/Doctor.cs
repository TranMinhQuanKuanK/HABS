using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            Schedules = new HashSet<Schedule>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int AverageCheckupDuration { get; set; }
        public long? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
