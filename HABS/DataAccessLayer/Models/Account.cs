using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Account
    {
        public Account()
        {
            Patients = new HashSet<Patient>();
        }

        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
