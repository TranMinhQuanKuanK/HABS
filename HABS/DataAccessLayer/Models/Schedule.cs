using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Schedule
    {
        public long Id { get; set; }
        public long DoctorId { get; set; }
        public long RoomId { get; set; }
        public int Weekday { get; set; }
        public int Session { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Room Room { get; set; }
    }
}
