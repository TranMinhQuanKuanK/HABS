using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Room
    {
        public Room()
        {
            Schedules = new HashSet<Schedule>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public string Note { get; set; }
        public long? DepartmentId { get; set; }
        public long? RoomTypeId { get; set; }

        public virtual Department Department { get; set; }
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
