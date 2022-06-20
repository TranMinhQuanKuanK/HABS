using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class DoctorLoginViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
    }
}
