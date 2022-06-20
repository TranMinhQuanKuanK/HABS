using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Doctor
    {
        public enum DoctorType
        {
            BS_DA_KHOA,
            BS_CHUYEN_KHOA,
            BS_XET_NGHIEM
        }
        public Doctor()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            Schedules = new HashSet<Schedule>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DoctorType Type { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public int? AverageCheckupDuration { get; set; }

        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
