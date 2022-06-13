using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Patient
    {
        public Patient()
        {
            CheckupRecords = new HashSet<CheckupRecord>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Bhyt { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
