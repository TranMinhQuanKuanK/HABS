using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public enum SessionType
    {
        SANG,
        CHIEU,
        TOI
    }

    public partial class Schedule
    {

        public long Id { get; set; }
        public long DoctorId { get; set; }
        public long RoomId { get; set; }
        public DayOfWeek Weekday { get; set; }
        public SessionType Session { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Room Room { get; set; }
    }
}
