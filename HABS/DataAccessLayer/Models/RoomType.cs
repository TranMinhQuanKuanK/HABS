using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class RoomType
    {
        public RoomType()
        {
            Operations = new HashSet<Operation>();
            Rooms = new HashSet<Room>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
