using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Account
    {
        public enum UserStatus
        {
            BINH_THUONG,
            DA_XOA,
            CHUA_XAC_THUC_OTP
        }
        public Account()
        {
            FcmTokenMobiles = new HashSet<FcmTokenMobile>();
            Patients = new HashSet<Patient>();
        }

        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public UserStatus? Status { get; set; }

        public virtual ICollection<FcmTokenMobile> FcmTokenMobiles { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}
