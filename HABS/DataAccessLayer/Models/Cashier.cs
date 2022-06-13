using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Cashier
    {
        public Cashier()
        {
            Bills = new HashSet<Bill>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
